import uvicorn
from fastapi import FastAPI, File, UploadFile
from starlette.responses import RedirectResponse
from PIL import Image
from io import BytesIO
import numpy as np
import tensorflow as tf
from tensorflow.keras.applications.imagenet_utils import decode_predictions
from tensorflow.keras.models import load_model

app = FastAPI()

input_shape = (224, 224)

# Model is placeholder
save_folder = "../data/saved"
model_name = "effnetalpha"
model = load_model(f"{save_folder}/{model_name}")

def read_image(file):
    image = Image.open(BytesIO(file))
    return image
    
# Image preprocessing method is placeholder too
def preprocess(image: Image.Image):
    image = image.resize(input_shape)
    image = np.asfarray(image)
    #image = image / 255
    image = np.expand_dims(image, 0)
    return image

# Prediction also placeholder
def predict(image: np.ndarray):
    pred = model.predict(image)
    if pred[0][0] > pred[0][1]:
        infer = "With Mask"
    else:
        infer = "Without Mask"
    return infer

# Actual API for the inference 
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

if __name__ == "__main__":
    uvicorn.run(app, port = 8000, host = "127.0.0.1")