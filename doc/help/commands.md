# Docker
## Hilfreiche Standard Commands
### um alle laufenden Docker anzuzeigen
```bash
sudo docker ps
```
### um die Logs einens Containers fortlaufend anzuzeigen
https://docs.docker.com/engine/reference/commandline/logs/#extended-description
```bash
sudo docker logs 66b6383bfa41 -f
```
### um alle möglichen Caches zu löschen
```bash
sudo docker system prune -a
```
### Teste das Nvidia-smi in einem Docker Container
```bash
sudo nvidia-docker run --rm nvidia/cuda:latest nvidia-smi
```

### Baue den Docker container
für mehr Hilfe der Make Befehle -> direkt im Make File bauen
```bash
sudo make build
```


### Run docker console with mapped folder
```bash
sudo nvidia-docker run --rm -it -e AzureStorageConnectionString -v /datadrive/training:/training --entrypoint /bin/bash sbetzin/neural-style
```
### Run python docker console
```bash
sudo nvidia-docker run --rm -it -e AzureStorageConnectionString -v /datadrive/training:/training --entrypoint /bin/bash sbetzin/neural-style-python
```

### Run the train lua script with mapped network in the background
```bash
sudo nvidia-docker run --rm -v /datadrive/training:/training --entrypoint /bin/bash sbetzin/neural-style -c 'cd /app/fast-neural-style/ && th train.lua -h5_file /training/coco2014.h5 -style_image /training/style/expressionismus.jpg -content_weights 5 -style_weights 1000 -style_image_size 512 -loss_network /app/fast-neural-style/vgg16.t7 -checkpoint_name /training/models/style/expressionismus_vgg16_cw_5_sw_1000_size_512 -checkpoint_every 500' &
```
### Run the fast neural style on all in images
```bash
sudo nvidia-docker run --rm -v /datadrive/training:/training --entrypoint /bin/bash sbetzin/neural-style -c 'cd /app/fast-neural-style/ && th fast_neural_style.lua -model /training/models/style/expressionismus_vgg16_cw_0.1_sw_10_size_512 -input_dir /training/in/ -output_dir /training/out/ -image_size 0 -gpu 0'
```
### default startup docker image
```bash
sudo nvidia-docker run --rm  -it  sbetzin/neural-style
```
### run docker with environment vars
```bash
sudo nvidia-docker run -it -e AzureStorageConnectionString -e TileSize --name neural-style sbetzin/neural-style

sudo nvidia-docker run --rm -it -v /datadrive/training:/training sbetzin/neural-style-python --content_img /training/in/eric_pool.JPG --style_imgs /training/style/kandinsky_schwarz-und-violett.jpg --max_size 1200 --max_iterations 500 --content_weight 1 --style_weight 10000 --original_colors --verbose --pooling_type max --img_output_dir /training/out --img_name /training/out/eric_pool_kandinsky_schwarz-und-violett_cw_1_sw_10000_iter_500_size_1200_pooling_max
```

### Online help for bash syntax check
https://www.shellcheck.net/


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
# Markdown
[Markdown Cheat Cheat](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet)