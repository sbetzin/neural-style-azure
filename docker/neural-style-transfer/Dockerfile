FROM nvidia/cuda:11.0.3-base-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# Install libs
RUN apt update && apt install -y libsm6 libxext6
RUN apt-get install -y libxrender-dev

# Install basics
RUN apt-get update && apt-get install -y nano wget git sudo curl


# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-latest-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
ENV PATH /root/miniconda/bin:$PATH

# Install dependencies
WORKDIR /env/
COPY ["/env/environment.yml", "/env/"]
RUN conda env update --file environment.yml

WORKDIR /app/
# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]
COPY ["/src/utils/*.py" ,"/app/utils/"]
COPY ["/src/models/definitions/*.py" ,"/app/models/definitions/"]

ENTRYPOINT ["/root/miniconda/bin/python", "/app/queueclient.py"]

#ENTRYPOINT ["/root/anaconda/bin/conda", "run", "--no-capture-output", "-n", "pytorch-nst", "python", "/app/queueclient.py"]
