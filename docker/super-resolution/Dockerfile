FROM tensorflow/tensorflow:latest-gpu
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"

# prepare basic os and python libs
RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo zip
RUN apt-get -y install libffi-dev libssl-dev libxml2-dev libxslt1-dev libjpeg8-dev zlib1g-dev python3-pip

# Install miniconda
RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-latest-Linux-x86_64.sh -O ~/miniconda.sh 
RUN sh ~/miniconda.sh -b -p ~/miniconda 
RUN rm ~/miniconda.sh
RUN echo $PATH
ENV PATH ~/miniconda/bin:$PATH
RUN echo $PATH

# Install dependecies
RUN conda install -y pytest keras tqdm Pillow
RUN pip3 install tensorflow-gpu

# clone super-resolution 
WORKDIR /app/
RUN git clone https://github.com/krasserm/super-resolution

WORKDIR /app/super-resolution

RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/super-resolution/edsr-16-x4-gen-epoch-088.h5
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/super-resolution/wdsr-a-32-x4-psnr-29.1736.h5
