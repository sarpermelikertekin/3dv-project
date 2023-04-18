using System.Net.Sockets;
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

        InvokeRepeating("CaptureAndSendImage", 1.0f, 1.0f);
    }

    void CaptureAndSendImage()
    {
        texture2D.SetPixels(webcamTexture.GetPixels());
        texture2D.Apply();

        if (sendOnlyImage)
        {
            // Encode the texture as a PNG image
            byte[] imageBytes = texture2D.EncodeToPNG();

            // Send the image data over the network
            stream.Write(imageBytes, 0, imageBytes.Length);
        }
        else
        {
            rawImage.texture = texture2D;
        }

        Debug.Log("Image sent. Length: " + texture2D.EncodeToPNG().Length);
    }

    void OnApplicationQuit()
    {
        stream.Close();
        client.Close();
    }
}