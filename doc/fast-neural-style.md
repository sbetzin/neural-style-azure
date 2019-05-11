# Lua Training
### train a new style
```bash
th train.lua -h5_file /training/coco2014.h5 -style_image /training/style/expressionismus.jpg -content_weights 10 -style_weights 10 -loss_network vgg16.t7 -checkpoint_name /training/model/expressionismus_cw_10_sw_10 -checkpoint_every 500
```
### create fast neural style on a single image
```bash
th fast_neural_style.lua -model /training/model/expressionismus_cw_10_sw_50.t7 -input_image /training/in/Ana.jpg -output_image /training/out/Ana.jpg -image_size 0 -timing 0 -median_filter 0 -gpu 0 
```
### create fast neural style on a folder
```bash
th fast_neural_style.lua -model /training/model/expressionismus_vgg16_cw_0.1_sw_1_size_512.t7 -input_dir /training/in/ -output_dir /training/out/ -image_size 0 -timing 0 -median_filter 0 -gpu 0
```

# Für den KELLER
```bash
sudo nvidia-docker run --rm -it -v ~/images:/training sbetzin/neural-style-python --content_img /training/in/e_r_pool.JPG --style_imgs /training/style/comic_style_1.jpg --max_size 1200 --max_iterations 500 --img_output_dir /training/out --content_weight 1 --style_weight 10000 --original_colors --verbose --pooling_type max --img_name /training/out/e_r_pool_comic_style_1_cw_1_sw_10000_pooling_max

sudo nohup bash batch.sh &
```