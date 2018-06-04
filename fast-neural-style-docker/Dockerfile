FROM kaixhin/cuda-torch:8.0

RUN apt-get -y install python2.7-dev libhdf5-dev

RUN git clone https://github.com/jcjohnson/fast-neural-style.git 

RUN pip install -r /root/torch/fast-neural-style/requirements.txt

WORKDIR /root/torch/fast-neural-style

ENTRYPOINT ["/bin/bash"]