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
<<<<<<< HEAD
	id = docker ps -q --filter ancestor=$(IMGNAME)
	echo $(id)
	nvidia-docker stop $(docker ps -q --filter ancestor=$(id) )
=======
	ID=docker ps -q --filter ancestor=$(IMGNAME)
	echo $(ID)
	nvidia-docker stop $(docker ps -q --filter ancestor=$(ID) )
>>>>>>> 21e52aa522d0ee7738623a9e0a001907ac0983e1

restart: stop start
all: build
