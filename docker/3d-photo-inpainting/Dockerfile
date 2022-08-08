FROM nvidia/cuda:11.2.0-cudnn8-devel-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0.0"

# https://hub.docker.com/r/nvidia/cuda/tags?page=1&name=11.0

# update basic os
RUN apt-get update && apt-get install -y nano wget git sudo sed

# Install Timezone Silent
# https://serverfault.com/questions/949991/how-to-install-tzdata-on-a-ubuntu-docker-image
RUN DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get -y install tzdata

# added needed os basics for 
RUN apt-get install -y pkg-config libfontconfig1-dev libsm6 libxext6 libxrender-dev

# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-py37_4.12.0-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
ENV PATH /root/miniconda/bin:$PATH

COPY ["/env/environment.yml", "/env/"]
RUN chmod 777 /env/

WORKDIR /env/
RUN conda env update --file environment.yml

WORKDIR /home/
# Clone Repo
RUN git clone https://github.com/sbetzin/3d-photo-inpainting
