FROM tensorflow/tensorflow:latest-gpu
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo
RUN pip install --upgrade pip && pip install opencv-python-headless wheel azure-storage-queue azure-storage-blob blobxfer piexif

RUN git clone https://github.com/brade31919/SRGAN-tensorflow.git

WORKDIR /app
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/vgg19.t7

RUN mkdir /app/images

COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["python", "/app/queueclient.py"]