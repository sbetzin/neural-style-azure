# neural-style-azure
This is a docker wrapper of a tensorflow implementation with azure storage connection of several techniques described in the papers: 
* [Image Style Transfer Using Convolutional Neural Networks](http://www.cv-foundation.org/openaccess/content_cvpr_2016/papers/Gatys_Image_Style_Transfer_CVPR_2016_paper.pdf)
by Leon A. Gatys, Alexander S. Ecker, Matthias Bethge
* [Artistic style transfer for videos](https://arxiv.org/abs/1604.08610)
by Manuel Ruder, Alexey Dosovitskiy, Thomas Brox
* [Preserving Color in Neural Artistic Style Transfer](https://arxiv.org/abs/1606.05897)
by Leon A. Gatys, Matthias Bethge, Aaron Hertzmann, Eli Shechtman  
* [Tensorflow python implementation by cysmith](https://github.com/cysmith/neural-style-tf)

the main goal of project is to create a docker container running on a gpu host that connects itself to an azure storage queue waiting for new jobs. If there is a new job it will download all needed images from an azure storage blob storage and uses the jobs transformation parameter to create the neural style image. It will create two images. the first uses the colors of the style image. the seconds uses the color of the original image. both results are then uploaded to another azure blob storage.
With this approach you can run several pysical nvidia gpu machines (or azure n-series gpu machines) to create a whole cluster to process batch neural style transformations.

## Requirements 
### for host computer
- nvidia drivers
- nvidia cuda
- nvidia docker
- docker
### for running the container
- an [azure storage account](https://azure.microsoft.com/en-us/services/storage/) (the queue und the two blob storages are created automatically)

## Setup Host
detailed instructions to setup a host environment
[How to setup the host](https://github.com/sbetzin/neural-style-azure/blob/master/setup/setup%20host.md)
## Run the conainer
**Important!!!**
Make sure the environment variable _"AzureStorageConnectionString"_ contains the connection string for the required azure storage account

There are two possibilities how to start up the container
### 1. using docker command
you can use the nvidia-docker command to start up the container. the image is available on public host
[sbetzin/neural-style-tensorflow](https://hub.docker.com/r/sbetzin/neural-style-tensorflow/)

```bash
sudo nvidia-docker run -d -e AzureStorageConnectionString --name neural-style-tensorflow --restart=unless-stopped  sbetzin/neural-style-tensorflow
```
this command downloads the image from the public repository and starts it as deamon (-d --restart=unless-stopped)

### 2. clone repository and use make 
if you downloaded this repository [as described in the setup host instructions ](https://github.com/sbetzin/neural-style-azure/blob/master/setup/setup%20host.md) then you can just change to the docer repository
```bash
cd neural-style-azure/docker/neural-style-tensorflow-docker
```
and then just start it with
```bash
sudo make start
```
please note that the [make file](https://github.com/sbetzin/neural-style-azure/blob/master/docker/neural-style-tensorflow-docker/Makefile) has other commands as well.
