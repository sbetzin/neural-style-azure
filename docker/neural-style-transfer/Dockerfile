FROM  anibali/pytorch:1.8.1-cuda11.1
LABEL maintainer="Sebastian Betzin, https://github.com/sbetzin"
LABEL version="1.0"

# Install basics
RUN sudo apt-get update && sudo apt-get install -y nano wget curl

WORKDIR /app/
# Copy the neural style files (modified) -> this is why we are not cloning the orignal github files
COPY ["/src/*.py" ,"/app/"]
COPY ["/src/utils/*.py" ,"/app/utils/"]
COPY ["/src/models/definitions/*.py" ,"/app/models/definitions/"]

# Make RUN commands use the new environment:
#SHELL ["conda", "run", "-n", "styletransfer", "/bin/bash", "-c"]

ENTRYPOINT ["python", "/app/queueclient.py"]

#ENTRYPOINT ["conda", "run", "--no-capture-output", "-n", "styletransfer", "python", "/app/queueclient.py"]
