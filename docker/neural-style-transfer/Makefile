.PHONY: build rebuild clean start start-two stop restart logs pull console-start console-stop console-logs prune clear-logs
IMGNAME=sbetzin/neural-style-transfer
IMGTAG=1.0.0
	
build:
	nvidia-docker build -f Dockerfile -t $(IMGNAME):$(IMGTAG) .

clean:
	docker rmi $(IMGNAME):$(IMGTAG)

start:
	nvidia-docker run -d -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-transfer --restart=unless-stopped $(IMGNAME):$(IMGTAG)


start-two:
	nvidia-docker run -d --gpus '"device=1"' -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-transfer-1 --restart=unless-stopped $(IMGNAME):$(IMGTAG)
	nvidia-docker run -d --gpus '"device=2"' -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-transfer-2 --restart=unless-stopped $(IMGNAME):$(IMGTAG)
	

stop:
	nvidia-docker stop $(shell docker ps -q --filter "ancestor=$(IMGNAME):$(IMGTAG)")
	docker container prune --force

pull:
	docker pull $(IMGNAME):$(IMGTAG)
	
logs:
	nvidia-docker logs $(shell docker ps -q --filter "ancestor=$(IMGNAME):$(IMGTAG)") -f

prune:
	docker container prune --force
	
console-start:
	nvidia-docker run --rm -it -e AzureStorageConnectionString -e AzureStorageQueueName --name neural-style-transfer-console --entrypoint /bin/bash $(IMGNAME):$(IMGTAG)

console-stop:
	nvidia-docker stop $(shell docker ps -q --filter "name=neural-style-transfer-console")
	docker container prune --force
	
console-logs:
	nvidia-docker logs $(shell docker ps -q --filter "name=neural-style-transfer-console")

console-restart: console-stop console-start

rebuild: stop build start
restart: stop start

clear-logs:
	truncate -s 0 /var/lib/docker/containers/**/*-json.log