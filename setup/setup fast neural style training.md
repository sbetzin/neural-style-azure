# install additional disk in vm
https://docs.microsoft.com/de-de/azure/virtual-machines/linux/add-disk

# setup json config for docker to store all docker related data on the new disk
sudo nano  /etc/docker/daemon.json
"graph":"/datadrive"

# add all training files from COCO to the new disk 
# http://cocodataset.org/#download
cd /datadrive
sudo mkdir training
sudo chmod -v 777 training
cd training
wget http://images.cocodataset.org/zips/train2014.zip
wget http://images.cocodataset.org/zips/val2014.zip

sudo apt-get install zip unzip 

unzip train2014.zip
unzip val2014.zip

rm train2014.zip
rm val2014.zip

# create the h5 file
# first start the fast neural style docker file 
sudo nvidia-docker run --rm -it -v /datadrive/training:/training sbetzin/fast-neural-style

#run python script to create the h5 file using the mapped datadrive
python scripts/make_style_dataset.py --train_dir /training/train2014 --val_dir /training/val2014 --output_file /training/coco2014.h5



https://pypi.org/project/blobxfer/0.11.5/