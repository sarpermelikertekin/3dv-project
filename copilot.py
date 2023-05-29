import socket
import struct
import cv2
import numpy as np

def receive_images():
    # Create a TCP socket
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    # Set the IP address and port the server will listen on
    server_ip = ''  # Replace with the server's IP address
    server_port = 5500  # Replace with the desired port number

    # Bind the socket to the IP address and port
    server_socket.bind((server_ip, server_port))

    # Listen for incoming connections
    server_socket.listen(1)
    print("Server listening on {}:{}".format(server_ip, server_port))

    # Accept a client connection
    client_socket, client_address = server_socket.accept()
    print("Client connected:", client_address)

    while True:
        # Receive the length of the image bytes
        length_bytes = client_socket.recv(4)
        length = struct.unpack('!I', length_bytes)[0]

        # Receive the image bytes
        image_bytes = b''
        remaining_bytes = length
        while remaining_bytes > 0:
            data = client_socket.recv(remaining_bytes)
            image_bytes += data
            remaining_bytes -= len(data)
            print("data: ", len(data))
            print("remaining_bytes: ", remaining_bytes)
            client_socket.send("send more".encode())

        # Convert the image bytes to a NumPy array
        image_np = np.frombuffer(image_bytes, dtype=np.uint8)

        # Decode the image array using OpenCV
        image = cv2.imdecode(image_np, cv2.IMREAD_COLOR)

        # Do whatever processing you need with the image
        cv2.imshow('Received Image', image)
        cv2.waitKey(1)  # Adjust as needed

    # Close the socket
    
    client_socket.close()
    server_socket.close()

receive_images()

