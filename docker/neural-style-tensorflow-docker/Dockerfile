FROM tensorflow/tensorflow:latest-gpu
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

# prepare basic os and python libs
RUN apt-get update && apt-get -y upgrade && apt-get install -y nano wget git sudo
RUN pip install --upgrade pip && pip install opencv-python-headless wheel azure-storage-queue azure-storage-blob blobxfer piexif moviepy

# download loss network
WORKDIR /app
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/imagenet-vgg-verydeep-19.mat

RUN mkdir /app/images && mkdir /app/images/enlarge

# adding the SRGAN Tensorflow implementation to updsclate images
RUN git clone https://github.com/brade31919/SRGAN-tensorflow.git

WORKDIR /app/SRGAN-tensorflow

# we need pretrained models here
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/vgg19.t7
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/srgan-pretrained/SRGAN_pre-trained.tar && tar xvf SRGAN_pre-trained.tar && rm SRGAN_pre-trained.tar

# adding the fast neural style tensorflow implementation
WORKDIR /app
RUN git clone https://github.com/lengstrom/fast-style-transfer.git

# This seems to be a different model than we are using above
WORKDIR /app/fast-style-transfer
RUN mkdir data && wget --progress=bar:force http://www.vlfeat.org/matconvnet/models/beta16/imagenet-vgg-verydeep-19.mat

# adding ffmpeg for fast style video transfer
RUN imageio_download_bin ffmpeg

# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]

ENTRYPOINT ["python", "/app/queueclient.py"]