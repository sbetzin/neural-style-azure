.PHONY: build build-nocache rebuild start stop restart logs start-console stop-console
IMGAUTHOR=sbetzin
IMGNAME=3d-photo-inpainting
IMGTAG=1.0.1
	
build:
	sudo nvidia-docker build -f Dockerfile -t $(IMGAUTHOR)/$(IMGNAME):$(IMGTAG) .

build-nocache:
	sudo nvidia-docker build -f Dockerfile --no-cache -t $(IMGAUTHOR)/$(IMGNAME):$(IMGTAG) .

start:
	sudo nvidia-docker run -d --gpus '"device=1"' -e AzureStorageConnectionString --name $(IMGNAME) -v ~/depth:/depth -v ~/mesh:/mesh -v /root/OneDrive/_nft:/nft  --restart=unless-stopped $(IMGAUTHOR)/$(IMGNAME):$(IMGTAG)

stop:
	sudo nvidia-docker stop $(shell sudo docker ps -q --filter "name=$(IMGNAME)")
	sudo nvidia-docker container prune --force
	
logs:
	sudo nvidia-docker logs $(shell sudo docker ps -q --filter "name=$(IMGNAME)") -f

start-console:
	sudo nvidia-docker run --rm --gpus '"device=1"' -it -e AzureStorageConnectionString --name $(IMGNAME) -v ~/depth:/depth -v ~/mesh:/mesh -v /root/OneDrive/_nft:/nft --entrypoint /bin/bash $(IMGAUTHOR)/$(IMGNAME):$(IMGTAG)

stop-console:
	sudo nvidia-docker stop $(shell sudo docker ps -q --filter "name$=(IMGNAME)-console")
	sudo docker container prune --force

git-pull:
	git pull

rebuild: stop git-pull build start
restart: stop start
