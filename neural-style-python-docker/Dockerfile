FROM python:3.6.5
MAINTAINER "Sebastian Betzin, https://github.com/sbetzin"

WORKDIR /app
COPY requirements.txt ./
RUN pip install -r requirements.txt

RUN git clone https://github.com/VainF/Neural-Style-Transfer-Gatys.git


#COPY ["/src/*.py" ,"/app/"]


#ENTRYPOINT ["python", "/app/queueclient.py"]
ENTRYPOINT ["/bin/bash"]



