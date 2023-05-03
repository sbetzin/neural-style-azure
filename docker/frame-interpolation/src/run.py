import os
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
    image_files = [os.path.join(path, file) for file in all_files if os.path.splitext(file)[1].lower() in extensions]

    return sorted(image_files)

def concatenate_videos(mp4_files: list, target_video_file: str):
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
    print(f'saving {video_file}')

    ffmpeg_path = util.get_ffmpeg_path()
    media.set_ffmpeg(ffmpeg_path)
    media.write_video(video_file, frames, fps=fps)
    
def main(target_path: str, fps: int, times_to_interpolate: int, block_height: int, block_width: int):
    print("GPUs Available: ", len(tf.config.list_physical_devices('GPU')))

    intermediate_path = '/intermediate'
    base_path = '/nft/video'

    clear_path(intermediate_path)

    input_files = get_files(target_path, ['.jpg'])
    print (f'Found {len(input_files)} input files')

    frame_sets = list(zip(input_files[:-1], input_files[1:]))

    for index, (frame1, frame2) in enumerate(frame_sets):
        predict_one (frame1, frame2, f'{intermediate_path}/out_{index}.mp4',fps, times_to_interpolate, block_height, block_width)

    intermediate_videos = get_files(intermediate_path, ['.mp4'])
    print (f'Found {len(intermediate_videos)} input files')

    if len(intermediate_videos):
        concatenate_videos(intermediate_videos, f'{target_path}/out.mp4')
    

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Process video parameters.')
    parser.add_argument('target_path', type=str, help='Path to the input images')
    parser.add_argument('fps', type=int, default=30, help='Frames per second')
    parser.add_argument('times_to_interpolate', default=4, type=int, help='Number of times to interpolate')
    parser.add_argument('block_height', type=int, default=1, help='Block height')
    parser.add_argument('block_width', type=int, default=1, help='Block width')

    args = parser.parse_args()

    main(args.target_path, args.fps, args.times_to_interpolate, args.block_height, args.block_width)
