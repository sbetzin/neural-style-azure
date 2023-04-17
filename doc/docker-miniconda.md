# Install miniconda
# export PATH="~/miniconda/bin:$PATH"
#RUN wget --progress=bar:force https://repo.continuum.io/miniconda/Miniconda3-py37_4.12.0-Linux-x86_64.sh -O ~/miniconda.sh && sh ~/miniconda.sh -b -p /root/miniconda && rm ~/miniconda.sh
#ENV PATH /root/miniconda/bin:$PATH

#COPY ["/env/environment.yml", "/env/"]
#RUN chmod 777 /env/

#WORKDIR /env/
#RUN conda env update --file environment.yml