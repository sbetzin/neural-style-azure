FROM ubuntu:16.04
RUN apt-get update
RUN apt-get install -y software-properties-common nano

FROM python:3.6.5
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

WORKDIR /app
COPY requirements.txt ./
RUN pip install -r requirements.txt

RUN git clone https://github.com/VainF/Neural-Style-Transfer-Gatys.git

WORKDIR Neural-Style-Transfer-Gatys
RUN wget https://neuralstylefiles.blob.core.windows.net/models/vgg19.npy

WORKDIR /app
#COPY ["/src/*.py" ,"/app/"]

#ENTRYPOINT ["python", "/app/queueclient.py"]
ENTRYPOINT ["/bin/bash"]



