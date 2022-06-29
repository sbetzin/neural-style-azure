FROM continuumio/anaconda3:latest
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# install basics
#RUN apt-get install -y nano wget git sudo

# install dependencies
WORKDIR /src/
RUN conda env create
RUN activate pytorch-nst

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
WORKDIR /app/
COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["/root/miniconda/bin/python", "/app/queueclient.py"]
