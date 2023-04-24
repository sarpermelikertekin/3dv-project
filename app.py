from flask import Flask
import os
import torch
import cv2
import numpy as np
import socket
import struct
import time



app = Flask(__name__)

#image_paths = ['./image'+str(i)+'.png' for i in range(1,7)]
weights_path = './yolov5/best2.pt'
image_path = './image6.png'
result_path = './yolov5/runs/detect/exp/labels/image.txt'
command = "python3 ./yolov5/detect.py --weight " + weights_path + " --source "+image_path + " --save-txt"
del_command = "rm -r ./yolov5/runs/detect/exp"



model = torch.hub.load('ultralytics/yolov5', 'custom', weights_path)

#a function that listens the image necessary
@app.route('/') 
def home():
    

    # code to listen the images and save to imagepath
    results = model(image_path)
    objects = results.pandas().xyxy  #this is a dataframe
    print(objects)
    print()

    #code to send the results

    return "success"   



@app.route('/detect/')
def detect():
    os.system(command)
    f = open(result_path,'r')
    os.system(del_command)
    return f.read()



@app.route('/stream/') 
def run():
    

    HOST = 'localhost'
    PORT = 5000

    # Create a socket and bind it to the host and port
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT))
        s.listen(1)
        print(f"Waiting for a connection on {HOST}:{PORT}")

        # Accept a connection from a client
        conn, addr = s.accept()
        print(f"Connected to {addr}")

        # Loop to receive images from the client
        while True:
            # Receive the length-prefixed image data
            length_data = conn.recv(4)
            if not length_data:
                print("Connection closed by client")
                break
            length = struct.unpack("I", length_data)[0]
            print(f"Expected length: {length}")
            data = conn.recv(length)
            if not data:
                break

            # Decode the image data and display the image
            nparr = np.frombuffer(data, np.uint8)
            try:
                img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
                cv2.imwrite(image_path, img)
                print("Image received")
                #cv2.waitKey(1)
            except Exception as e:
                print("Error decoding image:", e)

            results = model(image_path)
            objects = results.pandas().xyxy  #this is a dataframe
            print("Image processed")
            print(str(objects))
                
                    
            # Send the length of the received image data back to the client
            conn.send(str(objects).encode())

            print("Response sent")
            time.sleep(0.5)
            
        # Release resources and close the connection
        cv2.destroyAllWindows()
        conn.close()
        print("Connection closed")





app.run(port=5000)

