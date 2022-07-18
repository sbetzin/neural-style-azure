FROM nvidia/cuda:11.0.3-cudnn8-runtime-ubuntu18.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0.0"

# https://hub.docker.com/r/nvidia/cuda/tags?page=1&name=11.0

# update basic os
RUN apt-get update && apt-get install -y nano wget git sudo

# https://linuxconfig.org/how-to-install-g-the-c-compiler-on-ubuntu-18-04-bionic-beaver-linux
RUN sudo apt install -y g++
RUN sudo apt install -y build-essential

# dependecies for python libs (ssl)
# RUN apt-get -y install libffi-dev libssl-dev libxml2-dev libxslt1-dev libjpeg8-dev zlib1g-dev 

# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-py37_4.12.0-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
ENV PATH /root/miniconda/bin:$PATH

# Clone Repo
RUN git clone https://github.com/fmu2/3d_photo_stylization.git

# Install python libs
RUN conda install -y -c pytorch pytorch==1.7.1 torchvision==0.8.2 cudatoolkit=11.0
RUN conda install -y -c conda-forge tensorboard tensorboardx
RUN conda install -y -c anaconda scipy pyyaml
RUN conda install -y -c conda-forge ffmpeg imageio imageio-ffmpeg
RUN conda install -y -c plyfile matplotlib
RUN conda install -y -c open3d-admin open3d


