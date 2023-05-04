.PHONY: build rebuild start start-two stop restart logs start-console stop-console clear-logs
IMGNAME=sbetzin/neural-style-transfer
IMGTAG=1.0.0
	
build:
	nvidia-docker build -f Dockerfile -t $(IMGNAME):$(IMGTAG) .

start:
	nvidia-docker run -d -e AzureStorageConnectionString --name neural-style-transfer --restart=unless-stopped $(IMGNAME):$(IMGTAG)

start-two:
	nvidia-docker run -d --gpus '"device=1"' -e AzureStorageConnectionString --name neural-style-transfer-1 --restart=unless-stopped $(IMGNAME):$(IMGTAG)
	nvidia-docker run -d --gpus '"device=2"' -e AzureStorageConnectionString --name neural-style-transfer-2 --restart=unless-stopped $(IMGNAME):$(IMGTAG)
	
stop:
	nvidia-docker stop $(shell docker ps -q --filter "ancestor=$(IMGNAME):$(IMGTAG)")
	docker container prune --force

logs:
	nvidia-docker logs $(shell docker ps -q --filter "ancestor=$(IMGNAME):$(IMGTAG)") -f

start-console:
	nvidia-docker run --rm -it -e AzureStorageConnectionString --name neural-style-transfer-console --entrypoint /bin/bash $(IMGNAME):$(IMGTAG)

stop-console:
	sudo nvidia-docker stop $(shell docker ps -q --filter "name$=(IMGNAME)-console")
	sudo docker container prune --force

git-pull:
	git pull

rebuild: stop git-pull build start
restart: stop start

clear-logs:
	truncate -s 0 /var/lib/docker/containers/**/*-json.log