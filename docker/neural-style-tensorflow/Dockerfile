FROM tensorflow/tensorflow:latest-gpu
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.1"

# remove python 2.7 and 3.5 from native installations
RUN apt-get update && apt purge python2.7-minimal -y && apt purge python3-minimal -y && apt autoremove -y

# update basic os
RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo

# dependecies for python libs (ssl)
RUN apt-get -y install libffi-dev libssl-dev libxml2-dev libxslt1-dev libjpeg8-dev zlib1g-dev 

# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-latest-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
ENV PATH /root/miniconda/bin:$PATH

#install python libs
WORKDIR /env/
COPY ["/env/environment.yaml" ,"/env/"]
RUN conda env update --file environment.yaml

# download loss network
WORKDIR /app/
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/pretrained-models/imagenet-vgg-verydeep-19.mat

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["/root/miniconda/bin/python", "/app/queueclient.py"]
