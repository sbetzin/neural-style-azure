FROM tensorflow/tensorflow:latest-gpu
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo
RUN pip install --upgrade pip && pip install opencv-python-headless

WORKDIR /app
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/imagenet-vgg-verydeep-19.mat

COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["python", "/app/queueclient.py"]