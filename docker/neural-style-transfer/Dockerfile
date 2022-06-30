FROM nvidia/cuda:11.0.3-base-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# install basics
RUN apt-get update && apt-get install -y nano wget git sudo curl

# install anaconda
RUN cd /tmp
RUN curl https://repo.anaconda.com/archive/Anaconda3-2021.11-Linux-x86_64.sh --output anaconda.sh
RUN bash anaconda.sh



# install dependencies
COPY ["/src/*.yml", "/app/"]
WORKDIR /app/
RUN conda env create
RUN activate pytorch-nst

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["/opt/conda/bin/python", "/app/queueclient.py"]
