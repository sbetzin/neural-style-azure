import os

# 0: Alle Meldungen werden angezeigt (Standardverhalten)
# 1: Nur Warnungen und Fehlermeldungen werden angezeigt
# 2: Nur Fehlermeldungen werden angezeigt
# 3: Keine Meldungen werden angezeigt
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '2'

import glob
from pathlib import Path
import numpy as np
import tensorflow as tf
import mediapy as media
from PIL import Image
from eval import interpolator as interpolator_lib
from eval import util
import argparse

def clear_path(path: str):
    mp4_files = glob.glob(os.path.join(path, "*.mp4"))
    for file in mp4_files:
        print(f'   removing {file}')
        os.remove(file)

def get_files(path: str, extensions) -> list:
    all_files = os.listdir(path)
    files = [os.path.join(path, file) for file in all_files if os.path.splitext(file)[1].lower() in extensions]

    return sorted(files)

def concatenate_videos(mp4_files: list,intermediate_path:str, target_video_file: str):
    file_name = f"{intermediate_path}/filelist.txt";
    # Erstelle eine temporäre Textdatei, die die Liste der MP4-Dateien enthält
    with open(file_name, "w") as file:
        for mp4_file in mp4_files:
            file.write(f"file '{mp4_file}'\n")

    # Setze den Pfad zu ffmpeg
    ffmpeg_path = util.get_ffmpeg_path()
    media.set_ffmpeg(ffmpeg_path)

    # Führe den Befehl aus, um die Videos zusammenzufügen
    command = f"{ffmpeg_path} -y -f concat -safe 0 -i {file_name} -c copy {target_video_file}"
    os.system(command)

    # Lösche die temporäre Textdatei
    os.remove(file_name)

def predict_one(frame1, frame2, video_file, fps, times_to_interpolate, block_height, block_width):
    interpolator = interpolator_lib.Interpolator("/pretrained_models/film_net/Style/saved_model", None, [block_height, block_width])

    # make sure 2 images are the same size
    img1 = Image.open(str(frame1))
    img2 = Image.open(str(frame2))

    assert img1.size == img2.size, "Images must be same size"

    input_frames = [str(frame1), str(frame2)]

    frames = list(util.interpolate_recursively_from_files(input_frames, times_to_interpolate, interpolator))
    
    ## Experimental:
    frames.pop(-1)
    
    print(f'saving {video_file}')

    ffmpeg_path = util.get_ffmpeg_path()
    media.set_ffmpeg(ffmpeg_path)
    media.write_video(video_file, frames, fps=fps)
    
def main(target_path: str, out_name: str, fps: int, times_to_interpolate: int, block_height: int, block_width: int, loop: bool):
    gpus = tf.config.list_physical_devices('GPU')
    print("GPUs Available: ", len(gpus))

    intermediate_path = f'/intermediate'
    os.makedirs(intermediate_path,exist_ok=True)

    clear_path(intermediate_path)

    input_files = get_files(target_path, ['.jpg'])
    print (f'Found {len(input_files)} input files')
    
    if loop:
        input_files.append(input_files[0])

    frame_sets = list(zip(input_files[:-1], input_files[1:]))

    for index, (frame1, frame2) in enumerate(frame_sets):
        print(f"Working on {frame1}, {frame2}")
        predict_one (frame1, frame2, f'{intermediate_path}/out_{index:04d}.mp4',fps, times_to_interpolate, block_height, block_width)

    intermediate_videos = get_files(intermediate_path, ['.mp4'])
    print (f'Found {len(intermediate_videos)} input files')

    if len(intermediate_videos):
        target_video_file = f'{target_path}/{out_name}}.mp4'
        concatenate_videos(intermediate_videos, intermediate_path, target_video_file)
    

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Process video parameters.')
    parser.add_argument('--target_path', required=True, type=str, help='Path to the input images')
    parser.add_argument('--fps', type=int, required=False, default=30, help='Frames per second')
    parser.add_argument('--times_to_interpolate', required=False, default=4, type=int, help='Number of times to interpolate')
    parser.add_argument('--block_height', type=int, required=False, default=1, help='Block height')
    parser.add_argument('--block_width', type=int, required=False, default=1, help='Block width')
    parser.add_argument('--loop', type=bool, required=False, default=True, help='loop to the first frame')
    parser.add_argument('--out_name', type=str, required=False, default="out.mp4")

    args = parser.parse_args()

    main(args.target_path, args.out_name, args.fps, args.times_to_interpolate, args.block_height, args.block_width, args.loop)
