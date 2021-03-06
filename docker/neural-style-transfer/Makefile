.PHONY: all build build-1.15.0 clean start stop restart logs pull console-start console-stop console-logs
IMGNAME=sbetzin/neural-style-transfer
IMGTAG=latest

build:
	nvidia-docker build -t $(IMGNAME):$(IMGTAG) .
	
clean:
	docker rmi $(IMGNAME):$(IMGTAG)

start:
	nvidia-docker run -d -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-transfer --restart=unless-stopped $(IMGNAME)
	
stop:
	nvidia-docker stop $(shell docker ps -q --filter "name=neural-style-transfer")
	docker container prune --force

pull:
	docker pull $(IMGNAME):$(IMGTAG)
	
logs:
	nvidia-docker logs $(shell docker ps -q --filter "name=neural-style-transfer")
	
console-start:
	nvidia-docker run --rm -it -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-transfer-console --entrypoint /bin/bash $(IMGNAME)

console-stop:
	nvidia-docker stop $(shell docker ps -q --filter "name=neural-style-transfer-console")
	docker container prune --force
	
console-logs:
	nvidia-docker logs $(shell docker ps -q --filter "name=neural-style-transfer-console")

console-restart: console-stop console-start

restart: stop start
all: build
