#!/usr/bin/env python
import numpy as np
import cv2

def find_target_size(content_img_file, max_size):
    img = cv2.imread(content_img_file)
    h, w, _ = img.shape
    target_shape = (w,h)
  
    # resize if > max size
    if h > w and h > max_size:
        w = (float(max_size) / float(h)) * w
        target_shape = (int(w), max_size)
    if w > max_size:
        h = (float(max_size) / float(w)) * h
        target_shape = (max_size, int(h))
    
    return target_shape    
    
def create_image_with_original_colors(content_file, out_file_origcolor_0, out_file_origcolor_1):
    content_image = read_image(content_file)
    out_image = read_image(out_file_origcolor_0)
        
    out_image_orginal_colors = to_original_colors(content_image, out_image)
    
    save_image(out_file_origcolor_1, out_image_orginal_colors)

def save_image(path, image):
    cv2.cv2.imwrite(path, image)

def read_image(path):
    img = cv2.cv2.imread(path, cv2.cv2.IMREAD_COLOR)

    return img

def to_original_colors(content_image, output_image):
    h, w, _ = output_image.shape

    content_image = cv2.cv2.resize(content_image, dsize=(int(w), int(h)), interpolation=cv2.cv2.INTER_AREA)

    content_cvt = cv2.cv2.cvtColor(content_image, cv2.cv2.COLOR_BGR2YUV)
    output_cvt = cv2.cv2.cvtColor(output_image, cv2.cv2.COLOR_BGR2YUV)

    c1, _, _ = cv2.cv2.split(output_cvt)
    _, c2, c3 = cv2.cv2.split(content_cvt)
    merged = cv2.cv2.merge((c1, c2, c3))
    dst = cv2.cv2.cvtColor(merged, cv2.cv2.COLOR_YUV2BGR).astype(np.float32)

    return dst
