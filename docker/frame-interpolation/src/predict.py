from cog import BasePredictor, Path, Input
from eval import interpolator as interpolator_lib

class Predictor(BasePredictor):
    def setup(self):
        """Load the model into memory to make running multiple predictions efficient"""
        self.interpolator = interpolator_lib.Interpolator("/pretrained_models/film_net/Style/saved_model", None, [block_height, block_width])

        print("Done setup")

    def predict(self,
            target_path: str = Input(description="Path to the input images"),
            fps: int = Input(description="Frames per second" , default=30),
            times_to_interpolate: int = Input(description="Number of times to interpolate" , default=4),
            block_height: int = Input(description="Block height" , default=1),
            block_width: int = Input(description="Block width" , default=1),
    ) -> str:
        # ... post-processing ...
        return "Done"
    