using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WebcamController : MonoBehaviour
{
    public RawImage rawImage;
    public bool sendOnlyImage;

    private WebCamTexture webcamTexture;
    private Texture2D texture2D;

    public string host = "127.0.0.1";
    public int port = 5000;

    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        rawImage.material.mainTexture = webcamTexture;
        webcamTexture.Play();

        texture2D = new Texture2D(webcamTexture.width, webcamTexture.height);

        client = new TcpClient(host, port);
        stream = client.GetStream();

        InvokeRepeating("CaptureAndSendData", 1.0f, 1.0f);
    }

    void CaptureAndSendData()
    {
        texture2D.SetPixels(webcamTexture.GetPixels());
        texture2D.Apply();

        // Encode the texture as a PNG image
        byte[] imageBytes = texture2D.EncodeToPNG();

        // Prepend the length of the image data as a 4-byte integer
        byte[] lengthBytes = BitConverter.GetBytes(imageBytes.Length);
        byte[] sendBytes = new byte[lengthBytes.Length + imageBytes.Length];
        lengthBytes.CopyTo(sendBytes, 0);
        imageBytes.CopyTo(sendBytes, lengthBytes.Length);

        // Send the length-prefixed image data over the network
        stream.Write(sendBytes, 0, sendBytes.Length);

        Debug.Log("Image sent. Length: " + imageBytes.Length);

        // Wait for a response from the server
        byte[] responseLengthBytes = new byte[sizeof(int)];
        int bytesRead = stream.Read(responseLengthBytes, 0, responseLengthBytes.Length);
        if (bytesRead == responseLengthBytes.Length)
        {
            int responseLength = BitConverter.ToInt32(responseLengthBytes, 0);
            byte[] responseBytes = new byte[responseLength];
            bytesRead = 0;
            while (bytesRead < responseLength)
            {
                bytesRead += stream.Read(responseBytes, bytesRead, responseLength - bytesRead);
            }
            string responseString = Encoding.UTF8.GetString(responseBytes);
            Debug.Log("Response received. Length: " + responseLength + ", Message: " + responseString);
        }
        else
        {
            Debug.Log("Error receiving response from server");
        }
    }

    void OnApplicationQuit()
    {
        stream.Close();
        client.Close();
    }
}