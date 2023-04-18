using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;

public class WebcamController : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    public RawImage rawImage;

    public bool sendOnlyImage;
    private Texture2D textureToSend;

    // TCP communication variables
    private TcpListener listener;
    private TcpClient client;
    private NetworkStream stream;
    private bool isTcpConnected = false;

    // Start is called before the first frame update
    void Start()
    {
        // get the default webcam texture
        webcamTexture = new WebCamTexture();

        // start the camera
        webcamTexture.Play();

        // set the texture on the rawimage
        rawImage.texture = webcamTexture;

        // initialize the TCP communication
        listener = new TcpListener(IPAddress.Any, 9999);
        listener.Start();
        Debug.Log("TCP server started");

        // start listening for incoming connections
        Thread listenThread = new Thread(new ThreadStart(ListenForClient));
        listenThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // capture a frame every 1 second and send it to python
        if (sendOnlyImage)
        {
            if (textureToSend == null)
            {
                textureToSend = new Texture2D(webcamTexture.width, webcamTexture.height);
            }

            textureToSend.SetPixels(webcamTexture.GetPixels());
            textureToSend.Apply();
            byte[] imageData = textureToSend.EncodeToJPG();
            Debug.Log("Sending image of size " + imageData.Length);

            if (isTcpConnected)
            {
                try
                {
                    stream.Write(imageData, 0, imageData.Length);
                }
                catch (Exception e)
                {
                    Debug.Log("Error sending image: " + e);
                }
            }
        }
    }

    void OnDestroy()
    {
        // release the resources
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }

        if (isTcpConnected)
        {
            stream.Close();
            client.Close();
            listener.Stop();
        }
    }

    private void ListenForClient()
    {
        try
        {
            client = listener.AcceptTcpClient();
            Debug.Log("TCP client connected");
            stream = client.GetStream();
            isTcpConnected = true;

            // start receiving messages from the client
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
        }
        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e);
            listener.Stop();
        }
    }

    private void ReceiveMessage()
    {
        byte[] data = new byte[4096];

        while (isTcpConnected)
        {
            try
            {
                // read the incoming message from the client
                int bytesReceived = stream.Read(data, 0, data.Length);
                Debug.Log("Received message of size " + bytesReceived);

                // set the boolean to indicate that only images should be sent
                sendOnlyImage = true;
            }
            catch (Exception e)
            {
                Debug.Log("Error receiving message: " + e);
                isTcpConnected = false;
            }
        }
    }
}