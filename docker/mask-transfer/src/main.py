import cv2
import os
import itertools
import numpy as np

def change_path(target_path, image_path):
  file_name_with_ext = os.path.basename(image_path)

  return os.path.join(target_path, file_name_with_ext)

def get_subfolders(video_path, folder_name, filter=None):
    target_dir = os.path.join(video_path, folder_name)
    if filter is not None:
        subfolders = [os.path.split(f.path)[1] for f in os.scandir(target_dir) if f.is_dir() and filter in os.path.split(f.path)[1]]
    else:
        subfolders = [os.path.split(f.path)[1] for f in os.scandir(target_dir) if f.is_dir()]
    return subfolders


def load_mask(mask_path, shape):
    mask = cv2.imread(mask_path, cv2.IMREAD_GRAYSCALE)
    width, height, _ = shape
    mask = cv2.resize(mask, (height, width), interpolation=cv2.INTER_CUBIC).astype('float32')

    # Perform binarization of mask
    _, mask = cv2.threshold(mask, 127, 255, cv2.THRESH_BINARY)

    max = np.amax(mask)
    mask /= max

    return mask

def get_images(folder_path):
    file_list = os.listdir(folder_path)

    # Filtern Sie die Liste, um nur JPEG-Dateien (keine Unterordner) einzuschließen
    file_list = [os.path.join(folder_path, file) for file in file_list if os.path.isfile(os.path.join(folder_path, file)) and file.lower().endswith(('.jpg', '.jpeg'))]

    # Sortieren der Liste alphabetisch
    file_list = sorted(file_list)

    return file_list

def load_image(image_path, shape):
    image = cv2.imread(image_path)
    width, height, _ = shape
    image = cv2.resize(image, (height, width), interpolation=cv2.INTER_CUBIC)

    return image

# def mask_content(content, generated, mask):
#     result = np.zeros_like(generated)
#     result[..., :] = generated[..., :]
#     for i in range(generated.shape[0]):
#         for j in range(generated.shape[1]):
#             if mask[i, j] == 0.:
#                 result[i, j, :] = content[i, j, :]

#     return result

def mask_content(content, generated, mask):
    result = np.copy(generated)
    result[mask == 0] = content[mask == 0]
    return result


def generate_paths(base_path, video_name, mask_name, style_name):
    in_path = os.path.join (base_path, video_name, "in")
    mask_path = os.path.join (base_path, video_name, "masks", mask_name)
    style_path = os.path.join (base_path, video_name, "styles", style_name)
    out_path = os.path.join (base_path, video_name, "out", f"{style_name}_mask_{mask_name}")
    
    return in_path, mask_path, style_path, out_path

def generate_masked_style_files(base_path, video_name, mask_name, style_name):
  in_path, mask_path, style_path, out_path = generate_paths(base_path, video_name, mask_name, style_name)
  print(f'   mask_path={mask_path}, style_path={style_path}, out_path={out_path}')

  os.makedirs(out_path, exist_ok=True)

  style_files = get_images(style_path)
  image_files = get_images(in_path)
  mask_files = get_images(mask_path)
  out_files= get_images(out_path)

  print (f'   {style_name}={len(style_files)}, image_files={len(image_files)}, {mask_name}={len(mask_files)}, out_files={len(out_files)}')

  # Überprüfen, ob alle Listen die gleiche Anzahl an Dateien enthalten und weniger outfiles sind
  if len(style_files) == len(image_files) == len(mask_files) != len(out_files):
      # Die Listen gemeinsam durchiterieren
      for style_file, image_file, mask_file in zip(style_files, image_files, mask_files):
          print (f"      Working on {style_file}")
          
          style = cv2.imread(style_file)
          mask = load_mask(mask_file, style.shape)
          image = load_image(image_file, style.shape)

          masked_style = mask_content(image, style, mask)
          masked_style_file = change_path(out_path, image_file)

          cv2.imwrite(masked_style_file, masked_style)



#base_path = "/nft/video"

video_name = 'norwegen-19_move'
base_path = "C:\\Users\\gensb\\OneDrive\\_nft\\video"
video_path = os.path.join(base_path, video_name)

mask_names = get_subfolders(video_path, "masks")
style_names = get_subfolders(video_path, "styles", "enhanced")

for mask_name, style_name in itertools.product(mask_names, style_names):
    print (f"starting for mask={mask_name}, style={style_name}")
    generate_masked_style_files(base_path, video_name, mask_name, style_name)
