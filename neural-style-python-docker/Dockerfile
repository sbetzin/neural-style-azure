FROM ubuntu:18.04
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

RUN apt-get update && apt-get upgrade
#RUN apt-get install -y software-properties-common nano wget git sudo
#RUN apt-get install build-essential libpq-dev libssl-dev openssl libffi-dev zlib1g-dev python-software-properties
#RUN apt-get install python3-pip python3-dev
#RUN apt-get install python3.6

WORKDIR /app
COPY requirements.txt ./
#RUN pip install -r requirements.txt

RUN git clone https://github.com/VainF/Neural-Style-Transfer-Gatys.git

WORKDIR Neural-Style-Transfer-Gatys
#RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/vgg19.npy

WORKDIR /app
#COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["/bin/bash"]



