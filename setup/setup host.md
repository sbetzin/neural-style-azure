# Installation instructions for host
you can eighter use a physical host or one of [azure n-series with gpu support](https://docs.microsoft.com/en-us/azure/virtual-machines/windows/sizes-gpu).

the installation instructions are optimized for an ubuntu 16-04 azure virtual machine (n-series) server and based on 
https://docs.microsoft.com/de-de/azure/virtual-machines/linux/n-series-driver-setup

### 1. First install the Cuda drivers and Cuda from official nvidia repos
```bash
CUDA_REPO_PKG=cuda-repo-ubuntu1604_9.1.85-1_amd64.deb
wget -O /tmp/${CUDA_REPO_PKG} http://developer.download.nvidia.com/compute/cuda/repos/ubuntu1604/x86_64/${CUDA_REPO_PKG} 
sudo dpkg -i /tmp/${CUDA_REPO_PKG}
sudo apt-key adv --fetch-keys http://developer.download.nvidia.com/compute/cuda/repos/ubuntu1604/x86_64/7fa2af80.pub
rm -f /tmp/${CUDA_REPO_PKG}
sudo apt-get update
sudo apt-get upgrade
sudo apt-get -y install cuda-drivers
sudo apt-get -y install cuda
```
### 2. install docker 
this is based on the installation instructions
https://docs.docker.com/install/linux/docker-ce/ubuntu/#install-docker-ce
```bash
sudo apt-get -y install git apt-transport-https ca-certificates curl software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"
sudo apt-get update
sudo apt-get -y install docker-ce
```

### 3. now install nvidia-docker 
based on the installation instructions
Nvidia Docker https://github.com/NVIDIA/nvidia-docker
```bash
curl -s -L https://nvidia.github.io/nvidia-docker/gpgkey | sudo apt-key add -
distribution=$(. /etc/os-release;echo $ID$VERSION_ID)
curl -s -L https://nvidia.github.io/nvidia-docker/$distribution/nvidia-docker.list | sudo tee /etc/apt/sources.list.d/nvidia-docker.list
sudo apt-get update
sudo apt-get install -y nvidia-docker2
sudo pkill -SIGHUP dockerd
```
### 4. Check if nvidia is running correctly
```bash
nvidia-smi
```

### 5. Add the azure storage connectionstring to an evironment variable
```bash
sudo -H nano /etc/environment
```
append the lines
**AzureStorageConnectionString="<yourstorageconnection>"**

and optional (if you want to change the default name "jobs")
**AzureStorageQueueName="jobs"**

if you dont have nano installed you can easily get it
```bash
sudo apt-get install nano
```
### 6. Reboot computer
```bash
sudo reboot
```
### 7. Check environment variable is working correctly
```bash
echo $AzureStorageConnectionString
echo $AzureStorageQueueName
```
### 8. setup ssh to commit back to githup (optional)
https://help.github.com/articles/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent/
```bash
ssh-keygen -t rsa -b 4096 -C "<your email>"
eval $(ssh-agent -s)
ssh-add ~/.ssh/id_rsa
```
### 9. Clone Repository (optional)
```bash
git clone git@github.com:sbetzin/neural-style-azure.git
```
