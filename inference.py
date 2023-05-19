
import os
import torch
import cv2
import numpy as np



#image_paths = ['./image'+str(i)+'.png' for i in range(1,7)]
weights_path = './training results/exp_14052023_1411/weights/best.pt'
image_path = './real_images/image3.png'
result_path = './yolov5/runs/detect/exp/labels/image.txt'
command = "python3 ./yolov5/detect.py --weight " + weights_path + " --source "+image_path + " --save-txt"

command_to_paste = "python3 yolov5/detect.py --weight training\ results/exp_15052023_2206/weights/best.pt --source real_images/image3.png --save-txt"


model = torch.hub.load('ultralytics/yolov5', 'custom', weights_path)

#a function that listens the image necessary
def infer():
    

    # code to listen the images and save to imagepath
    results = model(image_path)
    objects = results.pandas().xyxy  #this is a dataframe
    print(objects)
    print()

    #code to send the results

    return str(objects) 

def detect():
    os.system(command)

detect()