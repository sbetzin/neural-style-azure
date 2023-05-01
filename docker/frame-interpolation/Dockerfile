FROM anibali/pytorch:1.8.1-cuda11.1
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0.0"
USER root

# update basic os
RUN apt-get update && apt-get upgrade -y 

# Install Timezone Silent
# https://serverfault.com/questions/949991/how-to-install-tzdata-on-a-ubuntu-docker-image
RUN DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get -y install tzdata

RUN apt-get install -y nano wget curl git sudo

#Install Python and Add python alias for python3
#RUN apt-get install python3.8 pip python3.8-venv -y && update-alternatives --install /usr/bin/python python /usr/bin/python3 1

# Install poetry
RUN curl -sSL https://install.python-poetry.org | python3 - 
ENV PATH="~/.local/bin:${PATH}"

# Turn off virtual env creation
# https://github.com/orgs/python-poetry/discussions/1879
RUN poetry config virtualenvs.create false

#Install dependencies
#COPY ["/src/pyproject.toml", "/app/"]
#WORKDIR /app
#RUN poetry install

# Clone BoostingMonocularDepth Repo
RUN git clone https://github.com/google-research/frame-interpolation

# Download models
#COPY ["/src/download_models.py", "/app/"]
#RUN python download_models.py /app/

# Copy sources
COPY ["/src/*.py" ,"/app/"]

#ENTRYPOINT ["python", "/app/queueclient.py"]