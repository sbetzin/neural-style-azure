.PHONY: build rebuild start start-two stop restart logs start-console stop-console test
NAME=mask-transfer
IMGNAME=sbetzin/$(NAME)
IMGTAG=1.0.0

test:
	echo $(NAME)
build:
	sudo nvidia-docker build -f Dockerfile -t $(IMGNAME):$(IMGTAG) .

start:
	sudo nvidia-docker run -d -e AzureStorageConnectionString -v /root/OneDrive/_nft:/nft --name >$(NAME) --restart=unless-stopped $(IMGNAME):$(IMGTAG)

stop:
	sudo nvidia-docker stop $(shell sudo docker ps -q --filter "ancestor=$(IMGNAME):$(IMGTAG)")
	sudo nvidia-docker container prune --force

logs:
	sudo nvidia-docker logs $(shell sudo docker ps -q --filter "ancestor=$(IMGNAME):$(IMGTAG)") -f

start-console:
	sudo nvidia-docker run --rm -it -e AzureStorageConnectionString -v /root/OneDrive/_nft:/nft --name $(NAME)-console --entrypoint /bin/bash $(IMGNAME):$(IMGTAG)

stop-console:
	sudo nvidia-docker stop $(shell sudo docker ps -q --filter "name=$(NAME)-console")
	sudo docker container prune --force

git-pull:
	git pull

rebuild: stop git-pull build start
restart: stop start
