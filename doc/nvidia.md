# Troubleshooting
## Drivers
https://askubuntu.com/questions/670485/how-to-inspect-the-currently-used-nvidia-driver-version-and-switch-it-to-another

### List available drivers 
apt-cache search nvidia | grep -P '^nvidia-[0-9]+\s'

### get driver version
cat /proc/driver/nvidia/version

### Install nvdia in a specific version
apt-get install nvidia-418

