from pathlib import Path
srgb_profile = (Path(__file__).resolve().parent / 'sRGB Profile.icc').read_bytes()
del Path