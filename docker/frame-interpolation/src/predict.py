from cog import BasePredictor, Path, Input
from eval import interpolator as interpolator_lib
from eval import util
import os
import glob
from pathlib import Path
import numpy as np
import tensorflow as tf
import mediapy as media
from PIL import Image



class Predictor(BasePredictor):
    def setup(self):
        print("Done setup")

    def predict(self,
            target_path: str = Input(description="Path to the input images"),
            fps: int = Input(description="Frames per second" , default=30),
            times_to_interpolate: int = Input(description="Number of times to interpolate" , default=4),
            block_height: int = Input(description="Block height" , default=1),
            block_width: int = Input(description="Block width" , default=1),
    ) -> str:
        
        gpus = tf.config.list_physical_devices('GPU')
        print("GPUs Available: ", len(gpus))

        return "Done"
    