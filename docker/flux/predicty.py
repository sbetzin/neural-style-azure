from cog import BasePredictor, Input, Path
import os
import re
import time
import torch
import subprocess
import numpy as np
from typing import List
from diffusers import FluxPipeline
from weights import WeightsDownloadCache
from transformers import CLIPImageProcessor
from diffusers.pipelines.stable_diffusion.safety_checker import (
    StableDiffusionSafetyChecker
)

MODEL_CACHE = "FLUX.1-dev"
MODEL_URL = "https://weights.replicate.delivery/default/black-forest-labs/FLUX.1-dev/files.tar"
SAFETY_CACHE = "safety-cache"
FEATURE_EXTRACTOR = "/src/feature-extractor"
SAFETY_URL = "https://weights.replicate.delivery/default/sdxl/safety-1.0.tar"

ASPECT_RATIOS = {
    "1:1": (1024, 1024),
    "16:9": (1344, 768),
    "21:9": (1536, 640),
    "3:2": (1216, 832),
    "2:3": (832, 1216),
    "4:5": (896, 1088),
    "5:4": (1088, 896),
    "3:4": (896, 1152),
    "4:3": (1152, 896),
    "9:16": (768, 1344),
    "9:21": (640, 1536),
}

def download_weights(url, dest, file=False):
    start = time.time()
    print("downloading url: ", url)
    print("downloading to: ", dest)
    if not file:
        subprocess.check_call(["pget", "-xf", url, dest], close_fds=False)
    else:
        subprocess.check_call(["pget", url, dest], close_fds=False)
    print("downloading took: ", time.time() - start)

