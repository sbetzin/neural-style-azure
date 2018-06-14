.PHONY: all build clean start stop restart logs pull console
IMGNAME=sbetzin/neural-style-tensorflow
IMGTAG=latest

build:
	nvidia-docker build -t $(IMGNAME):$(IMGTAG) .

clean:
	docker rmi $(IMGNAME):$(IMGTAG)

start:
	nvidia-docker run --rm -d -e AzureStorageConnectionString --name neural-style-tensorflow --restart=unless-stopped $(IMGNAME)
	
stop:
	nvidia-docker stop $(shell docker ps -q --filter ancestor=sbetzin/neural-style-tensorflow)
	docker container prune --force

pull:
	docker pull $(IMGNAME):$(IMGTAG)
	
logs:
	nvidia-docker logs $(shell docker ps -q --filter ancestor=sbetzin/neural-style-tensorflow)
	
console:
	nvidia-docker run --rm -it AzureStorageConnectionString --name neural-style-tensorflow-console --entrypoint /bin/bash $(IMGNAME)
	
restart: stop start
all: build
