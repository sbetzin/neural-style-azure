import os
from pathlib import Path
import numpy as np
import tempfile
import tensorflow as tf
import mediapy
from PIL import Image
import cog

from eval import interpolator, util


class Predictor(cog.Predictor):
    def setup(self):
       print("setup done")
       
    @cog.input(
        "frame1",
        type=Path,
        help="The first input frame",
    )
    @cog.input(
        "frame2",
        type=Path,
        help="The second input frame",
    )
    @cog.input(
        "times_to_interpolate",
        type=int,
        default=1,
        min=1,
        max=8,
        help="Controls the number of times the frame interpolator.",
    )
    def predict(self, frame1, frame2, times_to_interpolate):
        print("DONE")
