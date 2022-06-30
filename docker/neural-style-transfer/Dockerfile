FROM nvidia/cuda:11.0.3-base-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# remove python 2.7 and 3.5 from native installations
RUN apt-get update && apt purge python2.7-minimal -y && apt purge python3-minimal -y && apt autoremove -y

# install basics
RUN apt-get update && apt-get install -y nano wget git sudo

# install dependencies
COPY ["/src/*.yml", "/app/"]

WORKDIR /app/
RUN conda env create
RUN activate pytorch-nst

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["/opt/conda/bin/python", "/app/queueclient.py"]
