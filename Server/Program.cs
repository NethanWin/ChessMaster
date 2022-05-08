//ChessMaster Server
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Net.Sockets;

class Program
{
    public const int SERVER_PORT = 11000;
    public static void Main(string[] args)
    {
        ManagingClients();
        //MinmaxPerformanceTest();
        //TestEvaluation();
    }
    public static IPAddress GetHostsIP()
    {
        //returns the IP of the host's internal IP
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
            if (ip.AddressFamily.ToString() == "InterNetwork")
                return ip;
        return null;
    }
    public static void ManagingClients()
    {
        //main command which handles new clients and creates thread for each
        List<Thread> threads = new List<Thread>();
        
        //setup Host
        IPAddress ipAddress = GetHostsIP();
        Console.WriteLine("Welcome to ChessMaster!");
        Console.WriteLine("The server is now open for clients");
        Console.WriteLine();
        Console.WriteLine("Please copy this text to the client's IP field:");
        Console.WriteLine(ipAddress.ToString());
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, SERVER_PORT);

        // Create a Socket that will use Tcp protocol
        Socket serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(localEndPoint);
        int id = 0;
        while (true)
        {
            serverSocket.Listen(10);
            Socket handler = serverSocket.Accept();
            RemoveFinishedThreads(threads);
            threads.Add(new Thread(() => HandleClient(handler)) { Name = "t" + id });
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
        AiBoard board = new AiBoard();
        try
        {
            //handle sockets for each client
            string data = null;
            byte[] bytes = null;
            byte[] msg;
            while (true)
            {
                bytes = new byte[1024];
                if (data != "")
                {
                    int bytesRec = clientSocket.Receive(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    Console.WriteLine("{0}: {1}", Thread.CurrentThread.Name, data);

                    msg = Encoding.ASCII.GetBytes(AnalizingMsg(data, board));
                    clientSocket.Send(msg);
                }
                Thread.Sleep(100);
            }
        }
        catch 
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            Console.WriteLine("close");
        }
    }
    private static string AnalizingMsg(string msg, AiBoard board)
    {
        try
        {
            string[] arr = msg.Split('_');
            if (arr[0] == "1")
            {
                Move m = new Move(arr[1], arr[2]);
                board.MakeMove(m);
                Move bestMove = Ai.GetBestMove(board);
                board.MakeMove(bestMove);
                return "1_" + bestMove.ToString();
            }
            return "11_msg not in format";
        }
        catch
        {
            return "11_disconected because timeout";
        }
    }
    
    //Testing
    public static void MinmaxPerformanceTest()
    {
        //Board b = new Board();
        //Move m = new Move(new Point(0,0), new Point(0,2));
        //Move m2 = new Move(new Point(0,1), new Point(0,3));
        Move m;
        Stopwatch stopwatch = new Stopwatch();
        AiBoard b = new AiBoard();

        //Ai.Minmax(new Board(), 4, false, true);

        for (int i = 0; i < 20; i++)
        {
            stopwatch.Start();
            m = Ai.GetBestMove(b);
            stopwatch.Stop();
            Console.WriteLine();
            
            b.MakeMove(m);
            
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0);
            stopwatch.Reset();
        }
        

        
    }
    public static void TestEvaluation()
    {
        AiBoard b = new AiBoard("8/pppp4/8/8/8/8/8/8");
        Console.WriteLine(b.EvaluateBoard(true));
    }
}
