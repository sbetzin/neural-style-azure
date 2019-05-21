FROM tensorflow/tensorflow:latest-gpu
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"

# remove python 2.7 and 3.5 from native installations
RUN apt purge python2.7-minimal -y && apt purge python3-minimal -y apt autoremove -y

# update basic os
RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo

# dependecies for python libs (ssl)
RUN apt-get -y install libffi-dev libssl-dev libxml2-dev libxslt1-dev libjpeg8-dev zlib1g-dev 

# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-latest-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p ~/miniconda && rm ~/miniconda.sh
ENV PATH ~/miniconda/bin:$PATH

#install python libs
RUN conda install -y pytest keras tqdm Pillow
RUN pip install tensorflow-gpu

# clone super-resolution 
WORKDIR /app/
RUN git clone https://github.com/krasserm/super-resolution

WORKDIR /app/super-resolution

RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/super-resolution/edsr-16-x4-gen-epoch-088.h5
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/super-resolution/wdsr-a-32-x4-psnr-29.1736.h5
