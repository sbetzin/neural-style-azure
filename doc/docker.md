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
### Stope, Baue und Starte den container
```bash
sudo make stop ; sudo make ; sudo make start
```

### Run docker console with mapped folder
```bash
sudo nvidia-docker run --rm -it -v /:/images --entrypoint /bin/bash sbetzin/neural-style-tensorflow
```
### Append Command Line Args to the entrypoint
```bash
sudo nvidia-docker run --rm -it -e AzureStorageConnectionString --entrypoint "python"  tensorflow/tensorflow:1.12.0-gpu-py3  --version
```
### Run the train lua script with mapped network in the background
```bash
sudo nvidia-docker run --rm -v /datadrive/training:/training --entrypoint /bin/bash sbetzin/neural-style-tensorflow -c 'cd /app/fast-neural-style/ && th train.lua -h5_file /training/coco2014.h5 -style_image /training/style/expressionismus.jpg -content_weights 5 -style_weights 1000 -style_image_size 512 -loss_network /app/fast-neural-style/vgg16.t7 -checkpoint_name /training/models/style/expressionismus_vgg16_cw_5_sw_1000_size_512 -checkpoint_every 500' &
```
### Run the fast neural style on all in images
```bash
sudo nvidia-docker run --rm -v /datadrive/training:/training --entrypoint /bin/bash sbetzin/neural-style-tensorflow -c 'cd /app/fast-neural-style/ && th fast_neural_style.lua -model /training/models/style/expressionismus_vgg16_cw_0.1_sw_10_size_512 -input_dir /training/in/ -output_dir /training/out/ -image_size 0 -gpu 0'
```
### default startup docker image
```bash
sudo nvidia-docker run --rm  -it sbetzin/neural-style-tensorflow
```
### run docker with environment vars
```bash
sudo nvidia-docker run -it -e AzureStorageConnectionString -e TileSize --name neural-style sbetzin/neural-style-tensorflow
```
### docker infos (esp. path to docker images)
```bash
sudo docker info
```
### move docker image directory to other directory
https://docs.docker.com/config/daemon/#docker-daemon-directory

### push image to docker hub
```bash
sudo docker login -u uuu -p ppp
sudo docker push sbetzin/neural-style-tensorflow:1.12.0-gpu-py3
```
https://docs.docker.com/v17.12/docker-cloud/builds/push-images/










