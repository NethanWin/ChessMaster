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

//ChessMaster
// Client app is the one sending messages to a Server/listener.
// Both listener and client can send messages back and forth once a
// communication is established.
public class Client : MonoBehaviour
{
    static string ipStr;
    static bool waitForServer;
    static Socket socket;
    static Game game;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //game = GetComponent<Game>();
    }
    public bool StartClient(string ip)
    {
        //returns if succesful
        // Connect to a Remote server
        ipStr = ip;
        try
        {
            ipStr = ipStr.Substring(0, ipStr.Length - 1);

            IPAddress ipAddress = IPAddress.Parse(ipStr);
            Debug.Log(ipAddress.ToString());
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
            socket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            
            //Connect the socket to the server
            socket.Connect(remoteEP);
            //SendMessage("0_attempt to connect");
            return SocketConnected(socket);
        }
        catch
        {
            return false;
        }
    }
    static bool SocketConnected(Socket s)
    {
        bool part1 = s.Poll(1000, SelectMode.SelectRead);
        bool part2 = (s.Available == 0);
        if (part1 && part2)
            return false;
        else
            return true;
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
            StartClient(ipStr);
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
        if (SceneManager.GetActiveScene().name == "EnterIP")
            return;

        byte[] bytes = new byte[1024];
        if (waitForServer)
            try
            {
                // Receive the response from the remote device.
                int bytesRec = socket.Receive(bytes);
                string str = Encoding.ASCII.GetString(bytes);
                str = CleanString(str);
                if (str != "")
                {
                    HandleResponse(str);
                    waitForServer = false;
                }
            }
            catch (Exception) { }
    }
    public bool HandleResponse(string str)
    {
        //returns if succusful
        try
        {
            string[] arr = str.Split('_');
            if (arr[0] == "1")
            {
                Move m = new Move(arr[1], arr[2]);
                game.MakeMove(m);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
