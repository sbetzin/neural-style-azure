.PHONY: all build build-nocache rebuild clean start stop restart logs pull console-start console-stop console-logs prune start-cog
IMGAUTHOR=sbetzin
IMGNAME=frame-interpolation
IMGTAG=1.0.0
	
build:
	nvidia-docker build -f Dockerfile -t $(IMGAUTHOR)/$(IMGNAME):$(IMGTAG) .

build-nocache:
	nvidia-docker build -f Dockerfile --no-cache -t $(IMGAUTHOR)/$(IMGNAME):$(IMGTAG) .

clean:
	docker rmi $(IMGAUTHOR)/$(IMGNAME):$(IMGTAG)

start:
	nvidia-docker run -d --gpus '"device=1"' -e AzureStorageConnectionString -e AzureStorageQueueName --name $(IMGNAME) -v /root/OneDrive/_nft:/nft --restart=unless-stopped $(IMGAUTHOR)/$(IMGNAME):$(IMGTAG)

stop:
	nvidia-docker stop $(shell docker ps -q --filter "name=$(IMGNAME)")
	docker container prune --force

pull:
	docker pull $(IMGNAME):$(IMGTAG)
	
logs:
	nvidia-docker logs $(shell docker ps -q --filter "name=$(IMGNAME)")

prune:
	docker container prune --force
	
start-console:
	nvidia-docker run --rm --gpus '"device=1"' -it -e AzureStorageConnectionString -e AzureStorageQueueName --name $(IMGNAME) -v /root/OneDrive/_nft:/nft --entrypoint /bin/bash $(IMGAUTHOR)/$(IMGNAME):$(IMGTAG)

start-cog:
	git pull
	sudo bash -c "cd src && cog build -t test"
	sudo nvidia-docker run --rm --gpus '"device=1"' --name "test" -v /root/OneDrive/_nft:/nft -p 5000:5000 test 

stop-console:
	nvidia-docker stop $(shell docker ps -q --filter "name$=(IMGNAME)-console")
	docker container prune --force
	
logs-console:
	nvidia-docker logs $(shell docker ps -q --filter "name=$(IMGNAME)-console")

restart-console: console-stop console-start

rebuild: stop build start
restart: stop start
all: build
