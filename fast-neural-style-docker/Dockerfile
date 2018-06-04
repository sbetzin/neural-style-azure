FROM kaixhin/cuda-torch:8.0

RUN apt-get -y install python2.7-dev libhdf5-dev

# this step takes ages so let's make a layer from it
RUN git clone https://github.com/jcjohnson/fast-neural-style.git \
#    && luarocks install torch \
#    && luarocks install nn \
#    && luarocks install image \
#    && luarocks install lua-cjson \
#    && luarocks install cutorch \
#    && luarocks install cunn \
#    && luarocks install cudnn \
#    && luarocks install camera \
#    && luarocks install qtlua \
#	&& luarocks install hdf5 \
#    && /root/torch/update.sh

#RUN cd fast-neural-style && bash models/download_style_transfer_models.sh \
#    && apt-get update \
#	&& apt-get upgrade \
#	&& apt-get install wget \
#   && rm -rf /var/lib/apt/lists/*


#RUN echo 'source /exports.sh' >> /root/.bashrc

#RUN wget -O /root/torch/fast-neural-style/models/vgg16.t7 https://cs.stanford.edu/people/jcjohns/fast-neural-style/models/vgg16.t7

Run pip install -r /root/torch/fast-neural-style/requirements.txt

WORKDIR /root/torch/fast-neural-style

ENTRYPOINT ["/bin/bash"]

