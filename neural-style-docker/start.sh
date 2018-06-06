sudo nvidia-docker run -d -e AzureStorageConnectionString -e TileSize --name neural-style --restart=unless-stopped  sbetzin/neural-style
