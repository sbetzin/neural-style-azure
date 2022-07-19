# https://hub.docker.com/r/nvidia/cuda/tags?page=1&name=11.0
FROM nvidia/cuda:11.0.3-runtime-ubuntu18.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0.0"

# update basic os
RUN apt-get update && apt-get install -y nano wget git sudo

# Install miniconda with python 3.6
# export PATH="~/miniconda/bin:$PATH"
RUN wget --progress=bar:force https://repo.anaconda.com/miniconda/Miniconda3-4.3.11-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
ENV PATH /root/miniconda/bin:$PATH

# Clone Repo
RUN git clone https://github.com/fmu2/3d_photo_stylization.git

# Install python libs
RUN conda install -y -c pytorch pytorch==1.7.1 torchvision==0.8.2
RUN conda install -y tqdm=4.46.0 tifffile=2020.10.1 aiohttp=3.7.4 ipython-7.16.1



