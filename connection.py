#from flask import Flask
import os
import torch
import cv2
import numpy as np
import socket
import struct
import time



def run():
    

    #HOST = 'localhost'
    HOST = ''
    PORT = 6000

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
            print(nparr.shape)
            
            try:
                img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
                #cv2.imshow("receive", img)
                cv2.imwrite(image_path, img, [int(cv2.IMWRITE_PNG_COMPRESSION),0])  
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
            #conn.send(str(0).encode())

            print("Response sent")
            time.sleep(1.0)
            
        # Release resources and close the connection
        cv2.destroyAllWindows()
        conn.close()
        print("Connection closed")





weights_path = './models/best6.pt'
image_path = './image6.png'
model = torch.hub.load('ultralytics/yolov5', 'custom', weights_path)

run()

