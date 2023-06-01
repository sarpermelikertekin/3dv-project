using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;
using UnityEngine.UI;
using System.Net.Sockets;
using System;
using System.Net;
using TMPro;

public class HololensCameraCapture : MonoBehaviour
{
    PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;

    public RawImage rawImage;

    public GameObject InputPanel;
    public GameObject DebugPanel;

    public TextMeshProUGUI ipInput;
    public TextMeshProUGUI portInputField;

    public bool sendOnlyImage;
    private TcpClient client;
    private UdpClient client_u;

    private IPEndPoint endPoint;
    private NetworkStream stream;
    private bool isWaitingForResponse = false;
    private string response;

    //string host = "127.0.0.1";
    //string port = "6000";
    public string host;
    public string port;

    byte[] imageBytes;

    public const int BufferSize = 400000; // you may need to adjust this size
    private byte[] imageBuffer;
    private int imageBufferOffset = 0;

    private int accumulator = 0;
    private int tour = 0;

    // Use this for initialization
    void Start()
    {

    }

    public void AdjustUI()
    {
        host = ipInput.text;
        port = portInputField.text;

        InputPanel.SetActive(false);
        DebugPanel.SetActive(true);

        StartCapturing();
    }

    public int FindNumberInString(string inputString)
    {
        int number = 0;
        int multiplier = 1;

        for (int i = inputString.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(inputString[i]))
            {
                int.TryParse(inputString[i].ToString(), out int parsedNumber);
                number += parsedNumber * multiplier;
                multiplier *= 10;
            }
        }

        return number;
    }

    public string FindIPv4InString(string inputString)
    {
        string currentNumber = "";
        int partCount = 0;
        string ipAddress = "";

        for (int i = 0; i < inputString.Length; i++)
        {
            if (char.IsDigit(inputString[i]))
            {
                currentNumber += inputString[i];
            }
            else if (inputString[i] == '.' || i == inputString.Length - 1)
            {
                if (currentNumber != "" && int.Parse(currentNumber) <= 255)
                {
                    ipAddress += currentNumber + ".";
                    partCount++;
                    currentNumber = "";
                }
                else
                {
                    ipAddress = "";
                    partCount = 0;
                }
            }
            else
            {
                currentNumber = "";
                ipAddress = "";
                partCount = 0;
            }

            // If we have 4 valid parts, we have a valid IP address.
            if (partCount == 4)
            {
                break;
            }
        }

        // If we found a valid IP address, remove the trailing '.' and return it.
        if (partCount == 4)
        {
            return ipAddress.TrimEnd('.');
        }

        // No valid IP address found.
        return null;
    }

    public void StartCapturing()
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
                Debug.Log("Picture Taken");
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
        imageBytes = targetTexture.EncodeToPNG();

        if (client == null)
        {
            //client = new TcpClient(FindIPv4InString(host), FindNumberInString(port));
            client = new TcpClient("172.20.10.2", 5000);
            stream = client.GetStream();
        }

        int frameSize = imageBytes.Length;
        Debug.Log("received bytes: "+frameSize);
        byte[] sizeBytes = new byte[4];
        for(int i = 0; i<4; i++){
            sizeBytes[i] = (byte)(frameSize>>((3-i)*8));
        }
        stream.Write(sizeBytes, 0, sizeBytes.Length);
        stream.Write(imageBytes, 0, imageBytes.Length);

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
                Debug.Log("Response received: " + response);
                
            }
            yield return null;
        }
        
        isWaitingForResponse = false;
        Debug.Log("Response received: " + response);
        yield return null;
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