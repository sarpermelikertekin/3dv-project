using UnityEngine;
using System.Net.Sockets;
using System;
using System.IO;

public class CameraStreamer : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] cameraData;

    private void Start()
    {
        // Connect to Python script on PC
        string ipAddress = "127.0.0.1"; // Replace with PC's IP address
        int port = 12345; // Replace with desired port number
        client = new TcpClient(ipAddress, port);
        stream = client.GetStream();
    }

    private void Update()
    {
        // Get camera feed data
        cameraData = GetCameraFeed();

        // Send camera feed data to Python script
        SendData(cameraData);
    }

    private byte[] GetCameraFeed()
    {
        // Set up camera texture
        int cameraWidth = 640;
        int cameraHeight = 360;
        WebCamTexture webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name, cameraWidth, cameraHeight, 30);
        webcamTexture.Play();

        // Capture camera feed texture
        var cameraTexture = new Texture2D(cameraWidth, cameraHeight, TextureFormat.RGB24, false);
        cameraTexture.SetPixels(webcamTexture.GetPixels());
        cameraTexture.Apply();

        // Encode camera feed texture as JPEG image
        byte[] data = cameraTexture.EncodeToJPG();

        // Clean up
        webcamTexture.Stop();

        return data;
    }

    private void SendData(byte[] data)
    {
        try
        {
            // Send camera feed data to Python
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending data: {e}");
        }
    }

    private void OnApplicationQuit()
    {
        // Close TCP connection
        stream.Close();
        client.Close();
    }
}