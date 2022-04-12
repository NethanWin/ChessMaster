using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Client app is the one sending messages to a Server/listener.
// Both listener and client can send messages back and forth once a
// communication is established.
public class Client : MonoBehaviour
{
    int count = 0;
    static bool waitForServer;
    static Socket socket;
    static Game game;
    void Start()
    {
        game = GetComponent<Game>();
        StartClient();
    }
    public void StartClient()
    {
        // Connect to a Remote server
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
        socket = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        try
        {
            //Connect the socket to the server
            socket.Connect(remoteEP);
        }
        finally {}
    }
    public void CloseClient()
    {
        // Release the socket.
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
    public void SendMsg(string msg)
    {
        try
        {
            socket.Send(Encoding.ASCII.GetBytes(msg));
            waitForServer = true;
        }
        catch (Exception ex)
        {
            CloseClient();
            StartClient();
        }
    }
    public string CleanString(string str)
    {
        string newFen = "";
        foreach (char ch in str)
        {
            if ((int)ch == 0)
                return newFen;
            newFen += ch;
        }
        return newFen;
    }
    public void Update()
    {
        byte[] bytes = new byte[1024];
        if (waitForServer)
            try
            {
                // Receive the response from the remote device.
                int bytesRec = socket.Receive(bytes);
                string fen = Encoding.ASCII.GetString(bytes);
                fen = CleanString(fen);
                Debug.Log("recived len" + fen.Length);
                if (fen != "")
                {
                    game.BuildBoard(fen);//, 0, bytesRec));
                    waitForServer = false;
                }
            }
            catch (Exception e){}
    }
}
