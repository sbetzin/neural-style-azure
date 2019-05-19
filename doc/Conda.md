### Install miniconda
wget https://repo.continuum.io/miniconda/Miniconda3-latest-Linux-x86_64.sh -O ~/miniconda.sh

~/miniconda.sh -b -p ~/miniconda 
export PATH=~/miniconda/bin:$PATH
rm ./miniconda.sh

### Conda Environments
https://towardsdatascience.com/getting-started-with-python-environments-using-conda-32e9f2779307

### Conda in Docker
https://medium.com/@chadlagore/conda-environments-with-docker-82cdc9d25754