LOCAL_IMAGE=sbetzin/h8-creation

.PHONY: build clean

build:
	docker build -t $(LOCAL_IMAGE) .

clean:
	-docker rmi -f $(LOCAL_IMAGE) || true