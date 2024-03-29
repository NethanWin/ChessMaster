﻿using System;
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
    static string FEN;
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            waitForServer = false;
            game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        }
    }
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
        waitForServer = false;
    }
    public void Update()
    {
        byte[] bytes = new byte[1024];
        if (waitForServer)
            try
            {
                // Receive the response from the remote device
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
    public bool StartClient(string ip)
    {
        //returns if succesful
        // Connect to a Remote server
        ipStr = ip;
        try
        {
            ipStr = ipStr.Substring(0, ipStr.Length - 1);
            IPAddress ipAddress = IPAddress.Parse(ipStr);

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            socket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            //Connect the socket to the server
            bool isValidIP = IsValidIP(ipStr, 11000);
            if (isValidIP)
                socket.Connect(remoteEP);
            return isValidIP;
        }
        catch
        {
            return false;
        }
    }
    public static bool IsValidIP(string ip, int port)
    {
        try
        {
            //using (var client = new TcpClient(ip, port))
            using (var client = new TcpClient())
            {
                client.ReceiveTimeout = 1;
                client.SendTimeout = 1;
                client.Connect(ip, port);
                return true;
            }
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.ToString());
            return false;
        }
    }
    static bool SocketConnected(Socket s)
    {
        Debug.Log("test1");
        bool part1 = s.Poll(1000, SelectMode.SelectRead);
        Debug.Log("test2");
        bool part2 = (s.Available == 0);
        Debug.Log("test3");
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
        }
        catch
        {
            CloseClient();
            StartClient(ipStr);
        }
    }
    public void SetWaitForServer(bool isWaitForServer)
    {
        waitForServer = isWaitForServer;
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
    public string SendAndWaitForResponce(string msg)
    {
        waitForServer = false;
        SendMsg(msg);

        byte[] bytes = new byte[1024];
        int bytesRec = socket.Receive(bytes);
        string str = Encoding.ASCII.GetString(bytes);
        while (str == "")
        {
            bytes = new byte[1024];
            bytesRec = socket.Receive(bytes);
            str = Encoding.ASCII.GetString(bytes);
        }
        
        return CleanFEN(str);
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
            if (arr[0] == "6")
            {
                Destroy(GameObject.FindGameObjectWithTag("NetworkManager"));
                SceneManager.LoadScene("EnterIP");
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
    public bool Login(string username, string password)
    {
        //returns if found user and correct password (only both)
        string toSend = string.Format("2_{0}_{1}", username, password);
        string recivedStr = SendAndWaitForResponce(toSend);
        string[] arr = recivedStr.Split('_');
        string answerNumber = arr[0];
        if (answerNumber == "9")
        {
            FEN = arr[2];
            FEN = CleanFEN(FEN);
        }
        return answerNumber == "9";
    }
    public bool SignUp(string username, string password)
    {
        //returns if successful SignUp
        string toSend = string.Format("3_{0}_{1}", username, password);
        string recivedStr = SendAndWaitForResponce(toSend);
        Debug.Log(recivedStr);
        string answerNumber = recivedStr.Split('_')[0];
        return answerNumber == "7";
    }
    public string GetFEN => FEN;
    public static string CleanFEN(string str)
    {
        string temp = "";
        foreach (char ch in str)
        {
            if ((int)ch == 0)
                break;
            temp += ch;
        }
        return temp;
    }
}
