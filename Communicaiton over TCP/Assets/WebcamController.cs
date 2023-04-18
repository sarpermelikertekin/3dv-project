using System;
using System.Net.Sockets;
using System.Collections;
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

    private bool isWaitingForResponse = false;
    private string response;

    void Start()
    {
        webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        rawImage.material.mainTexture = webcamTexture;
        webcamTexture.Play();

        texture2D = new Texture2D(webcamTexture.width, webcamTexture.height);

        client = new TcpClient(host, port);
        stream = client.GetStream();

        StartCoroutine(CaptureAndSendImage());
    }

    IEnumerator CaptureAndSendImage()
    {
        while (true)
        {
            if (!isWaitingForResponse)
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
                isWaitingForResponse = true;
                yield return StartCoroutine(WaitForResponse());
            }
            yield return null;
        }
    }

    IEnumerator WaitForResponse()
    {
        byte[] responseBytes = new byte[1024];
        int bytesRead = 0;
        while (bytesRead == 0)
        {
            if (stream.DataAvailable)
            {
                bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                response = System.Text.Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
            }
            yield return null;
        }
        isWaitingForResponse = false;
        Debug.Log("Response received: " + response);
    }

    void OnApplicationQuit()
    {
        stream.Close();
        client.Close();
    }
}