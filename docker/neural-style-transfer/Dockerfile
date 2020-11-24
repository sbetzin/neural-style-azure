FROM nvidia/cuda:11.1-runtime-ubuntu18.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# update basic os
RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo

# dependecies for python libs (ssl)
RUN apt-get install -y libsm6 libxext6 libxrender-dev

