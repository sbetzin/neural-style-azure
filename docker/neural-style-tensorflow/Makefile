.PHONY: all build rebuild clean start stop restart logs pull console-start console-stop console-logs
IMGNAME=sbetzin/neural-style-tensorflow
IMGTAG=1.15.0-gpu-py3
	
build:
	nvidia-docker build -f Dockerfile-1.15.0-gpu-py3 -t $(IMGNAME):$(IMGTAG) .
	#nvidia-docker push sbetzin/neural-style-tensorflow:1.15.0-gpu-py3

rebuild:
	stop build start

clean:
	docker rmi $(IMGNAME):$(IMGTAG)

start:
	nvidia-docker run -d -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow --restart=unless-stopped $(IMGNAME):$(IMGTAG)
	
stop:
	nvidia-docker stop $(shell docker ps -q --filter "name=neural-style-tensorflow")
	docker container prune --force

pull:
	docker pull $(IMGNAME):$(IMGTAG)
	
logs:
	nvidia-docker logs $(shell docker ps -q --filter "name=neural-style-tensorflow")
	
console-start:
	nvidia-docker run --rm -it -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow-console --entrypoint /bin/bash $(IMGNAME):$(IMGTAG)

console-stop:
	nvidia-docker stop $(shell docker ps -q --filter "name=neural-style-tensorflow-console")
	docker container prune --force
	
console-logs:
	nvidia-docker logs $(shell docker ps -q --filter "name=neural-style-tensorflow-console")

console-restart: console-stop console-start

restart: stop start
all: build
