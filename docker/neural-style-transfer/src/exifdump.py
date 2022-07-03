#!/usr/bin/env python
import piexif
import os

def filename(filename):
    """Given a full path to a file, returns just its name, without path or extension"""
    return os.path.splitext(filename)[0]
  
def write_exif(target_file, config):
  exif = {"0th": {}, "Exif" : {}, "GPS" : {}, "1st" : {}}
  content_name = filename(config['content_img_name'])
  style_name =  filename(config['style_img_name'])
  keywords = "{0},{1}".format(content_name, style_name)
  
#   comment = "max_size={0},max_iterations={1},init_img_type={2},content_weight={3},style_weight={4},tv_weight={5}, color_convert_type={8}, optimizer={10}, model={12}".format(
#     args.max_size, args.max_iterations, args.init_img_type, args.content_weight, args.style_weight, args.tv_weight, args.temporal_weight, args.content_loss_function, args.color_convert_type, args.pooling_type, args.optimizer, args.learning_rate, args.model_weights)

  comment = "max_size={max_size},max_iterations={iterations},init_img_type={init_method},content_weight={content_weight},style_weight={style_weight},tv_weight={tv_weight}, optimizer={optimizer}, model={model}".format(**config)

  exif["0th"][piexif.ImageIFD.XPAuthor] = bytearray("Sebastian Betzin".encode("utf-16-le"))
  exif["0th"][piexif.ImageIFD.XPKeywords] = bytearray(keywords.encode("utf-16-le"))
  exif["0th"][piexif.ImageIFD.XPTitle] = bytearray(content_name.encode("utf-16-le"))
  exif["0th"][piexif.ImageIFD.XPSubject] = bytearray(style_name.encode("utf-16-le"))
  exif["0th"][piexif.ImageIFD.XPComment] = bytearray(comment.encode("utf-16-le"))

  exif_bytes = piexif.dump(exif)
  piexif.insert(exif_bytes, target_file)