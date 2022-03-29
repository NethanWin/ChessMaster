using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public class Server
{
    public const int SERVER_PORT = 11000;
    public static void ManagingClients()
    {
        //main command which handles new clients and creates thread for each
        List<Thread> threads = new List<Thread>();
        Console.WriteLine("server");
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, SERVER_PORT);
        // Create a Socket that will use Tcp protocol
        Socket serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        // A Socket must be associated with an endpoint using the Bind method
        serverSocket.Bind(localEndPoint);
        int id = 0;
        while (true)
        {
            // Specify how many requests a Socket can listen before it gives Server busy response.
            // We will listen 10 requests at a time
            serverSocket.Listen(10);
            Socket handler = serverSocket.Accept();

            RemoveFinishedThreads(threads);

            //threads.RemoveAll(item => item == null);
            threads.Add(new Thread(() => HandleClient(handler)) { Name = "t" + id});
            threads.Last().Start();
            id++;
        }
    }
    private static void RemoveFinishedThreads(List<Thread> threads)
    {
        List<Thread> deleteThreads = new List<Thread>();
        if (threads.Count > 0)
        {
            foreach (Thread t in threads)
                if (!t.IsAlive)
                  deleteThreads.Add(t);

            foreach (Thread t in deleteThreads)
                threads.Remove(t);
        }
    }
    public static void HandleClient(Socket clientSocket)
    {
        try
        {
            //handle sockets for each client
            // Incoming data from the client.
            string data = null;
            byte[] bytes = null;
            byte[] msg;
            while (data != "exit")
            {
                bytes = new byte[1024];
                if (data != "")
                {
                    int bytesRec = clientSocket.Receive(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    Console.WriteLine("{0}: {1}", Thread.CurrentThread.Name, data);
                }
                msg = Encoding.ASCII.GetBytes(data);
                clientSocket.Send(msg);
                Thread.Sleep(100);
            }
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
        catch (Exception e)
        {
            //Console.WriteLine(e.ToString());
        }
        Console.WriteLine("close");
    }

    public static void Main(string[] args)
    {
        ManagingClients();
    }
}
