FROM python:3.6.5
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

WORKDIR /usr/src/app
COPY requirements.txt ./
RUN pip install -r requirements.txt

#COPY ["/src/*.py" ,"/app/"]


#ENTRYPOINT ["python", "/app/queueclient.py"]
ENTRYPOINT ["bin/bash", ""]



