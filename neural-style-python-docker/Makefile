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

	TEST=100
	echo testecho
	echo "$(TEST)"
	@ID=$$(docker ps -q --filter ancestor=$(IMGNAME))
	echo $(ID)
	nvidia-docker stop $(ID)
	docker container prune --force

restart: stop start
all: build
