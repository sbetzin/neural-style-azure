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

# https://pythonspeed.com/articles/activate-conda-dockerfile/
# Make RUN commands use the new environment:
SHELL ["/root/anaconda/bin/conda", "run", "-n", "pytorch-nst", "/bin/bash", "-c"]

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]
COPY ["/src/utils/*.py" ,"/app/utils/"]
COPY ["/src/models/*.py" ,"/app/models/"]

RUN apt update && apt install -y libsm6 libxext6
RUN apt-get install -y libxrender-dev

ENTRYPOINT ["/root/anaconda/bin/conda", "run", "--no-capture-output", "-n", "pytorch-nst", "python", "/app/queueclient.py"]
