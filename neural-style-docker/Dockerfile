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
COPY conda.txt conda.txt
RUN conda install -y --file=conda.txt
RUN conda clean -y -i -l -p -t && rm -f conda.txt
COPY pip.txt pip.txt
RUN pip install -r pip.txt && rm -f pip.txt

# Install fast-neural-style train dependencies
RUN set -ex && \
	apt-get install --no-install-recommends --no-install-suggests -y \
	python2.7-dev \
	libhdf5-dev
	
# install hdf5 for fast neural style training
RUN luarocks install https://raw.githubusercontent.com/deepmind/torch-hdf5/master/hdf5-0-0.rockspec

# Clone neural-style app and fast-neural-style
WORKDIR /app
RUN git clone https://github.com/jcjohnson/neural-style.git && RUN git clone https://github.com/jcjohnson/fast-neural-style.git
	
# Download precomputed VGG network weights
WORKDIR /app/neural-style
RUN bash models/download_models.sh

# Download t7  source: https://github.com/Cadene/torchnet-vision/tree/master/models
WORKDIR /app/fast-neural-style
RUN wget --progress=dot --output-document=vgg16.t7 https://cs.stanford.edu/people/jcjohns/fast-neural-style/models/vgg16.t7  

#Install Python requirements for training
RUN pip install -r scripts/requirements.txt

# Add neural-style to path
ENV PATH /app/neural-style:$PATH
ENV PATH /app/fast-neural-style:$PATH

# Clone style-swap app
WORKDIR /app
RUN set -ex && \
	wget --no-check-certificate https://github.com/rtqichen/style-swap/archive/master.tar.gz && \
	tar -xvzf master.tar.gz && \
    mv style-swap-master style-swap && \
	rm master.tar.gz
# Link precomputed VGG network weights
RUN rm -rf /app/style-swap/models
RUN ln -s /app/neural-style/models /app/style-swap/models
# Add precomputed inverse network model
ADD models/dec-tconv-sigmoid.t7 /app/style-swap/models/dec-tconv-sigmoid.t7

# Install Azure Dependencies
RUN pip install wheel azure-storage-queue azure-storage-blob

# Copy wrapper scripts and config files
COPY ["/src/*.py" ,"/app/"]
RUN mkdir images

ENTRYPOINT ["python", "/app/queueclient.py"]




