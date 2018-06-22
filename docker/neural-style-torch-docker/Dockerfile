FROM nvidia/cuda:9.1-cudnn7-devel
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

# Install system dependencies
RUN set -ex && \
	apt-get update && apt-get install --no-install-recommends --no-install-suggests -y \
	ca-certificates \
	sudo \
	libprotobuf-dev \
	protobuf-compiler \
	wget \
	git \
	nano \
	&& apt-get clean && rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*
	
# Install torch
RUN git clone https://github.com/torch/distro.git /root/torch --recursive && cd /root/torch && bash install-deps
RUN cd /root/torch && ./clean.sh && export TORCH_NVCC_FLAGS="-D__CUDA_NO_HALF_OPERATORS__" && ./install.sh
RUN ln -s /root/torch/install/bin/* /usr/local/bin

# Install additional necessary torch dependencies
RUN luarocks install loadcaffe && luarocks install autograd

#  Fix torch error "Then make sure files named as libcudnn.so.5 or libcudnn.5.dylib are placed in ..." https://github.com/soumith/cudnn.torch/issues/383
RUN git clone https://github.com/soumith/cudnn.torch.git -b R7 && cd cudnn.torch && luarocks make cudnn-scm-1.rockspec

# Install Python miniconda3 + requirements
ENV MINICONDA_HOME="/opt/miniconda"
ENV PATH="${MINICONDA_HOME}/bin:${PATH}"
RUN curl -o Miniconda3-latest-Linux-x86_64.sh https://repo.continuum.io/miniconda/Miniconda3-latest-Linux-x86_64.sh \
  && chmod +x Miniconda3-latest-Linux-x86_64.sh \
  && ./Miniconda3-latest-Linux-x86_64.sh -b -p "${MINICONDA_HOME}" \
  && rm Miniconda3-latest-Linux-x86_64.sh
RUN conda install -y numpy
RUN conda clean -y -i -l -p -t && rm -f conda.txt
RUN pip install gputil

# Install fast-neural-style train dependencies
RUN set -ex && \
	apt-get install --no-install-recommends --no-install-suggests -y \
	libhdf5-dev
	
# install hdf5 for fast neural style training
RUN luarocks install https://raw.githubusercontent.com/deepmind/torch-hdf5/master/hdf5-0-0.rockspec

# Clone neural-style app and fast-neural-style
WORKDIR /app
RUN git clone https://github.com/jcjohnson/neural-style.git && git clone https://github.com/jcjohnson/fast-neural-style.git
	
# Download precomputed VGG network weights as in script models/download_models.sh
WORKDIR /app/neural-style/models
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/VGG_ILSVRC_19_layers_deploy.prototxt.txt \
	&& wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/vgg_normalised.caffemodel \
	&& wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/VGG_ILSVRC_19_layers.caffemodel

# Download t7  source: https://github.com/Cadene/torchnet-vision/tree/master/models
WORKDIR /app/fast-neural-style
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/vgg16.t7
RUN wget --progress=bar:force https://neuralstylefiles.blob.core.windows.net/models/vgg19.t7

# Update Torch
WORKDIR /root/torch
RUN sudo bash update.sh

# Add neural-style to path
ENV PATH /app/neural-style:$PATH
ENV PATH /app/fast-neural-style:$PATH

# Install Azure Dependencies
RUN pip install wheel azure-storage-queue azure-storage-blob blobxfer 

RUN mkdir /app/images && mkdir /app/convert-models

# Copy wrapper scripts
COPY ["/src/*.py" ,"/app/"]
COPY ["/lua/correct_colors.lua", "/app/"]
COPY ["/lua/fast_neural_style.lua", "/app/fast-neural-style/"]
COPY ["/lua/convert-models/*.lua", "/app/convert-models/"]


ENTRYPOINT ["python", "/app/queueclient.py"]




