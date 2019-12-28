# Update All
### Update OS

sudo apt-get update
sudo apt-get upgrade -y
sudo apt-get dist-upgrade -y
sudo apt-get install cuda-drivers
sudo reboot

### Delete docker cache and get sources from git and rebuild
sudo docker system prune -a
cd neural-style-azure/docker/neural-style-tensorflow/
git pull
sudo make build

### (optional) Push the docker image to the docker hub
sudo docker login -u uuu -p ppp
sudo docker push sbetzin/neural-style-tensorflow:latest
