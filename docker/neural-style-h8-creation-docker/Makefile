LOCAL_IMAGE=sbetzin/neural-style-h8-creation

.PHONY: build clean

build:
	docker build -t $(LOCAL_IMAGE) .

clean:
	-docker rmi -f $(LOCAL_IMAGE) || true