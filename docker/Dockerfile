FROM python:3.8-alpine
FROM tensorflow/tensorflow:latest

#RUN apk --update add \
#    build-base \
#    jpeg-dev \
#    zlib-dev

# install dependencies
COPY ./requirements.txt .
RUN pip install -r requirements.txt

ENV MODEL_NAME=effnet1

#copy project over 
COPY . .

# add and run as non-root user
#RUN adduser -D grtang
#USER grtang

EXPOSE $PORT

#CMD ["/usr/bin/tf_serving_entrypoint.sh"]
ENTRYPOINT ["python"]
CMD ["deploylocal.py"]