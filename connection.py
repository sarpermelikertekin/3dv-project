import socket
import sys

# Server IP address and port
server_ip = ''
server_port = 5200

# Create a TCP socket
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

try:
    # Bind the socket to the server IP address and port
    server_socket.bind((server_ip, server_port))

    # Listen for incoming connections
    server_socket.listen(1)
    print('Server listening on {}:{}'.format(server_ip, server_port))

    # Accept a client connection
    client_socket, client_address = server_socket.accept()
    print('Client connected:', client_address)

    # Receive the image data
    image_data = b''
    while True:
        data = client_socket.recv(4096)
        if not data:
            break
        image_data += data

    # Save the received image
    with open('received_image.png', 'wb') as file:
        file.write(image_data)
    print('Image received and saved as received_image.png')

except Exception as e:
    print('Error: {}'.format(e))

finally:
    # Close the client connection and the server socket
    client_socket.close()
    server_socket.close()
