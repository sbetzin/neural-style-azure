FROM  anibali/pytorch:1.8.1-cuda11.1
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"
USER root

# Install basics
RUN apt-get update && apt-get install -y nano wget curl

# Install Timezone Silent
# https://serverfault.com/questions/949991/how-to-install-tzdata-on-a-ubuntu-docker-image
RUN DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get -y install tzdata

# Install OpenCV (tzdata needs to be installed -> otherwise a prompt shows up)
RUN apt-get install python3-opencv -y

WORKDIR /env/
COPY ["/env/environment.yml", "/env/"]
RUN chmod 777 /env/

#USER user
RUN conda env update

#Download vgg19 modell
RUN wget -P /home/user/.cache/torch/hub/checkpoints https://neuralstylefiles.blob.core.windows.net/models/neural-style-transfer/vgg19-dcbb9e9d.pth

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
WORKDIR /app/
COPY ["/src/*.py" ,"/app/"]
COPY ["/src/utils/*.py" ,"/app/utils/"]
COPY ["/src/models/definitions/*.py" ,"/app/models/definitions/"]

ENTRYPOINT ["python", "/app/queueclient.py"]
