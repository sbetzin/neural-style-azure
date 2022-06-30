FROM nvidia/cuda:11.0.3-base-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# Install basics
RUN apt-get update && apt-get install -y nano wget git sudo curl

# Install anaconda
# https://docs.anaconda.com/anaconda/install/silent-mode/
WORKDIR /tmp/
RUN curl https://repo.anaconda.com/archive/Anaconda3-2021.11-Linux-x86_64.sh --output anaconda.sh
RUN bash anaconda.sh -b -p $HOME/anaconda
RUN rm anaconda.sh

# Install dependencies
WORKDIR /app/
COPY ["/src/*.yml", "/app/"]
RUN /root/anaconda/bin/conda env create
RUN /root/anaconda/bin/conda activate pytorch-nst

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["/opt/conda/bin/python", "/app/queueclient.py"]
