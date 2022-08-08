FROM nvidia/cuda:11.2.0-cudnn8-devel-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0.0"

# https://hub.docker.com/r/nvidia/cuda/tags?page=1&name=11.0

# update basic os
RUN apt-get update && apt-get install -y nano wget git sudo

# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-py37_4.12.0-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
ENV PATH /root/miniconda/bin:$PATH

# https://linuxconfig.org/how-to-install-g-the-c-compiler-on-ubuntu-18-04-bionic-beaver-linux
RUN pip3 install torch==1.4.0+cu100 torchvision==0.5.0+cu100 -f https://download.pytorch.org/whl/torch_stable.html
RUN pip3 install opencv-python==4.2.0.32
RUN pip3 install vispy==0.6.4
RUN pip3 install moviepy==1.0.2
RUN pip3 install transforms3d==0.3.1
RUN pip3 install networkx==2.3
RUN sudo apt install sed



# Clone Repo
RUN git clone https://github.com/sbetzin/3d-photo-inpainting

# Install python libs
# RUN conda install -y -c pytorch pytorch==1.7.1 torchvision==0.8.2 cudatoolkit=11.0
# RUN conda install -y -c anaconda scipy pyyaml
# RUN conda install -y -c conda-forge tensorboard tensorboardx ffmpeg imageio imageio-ffmpeg plyfile matplotlib
# RUN conda install -y -c open3d-admin open3d




