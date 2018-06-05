FROM ubuntu:16.04
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

RUN apt-get update
RUN apt-get install -y software-properties-common python-software-properties nano
#RUN apt-get install python3.6

WORKDIR /app
COPY requirements.txt ./
RUN pip install -r requirements.txt

RUN git clone https://github.com/VainF/Neural-Style-Transfer-Gatys.git

WORKDIR Neural-Style-Transfer-Gatys
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/vgg19.npy

WORKDIR /app
#COPY ["/src/*.py" ,"/app/"]

#ENTRYPOINT ["python", "/app/queueclient.py"]
ENTRYPOINT ["/bin/bash"]



