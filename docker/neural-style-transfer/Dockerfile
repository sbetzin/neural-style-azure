FROM nvidia/cuda:11.3.1-base-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# Remove any third-party apt sources to avoid issues with expiring keys.
RUN rm -f /etc/apt/sources.list.d/*.list

# Install some basic utilities
RUN apt-get update && apt-get install -y \
    curl \
    ca-certificates \
    sudo \
    git \
    bzip2 \
    libx11-6 \
 && rm -rf /var/lib/apt/lists/*


# Install libs for open cv
RUN sudo apt-get update && sudo apt-get install -y libgl1-mesa-glx libgtk2.0-0 libsm6 libxext6 && sudo rm -rf /var/lib/apt/lists/*


# Install basics
RUN apt-get update && apt-get install -y nano wget curl


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

# Make RUN commands use the new environment:
#SHELL ["conda", "run", "-n", "styletransfer", "/bin/bash", "-c"]

ENTRYPOINT ["/root/miniconda/bin/python", "/app/queueclient.py"]

#ENTRYPOINT ["conda", "run", "--no-capture-output", "-n", "styletransfer", "python", "/app/queueclient.py"]
