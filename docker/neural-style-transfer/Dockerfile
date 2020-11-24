FROM nvidia/cuda:11.1-runtime-ubuntu18.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# update basic os
RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo

# dependecies for python libs (ssl)
RUN apt-get install -y libsm6 libxext6 libxrender-dev

# Install miniconda
#RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-latest-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
RUN wget --progress=bar:force https://repo.anaconda.com/archive/Anaconda3-2020.11-Linux-x86_64.sh -O ~/anaconda.sh && sh ~/anaconda.sh -b -p /root/anaconda && rm ~/anaconda.sh
ENV PATH /root/miniconda/bin:$PATH

#install python libs
WORKDIR /env/
COPY ["/env/environment.yml" ,"/env/"]
RUN conda env update --file environment.yml

WORKDIR /app/
# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/" ,"/app/"]

ENTRYPOINT ["/root/miniconda/bin/python", "/app/queueclient.py"]