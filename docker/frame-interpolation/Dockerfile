FROM  nvidia/cuda:11.2.1-cudnn8-runtime-ubuntu20.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0.0"
USER root

# update basic os
RUN apt-get update && apt-get upgrade -y 

# Install Timezone Silent
# https://serverfault.com/questions/949991/how-to-install-tzdata-on-a-ubuntu-docker-image
RUN DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get -y install tzdata
RUN apt-get install -y nano wget curl git sudo ffmpeg unzip libglib2.0-0 libsm6 libxrender1 libxext6 fontconfig

#Install Python and Add python alias for python3
RUN apt-get install python3.8 pip python3.8-venv -y && update-alternatives --install /usr/bin/python python /usr/bin/python3 1

# Install poetry
RUN curl -sSL https://install.python-poetry.org | python3 - 
ENV PATH="~/.local/bin:${PATH}"

# Turn off virtual env creation
# https://github.com/orgs/python-poetry/discussions/1879
RUN ~/.local/bin/poetry config virtualenvs.create false

# Download models
RUN wget -O /tmp/models.zip https://neuralstylefiles.blob.core.windows.net/models/frame-interpolation/pretrained_models-20230501T220609Z-001.zip
RUN unzip /tmp/models.zip -d /
RUN rm /tmp/models.zip

#Install dependencies
COPY ["/src/pyproject.toml", "/app/"]
WORKDIR /app
RUN ~/.local/bin/poetry install

#Create Intermediate directory for the intermediate videos
RUN mkdir -p /intermediate

# Copy sources. They are filtered only to .py and .yaml in the .dockerignore file
COPY src/ .

ENTRYPOINT ["python", "/app/run.py"]