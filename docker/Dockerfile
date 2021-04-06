# base images

FROM python:3.8-alpine
FROM tensorflow/tensorflow:latest

# install dependencies
COPY ./requirements.txt .
RUN pip install -r requirements.txt

# setting env variables
ENV MODEL_NAME=effnet1

# copy project over 
COPY . .

# exposing port, heroku using random assign
EXPOSE $PORT

# setting entry point 
ENTRYPOINT ["python"]
CMD ["deploylocal.py"]