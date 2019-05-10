FROM tensorflow/tensorflow:1.12.0-gpu-py3
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

# prepare basic os and python libs
RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo
RUN pip install --upgrade pip && pip install opencv-python-headless scipy wheel azure-storage-queue azure-storage-blob blobxfer piexif moviepy

# download loss network
WORKDIR /app/
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/imagenet-vgg-verydeep-19.mat

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["python", "/app/queueclient.py"]