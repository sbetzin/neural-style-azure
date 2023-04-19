FROM nvidia/cudagl:11.1.1-runtime-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0.1"
USER root

# update basic os
RUN apt-get update && apt-get upgrade -y 

# Install Timezone Silent
# https://serverfault.com/questions/949991/how-to-install-tzdata-on-a-ubuntu-docker-image
RUN DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get -y install tzdata

RUN apt-get install -y nano wget curl git sudo libglib2.0-0 libsm6 libxrender1 libxext6 fontconfig

#Install Python and Add python alias for python3
RUN apt-get install python3.8 pip python3.8-venv -y && update-alternatives --install /usr/bin/python python /usr/bin/python3 1

# Install poetry
RUN curl -sSL https://install.python-poetry.org | python3 - 
ENV PATH="/root/.local/bin:${PATH}"

#Install dependencies
COPY ["/src/pyproject.toml", "/app/"]
WORKDIR /app
RUN poetry install

# Clone BoostingMonocularDepth Repo
RUN git clone https://github.com/sbetzin/BoostingMonocularDepth.git

# Download models
COPY ["/src/download_models.py", "/app/"]
RUN python download_models.py /app/

# Copy sources
COPY ["/src/*.py" ,"/app/"]
COPY ["/src/MiDaS/*.py" ,"/app/MiDaS/"]

