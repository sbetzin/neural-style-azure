.PHONY: all build clean start stop restart
IMGNAME=sbetzin/neural-style-python
IMGTAG=latest

build:
	nvidia-docker build -t $(IMGNAME):$(IMGTAG) .

clean:
	docker rmi $(IMGNAME):$(IMGTAG)

start:
	nvidia-docker run -d -e AzureStorageConnectionString --name neural-style-python --restart=unless-stopped $(IMGNAME)
	
stop:
	echo $(IMGNAME)
	id = docker ps -q --filter ancestor=$(IMGNAME)
	echo $(id)
	nvidia-docker stop $(docker ps -q --filter ancestor=$(id) )

restart: stop start
all: build
