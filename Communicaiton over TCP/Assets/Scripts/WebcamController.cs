using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class WebcamController : MonoBehaviour
{
    string ipAddress = "10.5.176.228"; // Replace with the IP address of the Python server
    int port = 6000; // Replace with a port number

    private WebCamTexture webcamTexture;

    void Start()
    {
        // Get the default webcam and start it
        var webcamDevices = WebCamTexture.devices;
        if (webcamDevices.Length > 0)
        {
            var webcamDevice = webcamDevices[0];
            webcamTexture = new WebCamTexture(webcamDevice.name, 640, 480, 30);
            webcamTexture.Play();
        }
    }

    void Update()
    {
        // Capture a frame from the webcam texture and send it to the Python server
        if (webcamTexture != null)
        {
            Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
            texture.SetPixels(webcamTexture.GetPixels());
            texture.Apply();
            byte[] imageData = texture.GetRawTextureData();
            var imageWidth = webcamTexture.width;
            var imageHeight = webcamTexture.height;
            var imageBytes = new byte[imageData.Length + 8];
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(imageWidth), 0, imageBytes, 0, 4);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(imageHeight), 0, imageBytes, 4, 4);
            System.Buffer.BlockCopy(imageData, 0, imageBytes, 8, imageData.Length);

            try
            {
                // Send the image data to the Python server
                var client = new TcpClient(ipAddress, port);
                var stream = client.GetStream();
                stream.Write(imageBytes, 0, imageBytes.Length);
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Debug.Log("SocketException: " + e);
            }
        }
    }
}