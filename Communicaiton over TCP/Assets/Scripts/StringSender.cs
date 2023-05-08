using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class StringSender : MonoBehaviour
{
    public string host = "127.0.0.1";
    public int port = 5000;
    public string message = "hello world";

    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        client = new TcpClient(host, port);
        stream = client.GetStream();
        SendString(message);
    }

    void SendString(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        stream.Write(messageBytes, 0, messageBytes.Length);
        Debug.Log("Message sent. Length: " + messageBytes.Length);
    }

    void OnApplicationQuit()
    {
        stream.Close();
        client.Close();
    }
}