FROM tensorflow/tensorflow:latest-gpu
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"

# remove python 2.7
RUN apt purge python2.7-minimal -y && apt autoremove -y

# update basic os
RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo python3-pip

# dependecies for python libs (ssl)
RUN apt-get -y install libffi-dev libssl-dev libxml2-dev libxslt1-dev libjpeg8-dev zlib1g-dev 

#install python libs
RUN pip3 tensorflow install opencv-python-headless scipy azure-storage-queue azure-storage-blob applicationinsights piexif

# download loss network
WORKDIR /app/
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-vgg-verydeep-19.mat

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["python3", "/app/queueclient.py"]
