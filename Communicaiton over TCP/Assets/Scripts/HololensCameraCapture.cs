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

    public string host = "127.0.0.1";
    public int port = 5000;

    // Use this for initialization
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
        // Copy the raw image data into the target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        rawImage.texture = targetTexture;
        rawImage.material.mainTexture = targetTexture;

        // Encode the texture as a PNG image
        byte[] imageBytes = targetTexture.EncodeToPNG();

        // Prepend the length of the image data as a 4-byte integer
        byte[] lengthBytes = BitConverter.GetBytes(imageBytes.Length);
        byte[] sendBytes = new byte[lengthBytes.Length + imageBytes.Length];
        lengthBytes.CopyTo(sendBytes, 0);
        imageBytes.CopyTo(sendBytes, lengthBytes.Length);

        if (client == null)
        {
            client = new TcpClient(host, port);
            stream = client.GetStream();
        }

        // Send the length-prefixed image data over the network
        stream.Write(sendBytes, 0, sendBytes.Length);

        Debug.Log("Image sent. Length: " + imageBytes.Length);

        // Wait for a response from the server
        isWaitingForResponse = true;
        StartCoroutine(WaitForResponse());
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
        void OnApplicationQuit()
        {
            stream.Close();
            client.Close();
        }
    }
}