FROM tensorflow/tensorflow:1.12.0-gpu-py3
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# prepare basic os and python libs
# RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo
RUN apt-get update && apt-get install -y nano wget git sudo
RUN pip install --upgrade pip && pip install opencv-python-headless scipy azure-storage-queue azure-storage-blob piexif

# download loss network
WORKDIR /app/
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/imagenet-vgg-verydeep-19.mat

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["python", "/app/queueclient.py"]