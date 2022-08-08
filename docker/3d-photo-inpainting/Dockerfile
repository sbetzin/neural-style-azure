FROM nvidia/cuda:11.2.0-cudnn8-devel-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0.0"

# https://hub.docker.com/r/nvidia/cuda/tags?page=1&name=11.0

# update basic os
RUN apt-get update && apt-get install -y nano wget git sudo sed

# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-py37_4.12.0-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
ENV PATH /root/miniconda/bin:$PATH

COPY ["/env/environment.yml", "/env/"]
RUN chmod 777 /env/

WORKDIR /env/
RUN conda env update --file environment.yml

# Clone Repo
RUN git clone https://github.com/sbetzin/3d-photo-inpainting

# Install python libs
# RUN conda install -y -c pytorch pytorch==1.7.1 torchvision==0.8.2 cudatoolkit=11.0
# RUN conda install -y -c anaconda scipy pyyaml
# RUN conda install -y -c conda-forge tensorboard tensorboardx ffmpeg imageio imageio-ffmpeg plyfile matplotlib
# RUN conda install -y -c open3d-admin open3d




