FROM nvidia/cudagl:11.1.1-runtime-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0.1"
USER root



# update basic os
RUN apt-get update && apt-get upgrade -y && apt-get install -y nano wget git sudo libglib2.0-0 libsm6 libxrender1 libxext6 fontconfig

#Install Python and Add python alias for python3
RUN apt-get install python3 pip -y && update-alternatives --install /usr/bin/python python /usr/bin/python3 1

#Install packages
RUN pip install torch scipy pyyaml matplotlib scikit-image torchvision opencv-python vispy moviepy transforms3d networkx piexif azure-storage-queue azure-storage-blob applicationinsights
RUN pip install numpy==1.22.2

# Install Timezone Silent
# https://serverfault.com/questions/949991/how-to-install-tzdata-on-a-ubuntu-docker-image

#RUN DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get -y install tzdata

# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
#RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-py37_4.12.0-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
#ENV PATH /root/miniconda/bin:$PATH

#COPY ["/env/environment.yml", "/env/"]
#RUN chmod 777 /env/

#WORKDIR /env/
#RUN conda env update --file environment.yml

WORKDIR /home/
# Clone Repo
RUN git clone https://github.com/sbetzin/3d-photo-inpainting.git
WORKDIR /home/3d-photo-inpainting
RUN mkdir checkpoints

# Clone BoostingMonocularDepth Repo
RUN git clone https://github.com/sbetzin/BoostingMonocularDepth.git
RUN mkdir -p BoostingMonocularDepth/pix2pix/checkpoints/mergemodel/

# Download models
WORKDIR /home/3d-photo-inpainting/checkpoints
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/color-model.pth
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/depth-model.pth
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/edge-model.pth

WORKDIR /home/3d-photo-inpainting/BoostingMonocularDepth/pix2pix/checkpoints/mergemodel/
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/latest_net_G.pth

WORKDIR /home/3d-photo-inpainting/BoostingMonocularDepth/midas
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/model.pt

WORKDIR /home/3d-photo-inpainting/BoostingMonocularDepth
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/res101.pth

#Testimage
WORKDIR /home
RUN mkdir -p image
WORKDIR /home/image
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/images/close_up_01.jpg

WORKDIR /home/3d-photo-inpainting
