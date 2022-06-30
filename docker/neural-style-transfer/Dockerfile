FROM  anibali/pytorch:1.8.1-cuda11.1
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# Install basics
RUN sudo apt-get update && sudo apt-get install -y nano wget curl

# Install Timezone
RUN sudo apt-get update && sudo apt-get install -y tzdata 

# Update Timezone
#ENV TZ=Europe/Berlin
#RUN sudo ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && sudo echo $TZ > /etc/timezone

# Install OpenCV
RUN sudo apt-get install python3-opencv -y

WORKDIR /env/
COPY ["/env/environment.yml", "/env/"]
RUN conda env update

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
WORKDIR /app/
COPY ["/src/*.py" ,"/app/"]
COPY ["/src/utils/*.py" ,"/app/utils/"]
COPY ["/src/models/definitions/*.py" ,"/app/models/definitions/"]

ENTRYPOINT ["python", "/app/queueclient.py"]
