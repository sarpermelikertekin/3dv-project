using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;
using UnityEngine.UI;
using System.Net.Sockets;
using System;

public class HololensCameraCapture : MonoBehaviour
{
    PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;
    public RawImage rawImage;

    public bool sendOnlyImage;
    private TcpClient client;
    private NetworkStream stream;
    private bool isWaitingForResponse = false;
    private string response;

    string host = "192.168.247.229";
    int port = 6000;
    int segmentSize = 1024;  // Define the size of each segment

    void Start()
    {
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                StartCoroutine(CaptureAndSendImage());
            });
        });
    }

    IEnumerator CaptureAndSendImage()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (!isWaitingForResponse)
            {
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            }
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);
        rawImage.texture = targetTexture;
        rawImage.material.mainTexture = targetTexture;

        byte[] imageBytes = targetTexture.EncodeToPNG();

        if (client == null)
        {
            client = new TcpClient(host, port);
            stream = client.GetStream();
        }

        int offset = 0;
        while (offset < imageBytes.Length)
        {
            int size = Math.Min(segmentSize, imageBytes.Length - offset);
            byte[] segment = new byte[size];
            Array.Copy(imageBytes, offset, segment, 0, size);
            offset += size;

            byte[] lengthBytes = BitConverter.GetBytes(size);
            byte[] sendBytes = new byte[lengthBytes.Length + segment.Length];
            lengthBytes.CopyTo(sendBytes, 0);
            segment.CopyTo(sendBytes, lengthBytes.Length);

            stream.Write(sendBytes, 0, sendBytes.Length);

            Debug.Log("Segment sent. Length: " + segment.Length);

            isWaitingForResponse = true;
            StartCoroutine(WaitForResponse());
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
        if (response == "ACK")
        {
            isWaitingForResponse = false;
        }
        Debug.Log("Response received: " + response);
    }

    void OnApplicationQuit()
    {
        stream.Close();
        client.Close();
    }
}