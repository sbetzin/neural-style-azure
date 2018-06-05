LOCAL_IMAGE=sbetzin/fast-neural-style

.PHONY: build clean

build:
	docker build -t $(LOCAL_IMAGE) .

clean:
	-docker rmi -f $(LOCAL_IMAGE) || true