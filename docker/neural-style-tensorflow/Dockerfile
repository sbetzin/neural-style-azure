FROM tensorflow/tensorflow:latest-gpu
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"

# prepare basic os and python libs
RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo
RUN pip install --upgrade pip && pip install opencv-python-headless scipy azure-storage-queue azure-storage-blob piexif

# download loss network
# pretrained networks from http://www.vlfeat.org/matconvnet/pretrained/
WORKDIR /app/
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-vgg-verydeep-19.mat
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-vgg-verydeep-16.mat
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-resnet-101-dag.mat
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-resnet-152-dag.mat
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-resnet-50-dag.mat
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-vgg-f.mat
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-vgg-m-1024.mat
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-vgg-m-128.mat
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-vgg-m-2048.mat
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-vgg-m.mat
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-vgg-s.mat


# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["python", "/app/queueclient.py"]
