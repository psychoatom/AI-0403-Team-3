# importing necessary libraries

import uvicorn
from fastapi import FastAPI, File, UploadFile
from starlette.responses import RedirectResponse
from PIL import Image
from io import BytesIO
import numpy as np
import tensorflow as tf
from tensorflow.keras.applications.imagenet_utils import decode_predictions
from tensorflow.keras.models import load_model
import os


# setting parameters

app = FastAPI()
input_shape = (224, 224)

# loading saved model

save_folder = "../models"
model_name = "effnet1"
model = load_model(f"{save_folder}/{model_name}")

def read_image(file):
    image = Image.open(BytesIO(file))
    return image

# defining image preprocess 

def preprocess(image: Image.Image):
    image = image.resize(input_shape)
    image = np.asfarray(image)
    #image = image / 255
    image = np.expand_dims(image, 0)
    return image

# loading the image for inference

def predict(image: np.ndarray):
    pred = model.predict(image)
    if pred[0][0] > pred[0][1]:
        infer = "With Mask"
    else:
        infer = "Without Mask"
    return infer

# Restful API for the inference 

@app.get("/", include_in_schema=False)
async def index():
    return RedirectResponse(url="/docs")

@app.post("/predict/image")
async def predict_image(file: UploadFile = File(...)):
    image = read_image(await file.read())
    image = preprocess(image)
    infer = predict(image)
    print(infer)
    return infer

# on starting the service

if __name__ == "__main__":
    port = int(os.environ.get("PORT", 8000))
    # change host to "0.0.0.0" before building image to deploy on heroku 
    # or "127.0.0.1" for local
    uvicorn.run(app, port = port, host = "0.0.0.0")