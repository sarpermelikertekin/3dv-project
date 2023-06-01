import socket
import sys
import cv2
import pickle
import numpy as np
import struct ## new
import zlib
import time



def receive_frame(client_socket, frame_size):
    frame_data = b""
    bytes_received = 0

    print("frame size: ", frame_size)

    while bytes_received < frame_size:
        try:
            chunk = client_socket.recv(min(frame_size - bytes_received, 8388608))
            if not chunk:
                break
            frame_data += chunk
            bytes_received += len(chunk)

        except:
            return frame_data

    return frame_data


def main():
    server_ip = ''  # Change this to your server IP
    server_port = 5000  # Change this to your server port

    # Create a TCP socket server
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((server_ip, server_port))
    server_socket.listen(1)

    print('Waiting for client connection...')
    client_socket, address = server_socket.accept()
    print('Client connected:', address)

    while True:
       
        size_data = client_socket.recv(4)
        frame_size = struct.unpack('!I', size_data)[0]

        # Receive the frame from the client
        frame_data = receive_frame(client_socket, frame_size)

        # Process the received frame as needed
        with open('received_frame.jpg', 'wb') as f:
            f.write(frame_data)

        print('Frame received successfully!')

        # Close the client socket
        client_socket.send("Image received".encode())
        #print('sleep')
        #time.sleep(0.1)
    
    
    client_socket.close()

    # Close the server socket
    server_socket.close()


main()