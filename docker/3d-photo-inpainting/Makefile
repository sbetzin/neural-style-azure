.PHONY: all build rebuild clean start stop restart logs pull console-start console-stop console-logs prune
IMGNAME=sbetzin/3d-photo-inpainting
IMGTAG=1.0.0
	
build:
	nvidia-docker build -f Dockerfile -t $(IMGNAME):$(IMGTAG) .

clean:
	docker rmi $(IMGNAME):$(IMGTAG)

start:
	nvidia-docker run -d -e AzureStorageConnectionString -e AzureStorageQueueName --name $(IMGNAME) --restart=unless-stopped $(IMGNAME):$(IMGTAG)

stop:
	nvidia-docker stop $(shell docker ps -q --filter "name=$(IMGNAME)")
	docker container prune --force

pull:
	docker pull $(IMGNAME):$(IMGTAG)
	
logs:
	nvidia-docker logs $(shell docker ps -q --filter "name=$(IMGNAME)")

prune:
	docker container prune --force
	
console-start:
	nvidia-docker run --rm -it -e AzureStorageConnectionString -e AzureStorageQueueName --name $(IMGNAME) --entrypoint /bin/bash $(IMGNAME):$(IMGTAG)

console-stop:
	nvidia-docker stop $(shell docker ps -q --filter "name$=(IMGNAME)-console")
	docker container prune --force
	
console-logs:
	nvidia-docker logs $(shell docker ps -q --filter "name=$(IMGNAME)-console")

console-restart: console-stop console-start

rebuild: stop build start
restart: stop start
all: build
