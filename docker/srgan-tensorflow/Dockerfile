FROM tensorflow/tensorflow:latest-gpu
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"

# prepare basic os and python libs
RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo zip
RUN apt-get install libffi-dev libssl-dev libxml2-dev libxslt1-dev libjpeg8-dev zlib1g-dev
RUN apt-get -y install python3-pip
RUN pip3 install tensorflow==2.0.0-alpha0 tensorflow-gpu==2.0.0-alpha0 tensorlayer
RUN pip3 install --upgrade tensorlayer[all]
RUN pip3 install easydict 
RUN pip3 install azure-storage-queue azure-storage-blob


# clone srgan 
WORKDIR /app/
RUN git clone https://github.com/tensorlayer/srgan

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
#COPY ["/src/*.py" ,"/app/"]

# ENTRYPOINT ["python", "/app/queueclient.py"]#