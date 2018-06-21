FROM tensorflow/tensorflow:latest-gpu
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo
RUN pip install --upgrade pip && pip install opencv-python-headless wheel azure-storage-queue azure-storage-blob blobxfer piexif

WORKDIR /app
RUN git clone https://github.com/brade31919/SRGAN-tensorflow.git

WORKDIR /app/SRGAN-tensorflow

RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/vgg19.t7
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/srgan-pretrained/SRGAN_pre-trained.tar && tar xvf SRGAN_pre-trained.tar && rm SRGAN_pre-trained.tar

WORKDIR /app/SRGAN-tensorflow/data/test_LR
RUN rm *.png  && wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/ana.png

WORKDIR /app/SRGAN-tensorflow

#RUN mkdir /app/images

#COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["/bin/bash"]
#ENTRYPOINT ["python", "/app/queueclient.py"]