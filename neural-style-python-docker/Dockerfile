FROM tensorflow/tensorflow:latest-gpu
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

#RUN apt-get update && apt-get -y upgrade && apt-get install -y software-properties-common nano wget git sudo
#RUN apt-get install -y python3-pip
#RUN apt-get install -y python3.5-tk

RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo

WORKDIR /app
#COPY requirements.txt ./
#RUN pip3 install -r requirements.txt
RUN git clone https://github.com/VainF/Neural-Style-Transfer-Gatys.git

WORKDIR Neural-Style-Transfer-Gatys
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/vgg19.npy

WORKDIR /app
#COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["/bin/bash"]



