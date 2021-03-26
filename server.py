import uvicorn
from fastapi import FastAPI, File, UploadFile
from starlette.responses import RedirectResponse
from PIL import Image
from io import BytesIO
import numpy as np
import tensorflow as tf
from tensorflow.keras.applications.imagenet_utils import decode_predictions

app = FastAPI()

input_shape = (224, 224)

def load_model():
    model = tf.keras.applications.EfficientNetB0(input_shape)
    return model

model = load_model()

def read_image(file):
    image = Image.open(BytesIO(file))
    return image


def preprocess(image: Image.Image):
    image = image.resize(input_shape)
    image = np.asfarray(image)
    image = image / 255
    image = np.expand_dims(image, 0)
    return image


def predict(image: np.ndarray):
    predictions = model.predict(image)
    predictions = decode_predictions(predictions)[0][0][1]
    return predictions


@app.get("/", include_in_schema=False)
async def index():
    return RedirectResponse(url="/docs")

@app.post("/predict/image")
async def predict_image(file: UploadFile = File(...)):
    #read file uploaded
    image = read_image(await file.read())
    #preprocessing of image
    image = preprocess(image)
    #pass to model to classify
    predictions = predict(image)
    print(predictions)
    return predictions

if __name__ == "__main__":
    uvicorn.run(app, port = 8000, host = "127.0.0.1")