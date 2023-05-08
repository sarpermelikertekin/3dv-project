using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;
using UnityEngine.Windows.WebCam;

#if UNITY_WSA_10_0 && NETFX_CORE
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Foundation;
using Windows.Graphics.Imaging;


public class HololensController : MonoBehaviour
{
    public RawImage rawImage;

    private VideoCapture videoCapture;
    private Texture2D texture2D;

    public string host = "127.0.0.1";
    public int port = 5000;

    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        texture2D = new Texture2D(1, 1);

        client = new TcpClient(host, port);
        stream = client.GetStream();

#if UNITY_WSA_10_0 && NETFX_CORE
        StartHololensCapture();
#endif

        InvokeRepeating("CaptureAndSendImage", 1.0f, 1.0f);
    }

    void StartHololensCapture()
    {
        Task.Run(async () =>
        {
            videoCapture = await VideoCapture.CreateAsync();

            if (videoCapture != null)
            {
                var resolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
                var framerate = VideoCapture.GetSupportedFrameRatesForResolution(resolution).OrderByDescending((fps) => fps).First();

                await videoCapture.StartAsync(resolution, framerate);

                Debug.Log($"Capture started: {resolution} {framerate}");
            }
            else
            {
                Debug.LogError("Failed to start capture");
            }
        });
    }

    void CaptureAndSendImage()
    {
        if (videoCapture == null)
        {
            return;
        }

        var frame = videoCapture?.GetSample();

        if (frame != null)
        {
            frame.CopyRawImageDataIntoTexture(texture2D.GetNativeTexturePtr());
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
        }
    }

    void OnApplicationQuit()
    {
        if (videoCapture != null)
        {
            videoCapture.StopAsync();
            videoCapture.Dispose();
        }

        stream.Close();
        client.Close();
    }
}

#endif