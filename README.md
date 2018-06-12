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
- host with nvidia drivers, nvidia cuda, nvidia-docker, docker 
  see 

