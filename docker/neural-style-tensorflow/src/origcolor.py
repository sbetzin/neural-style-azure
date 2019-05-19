#!/usr/bin/env python
import numpy as np
import cv2 as cv2

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
    # dst = preprocess(dst)

    return dst
