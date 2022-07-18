FROM nvidia/cuda:11.0.3-cudnn8-runtime-ubuntu18.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0.0"

# update basic os
RUN apt-get update && apt-get install -y nano wget git sudo

RUN sudo apt install -y g++
RUN sudo apt install -y build-essential

# dependecies for python libs (ssl)
# RUN apt-get -y install libffi-dev libssl-dev libxml2-dev libxslt1-dev libjpeg8-dev zlib1g-dev 

# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-py37_4.12.0-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
ENV PATH /root/miniconda/bin:$PATH
