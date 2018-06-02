# ad-hoc user to make it possible to run from devconf user
USER=jkremser
LOCAL_IMAGE=$(USER)/fast-neural-style
TRAIN_IMAGE=$(USER)/fast-neural-style:training
YOLO_IMAGE=$(USER)/fast-neural-style:yolo
SSH_PARAMS=-o UserKnownHostsFile=/dev/null -o StrictHostKeyChecking=no root@`docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' fns`
SSH_RUN_COMMAND=ssh -X $(SSH_PARAMS)
SRC_DIR=$(HOME)/workspace/fast-neural-style/data

.PHONY: build clean run run-cont run-for-training run-cont-mount attach copy-id webcam1 webcam2 webcam3 prepare-training-env train training-all

build:
	docker build -t $(LOCAL_IMAGE) .

clean:
	-docker rmi -f $(LOCAL_IMAGE) || true

run-cont:
	-docker rm -f fns || true
	nvidia-docker run --rm -d --name fns --device=/dev/video0 $(LOCAL_IMAGE)
	sleep 1

run-cont-mount:
	-docker rm -f fns || true
	mkdir -p $(SRC_DIR)/{art,models}
	nvidia-docker run --rm -d --name fns -v $(SRC_DIR):/tmp/data --device=/dev/video0 $(LOCAL_IMAGE)
	sleep 1

attach:
	docker exec -ti fns /bin/bash

copy-id:
	echo -e "\n\nPassword: p\n\n"
	ssh-copy-id $(SSH_PARAMS)

webcam0:
	$(SSH_RUN_COMMAND) /webcam.sh -models models/instance_norm/the_scream.t7

webcam1:
	$(SSH_RUN_COMMAND) /webcam.sh

webcam2:
	$(SSH_RUN_COMMAND) /webcam.sh -models models/eccv16/starry_night.t7,models/instance_norm/udnie.t7

webcam3:
	$(SSH_RUN_COMMAND) /webcam.sh -models models/instance_norm/udnie.t7,models/instance_norm/la_muse.t7,models/instance_norm/mosaic.t7,models/instance_norm/the_scream.t7

prepare-training-env:
	$(SSH_RUN_COMMAND) apt-get update
	$(SSH_RUN_COMMAND) apt-get -y install python2.7-dev libhdf5-dev
	$(SSH_RUN_COMMAND) luarocks install hdf5
	$(SSH_RUN_COMMAND) wget -O /root/torch/fast-neural-style/models/vgg16.t7 https://cs.stanford.edu/people/jcjohns/fast-neural-style/models/vgg16.t7
	$(SSH_RUN_COMMAND) pip install -r /root/torch/fast-neural-style/requirements.txt
	docker cp common.sh fns:/
	echo -e "\n\n\n Docker image has been prepared for training \n\n Run \`make attach\`, `source /common.sh` and start the training for example with command:\n\nth train.lua \\ \n   -style_image_size 384 \\ \n   -content_weights 1.0 \\ \n   -style_weights 8.0 \\ \n   -checkpoint_name checkpoint \\ \n   -gpu 0 \\ \n   -h5_file /tmp/data/dataset17.h5 \\ \n   -style_image /tmp/data/art/division.jpg\n\n\n"

train:
	-docker rm -f fns || true
	nvidia-docker run --rm -d --name fns -v $(SRC_DIR):/tmp/data --device=/dev/video0 $(TRAIN_IMAGE)
	sleep 1
	docker cp trainer.sh fns:/
	$(SSH_RUN_COMMAND) /trainer.sh

train-using-image:
	docker cp trainer.sh fns:/
	$(SSH_RUN_COMMAND) /trainer.sh

run: run-cont copy-id webcam1

run-for-training: run-cont-mount copy-id prepare-training-env

training-all: run-for-training train

yolo:
	-docker rm -f fns || true
	nvidia-docker run --rm -d --name fns -v $(SRC_DIR):/tmp/data --device=/dev/video0 $(YOLO_IMAGE)
	$(SSH_RUN_COMMAND) /webcam.sh
