.PHONY: all build clean
IMGNAME=sbetzin/neural-style-torch
IMGTAG=latest

build:
	nvidia-docker build -t $(IMGNAME):$(IMGTAG) .

clean:
	docker rmi $(IMGNAME):$(IMGTAG)

all: build
