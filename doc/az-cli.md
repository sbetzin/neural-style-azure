# Azure AZ CLI

## Online Documentation

### azure cli command documentation
https://docs.microsoft.com/en-us/cli/azure/container?view=azure-cli-latest#az-container-create
### how to start a new container with cli
https://docs.microsoft.com/bs-latn-ba/azure/container-instances/container-instances-start-command?view=azure-cli-2018-03-01-hybrid
### important information with gpu containers
https://docs.microsoft.com/de-de/azure/container-instances/container-instances-gpu


## Important commands
### Create a new container with images with a yaml file
az container create --resource-group neuralstyle  -f c:\data\neuralstyle.yaml --no-wait

### displays the current state 
az container show -g neuralstyle --name neural-style-container --output table

### shows the log file
az container logs -g neuralstyle --name neural-style-container --output table

### lists all containers
az container list -g neuralstyle

### exports a container to a yaml file
az container export -g neuralstyle --name neural-style-container -f c:\data\test.yaml