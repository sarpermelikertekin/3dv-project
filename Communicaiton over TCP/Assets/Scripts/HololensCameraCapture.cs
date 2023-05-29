using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PythonClient : MonoBehaviour
{
    // Server IP address and port
    public string serverIP = "77.109.166.99";
    public int serverPort = 5200;

    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[4096];

    private void Start()
    {
        ConnectToServer();
    }

    private void OnDestroy()
    {
        DisconnectFromServer();
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, serverPort);
            stream = client.GetStream();

            // Begin receiving data from the server
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to the server: " + e.Message);
        }
    }

    private void DisconnectFromServer()
    {
        if (stream != null)
        {
            stream.Close();
            stream = null;
        }

        if (client != null)
        {
            client.Close();
            client = null;
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int bytesRead = stream.EndRead(result);
            if (bytesRead > 0)
            {
                // Convert received bytes to string
                string receivedData = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);

                // Process the received data here
                Debug.Log("Received data: " + receivedData);

                // Begin receiving data again
                stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
            }
            else
            {
                // Connection closed by the server
                Debug.Log("Server connection closed.");
                DisconnectFromServer();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
            DisconnectFromServer();
        }
    }
}
