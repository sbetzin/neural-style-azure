.PHONY: all build clean start stop restart logs pull
IMGNAME=sbetzin/neural-style-tensorflow
IMGTAG=latest

build:
	nvidia-docker build -t $(IMGNAME):$(IMGTAG) .

clean:
	docker rmi $(IMGNAME):$(IMGTAG)

start:
	nvidia-docker run -d -e AzureStorageConnectionString --name neural-style-python --restart=unless-stopped $(IMGNAME)
	
stop:
	nvidia-docker stop $(shell docker ps -q --filter ancestor=sbetzin/neural-style-tensorflow)
	docker container prune --force

pull:
	docker pull $(IMGNAME):$(IMGTAG)
	
logs:
	nvidia-docker logs $(shell docker ps -q --filter ancestor=sbetzin/neural-style-tensorflow)
	
restart: stop start
all: build
