# Source
https://pypi.org/project/blobxfer/

# Installation
pip install blobxfer --user

# Documentation
http://blobxfer.readthedocs.io/en/latest/10-cli-usage/

blobxfer upload --storage-account neuralstylefiles --sas "<key>" --remote-path models --local-path vgg16.t7


blobxfer upload --storage-account neuralstylefiles --sas "<key>" --remote-path models/style --local-path /datadrive/training/models/style --no-overwrite


blobxfer download --storage-account neuralstylefiles --sas "<key>" --remote-path models/style --local-path /datadrive/training/models --no-overwrite




blobxfer upload --storage-account neuralstylefiles --sas "" --remote-path results/srgan --local-path ana.png