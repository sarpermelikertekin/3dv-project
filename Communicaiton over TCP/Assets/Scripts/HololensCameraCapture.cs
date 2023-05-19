using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;
using UnityEngine.UI;
using System.Net.Sockets;
using System;
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
    private NetworkStream stream;
    private bool isWaitingForResponse = false;
    private string response;

    //string host = "127.0.0.1";
    //string port = "6000";
    public string host;
    public string port;

    byte[] imageBytes;

    public const int BufferSize = 16384; // you may need to adjust this size
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

        // Segment image into chunks and send them
        for (int i = 0; i < imageBytes.Length; i += BufferSize)
        {
            tour += 1;

            // Calculate chunk size (last chunk can be smaller)
            int chunkSize = Mathf.Min(BufferSize, imageBytes.Length - i);

            Debug.Log(tour);
            Debug.Log("Rest : " + (imageBytes.Length - i).ToString());
            Debug.Log("Chunk Size : " + chunkSize.ToString());

            imageBuffer = new byte[chunkSize];

            // Copy chunk into buffer
            Array.Copy(imageBytes, i, imageBuffer, 0, chunkSize);
            imageBufferOffset = chunkSize;

            // Send buffer
            SendBuffer();

            // Wait for a response from the server
            isWaitingForResponse = true;
            StartCoroutine(WaitForResponse());
        }
    }

    private void SendBuffer()
    {
        // Prepend the size of the package (i.e., imageBufferOffset)
        byte[] packageSizeBytes = BitConverter.GetBytes(imageBufferOffset);

        // Prepend the size of the entire image
        byte[] imageSizeBytes = BitConverter.GetBytes(imageBytes.Length);

        byte[] sendBytes = new byte[packageSizeBytes.Length + imageSizeBytes.Length + imageBufferOffset];
        packageSizeBytes.CopyTo(sendBytes, 0);
        imageSizeBytes.CopyTo(sendBytes, packageSizeBytes.Length);
        imageBuffer.CopyTo(sendBytes, packageSizeBytes.Length + imageSizeBytes.Length);

        if (client == null)
        {
            client = new TcpClient(FindIPv4InString(host), FindNumberInString(port));
            stream = client.GetStream();
        }

        // Send the length-prefixed image data over the network
        stream.Write(sendBytes, 0, sendBytes.Length);

        accumulator += BufferSize;
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

                // Check if server wants more data
                if (response == "send more")
                {
                    // Reset buffer offset
                    imageBufferOffset = 0;

                    // Send the next chunk
                    SendBuffer();
                }
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