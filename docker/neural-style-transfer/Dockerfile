FROM nvidia/cuda:10.2-cudnn7-devel-ubuntu18.04
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# set bash as current shell
RUN chsh -s /bin/bash
SHELL ["/bin/bash", "-c"]

# Install basics
RUN apt-get update && apt-get install -y nano wget curl

# install anaconda
RUN apt-get update
RUN apt-get install -y wget bzip2 ca-certificates libglib2.0-0 libxext6 libsm6 libxrender1 git mercurial subversion && \
        apt-get clean
RUN wget --quiet https://repo.anaconda.com/archive/Anaconda3-2020.02-Linux-x86_64.sh -O ~/anaconda.sh && \
        /bin/bash ~/anaconda.sh -b -p /opt/conda && \
        rm ~/anaconda.sh && \
        ln -s /opt/conda/etc/profile.d/conda.sh /etc/profile.d/conda.sh && \
        echo ". /opt/conda/etc/profile.d/conda.sh" >> ~/.bashrc && \
        find /opt/conda/ -follow -type f -name '*.a' -delete && \
        find /opt/conda/ -follow -type f -name '*.js.map' -delete && \
        /opt/conda/bin/conda clean -afy

# set path to conda
ENV PATH /opt/conda/bin:$PATH

# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
# RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-latest-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
# ENV PATH /root/miniconda/bin:$PATH

# Install dependencies
WORKDIR /env/
COPY ["/env/environment.yml", "/env/"]
RUN conda update conda && conda env create --name styletransfer -f /env/environment.yml

RUN echo "conda activate styletransfer" >> ~/.bashrc
ENV PATH /opt/conda/envs/styletransfer/bin:$PATH
ENV CONDA_DEFAULT_ENV $styletransfer

#RUN conda env update --file environment.yml

WORKDIR /app/
# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]
COPY ["/src/utils/*.py" ,"/app/utils/"]
COPY ["/src/models/definitions/*.py" ,"/app/models/definitions/"]

# Make RUN commands use the new environment:
#SHELL ["conda", "run", "-n", "styletransfer", "/bin/bash", "-c"]

ENTRYPOINT ["python", "/app/queueclient.py"]

#ENTRYPOINT ["conda", "run", "--no-capture-output", "-n", "styletransfer", "python", "/app/queueclient.py"]
