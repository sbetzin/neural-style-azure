.PHONY: all build rebuild clean start stop restart logs pull console-start console-stop console-logs
IMGNAME=sbetzin/neural-style-tensorflow
IMGTAG=1.15.0-gpu-py3
	
build:
	nvidia-docker build -f Dockerfile-1.15.0-gpu-py3 -t $(IMGNAME):$(IMGTAG) .
	#nvidia-docker push sbetzin/neural-style-tensorflow:1.15.0-gpu-py3

clean:
	docker rmi $(IMGNAME):$(IMGTAG)

start:
	# nvidia-docker run -d -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow --restart=unless-stopped $(IMGNAME):$(IMGTAG)
	nvidia-docker run -d --gpus '"device=1"' -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow-0 --restart=unless-stopped $(IMGNAME):$(IMGTAG) 
	nvidia-docker run -d --gpus '"device=2"' -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow-1 --restart=unless-stopped $(IMGNAME):$(IMGTAG) 
	# nvidia-docker run -d --gpus '"device=2"' -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow-2 --restart=unless-stopped $(IMGNAME):$(IMGTAG)
	# nvidia-docker run -d --gpus '"device=3"' -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow-3 --restart=unless-stopped $(IMGNAME):$(IMGTAG) 
	# nvidia-docker run -d --gpus '"device=4"' -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow-4 --restart=unless-stopped $(IMGNAME):$(IMGTAG)
	# nvidia-docker run -d --gpus '"device=5"' -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow-5 --restart=unless-stopped $(IMGNAME):$(IMGTAG) 
	# nvidia-docker run -d --gpus '"device=6"' -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow-6 --restart=unless-stopped $(IMGNAME):$(IMGTAG) 
	# nvidia-docker run -d --gpus '"device=7"' -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-tensorflow-7 --restart=unless-stopped $(IMGNAME):$(IMGTAG) 

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

rebuild: stop build start
restart: stop start
all: build