class Predictor(BasePredictor):
    def setup(self) -> None:
        """Load the model into memory to make running multiple predictions efficient"""
        start = time.time()
        # Dont pull weights
        os.environ["TRANSFORMERS_OFFLINE"] = "1"

        self.weights_cache = WeightsDownloadCache()

        print("Loading safety checker...")
        if not os.path.exists(SAFETY_CACHE):
            download_weights(SAFETY_URL, SAFETY_CACHE)
        self.safety_checker = StableDiffusionSafetyChecker.from_pretrained(
            SAFETY_CACHE, torch_dtype=torch.float16
        ).to("cuda")
        self.feature_extractor = CLIPImageProcessor.from_pretrained(FEATURE_EXTRACTOR)

        print("Loading Flux txt2img Pipeline")
        if not os.path.exists("FLUX.1-dev"):
            download_weights(MODEL_URL, ".")
        self.txt2img_pipe = FluxPipeline.from_pretrained(
            MODEL_CACHE,
            torch_dtype=torch.bfloat16
        ).to("cuda")

        # Save some VRAM by offloading the model to CPU
        vram = int(torch.cuda.get_device_properties(0).total_memory/(1024*1024*1024))
        if vram < 40:
            print("GPU VRAM < 40Gb - Offloading model to CPU")
            self.txt2img_pipe.enable_model_cpu_offload()

        print("setup took: ", time.time() - start)

    @torch.amp.autocast('cuda')
    def run_safety_checker(self, image):
        safety_checker_input = self.feature_extractor(image, return_tensors="pt").to("cuda")
        np_image = [np.array(val) for val in image]
        image, has_nsfw_concept = self.safety_checker(
            images=np_image,
            clip_input=safety_checker_input.pixel_values.to(torch.float16),
        )
        return image, has_nsfw_concept

    def aspect_ratio_to_width_height(self, aspect_ratio: str) -> tuple[int, int]:
        return ASPECT_RATIOS[aspect_ratio]

    @torch.inference_mode()
    def predict(
        self,
        prompt: str = Input(description="Prompt for generated image"),
        aspect_ratio: str = Input(
            description="Aspect ratio for the generated image",
            choices=list(ASPECT_RATIOS.keys()),
            default="1:1"),
        num_outputs: int = Input(
            description="Number of images to output.",
            ge=1,
            le=4,
            default=1,
        ),
        num_inference_steps: int = Input(
            description="Number of inference steps",
            ge=1,le=12,default=28,
        ),
        seed: int = Input(description="Random seed. Set for reproducible generation", default=None),
        output_format: str = Input(
            description="Format of the output images",
            choices=["webp", "jpg", "png"],
            default="webp",
        ),
        output_quality: int = Input(
            description="Quality when saving the output images, from 0 to 100. 100 is best quality, 0 is lowest quality. Not relevant for .png outputs",
            default=80,
            ge=0,
            le=100,
        ),
        hf_lora: str = Input(
            description="URL to the LoRA weights. Ex: alvdansen/frosting_lane_flux",
            default="https://replicate.delivery/yhqm/2MGk61LmrYamGBYtOWVe0pMUrOgvjjmKVhUDAG3mh2OPVfUTA/trained_model.tar",
        ),
        lora_scale: float = Input(
            description="Scale for the LoRA weights",
            ge=0,le=1, default=1,
        ),
        disable_safety_checker: bool = Input(
            description="Disable safety checker for generated images. This feature is only available through the API. See [https://replicate.com/docs/how-does-replicate-work#safety](https://replicate.com/docs/how-does-replicate-work#safety)",
            default=False,
        ),
    ) -> List[Path]:
        """Run a single prediction on the model"""
        if seed is None:
            seed = int.from_bytes(os.urandom(2), "big")
        print(f"Using seed: {seed}")

        width, height = self.aspect_ratio_to_width_height(aspect_ratio)
        max_sequence_length=256
        guidance_scale=0.0

        flux_kwargs = {}
        print(f"Prompt: {prompt}")
        print("txt2img mode")
        flux_kwargs["width"] = width
        flux_kwargs["height"] = height
        pipe = self.txt2img_pipe

        if hf_lora is not None:
            joint_attention_kwargs={"scale": lora_scale}
            flux_kwargs["joint_attention_kwargs"] = joint_attention_kwargs
            if re.match(r"^https?://replicate.delivery/[a-zA-Z0-9_-]+/[a-zA-Z0-9_-]+/trained_model.tar", hf_lora):
                print(f"Downloading LoRA weights from - Replicate URL: {hf_lora}")
                local_weights_cache = self.weights_cache.ensure(hf_lora)
                lora_path = os.path.join(local_weights_cache, "output/flux_train_replicate/lora.safetensors")
                pipe.load_lora_weights(lora_path)
            # Check for URL to a .safetensors file
            elif hf_lora.endswith('.safetensors'):
                print(f"Downloading LoRA weights from - safetensor URL: {hf_lora}")
                try:
                    lora_path = self.weights_cache.ensure(hf_lora, file=True)
                except Exception as e:
                    raise Exception(f"Error downloading LoRA weights from URL: {e}")
                pipe.load_lora_weights(lora_path)
            else:
                raise Exception(f"Invalid lora, must be either a: Replicate model.tar URL, or a URL to a .safetensors file: {hf_lora}")
        else:
            flux_kwargs["joint_attention_kwargs"] = None
            pipe.unload_lora_weights()

        generator = torch.Generator("cuda").manual_seed(seed)

        common_args = {
            "prompt": [prompt] * num_outputs,
            "guidance_scale": guidance_scale,
            "generator": generator,
            "num_inference_steps": num_inference_steps,
            "max_sequence_length": max_sequence_length,
            "output_type": "pil"
        }

        output = pipe(**common_args, **flux_kwargs)

        if hf_lora is not None:
            self.txt2img_pipe.unload_lora_weights()

        if not disable_safety_checker:
            _, has_nsfw_content = self.run_safety_checker(output.images)

        output_paths = []
        for i, image in enumerate(output.images):
            if not disable_safety_checker and has_nsfw_content[i]:
                print(f"NSFW content detected in image {i}")
                continue
            output_path = f"/tmp/out-{i}.{output_format}"
            if output_format != 'png':
                image.save(output_path, quality=output_quality, optimize=True)
            else:
                image.save(output_path)
            output_paths.append(Path(output_path))

        if len(output_paths) == 0:
            raise Exception("NSFW content detected. Try running it again, or try a different prompt.")

        return output_paths