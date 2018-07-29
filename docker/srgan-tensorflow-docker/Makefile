.PHONY: all build clean start stop restart logs pull console-start console-stop console-logs
IMGNAME=sbetzin/srgan-tensorflow
IMGTAG=latest

build:
	nvidia-docker build -t $(IMGNAME):$(IMGTAG) .

clean:
	docker rmi $(IMGNAME):$(IMGTAG)

start:
	nvidia-docker run -d --name srgan-tensorflow --restart=unless-stopped $(IMGNAME)
	
stop:
	nvidia-docker stop $(shell docker ps -q --filter "name=srgan-tensorflow")
	docker container prune --force

pull:
	docker pull $(IMGNAME):$(IMGTAG)
	
logs:
	nvidia-docker logs $(shell docker ps -q --filter "name=srgan-tensorflow")
	
console-start:
	nvidia-docker run --rm -it --name srgan-tensorflow-console -v /datadrive/training/enlarge:/images --entrypoint /bin/bash $(IMGNAME)

console-stop:
	nvidia-docker stop $(shell docker ps -q --filter "name=srgan-tensorflow-console")
	docker container prune --force
	
console-logs:
	nvidia-docker logs $(shell docker ps -q --filter "name=srgan-tensorflow-console")

console-restart: console-stop console-start

restart: stop start
all: build
