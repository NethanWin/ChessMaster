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
        //DBManager db = new DBManager();
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
        DBManager db = new DBManager();

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
            threads.Add(new Thread(() => HandleClient(handler, db)) { Name = "t" + id });
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
    public static void HandleClient(Socket clientSocket, DBManager db)
    {
        int userID = 0; // to get
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
                    string msgToSend = "";
                    try
                    {
                        string[] arr = data.Split('_');
                        if (arr[0] == "1")
                        {
                            Move m = new Move(arr[1], arr[2]);
                            db.AddMove(m, userID);
                            board.MakeMove(m);
                            Move bestMove = Ai.GetBestMove(board);
                            board.MakeMove(bestMove);
                            msgToSend = string.Format("1_{0}", bestMove.ToString());
                        }
                        else if (arr[0] == "2")
                        {
                            int tempId = db.GetUserID(arr[1], arr[2]);
                            if (tempId == -1 || tempId == 0)
                                msgToSend = "10_wrong user or password";
                            else
                            {
                                userID = tempId;
                                msgToSend = "9_ok";
                            }
                        }
                        else if (arr[0] == "3")
                        {
                            db.CreateUser(arr[1], arr[2]);
                            int tempId = db.GetUserID(arr[1], arr[2]);
                            if (tempId == -1 || tempId == 0)
                            {
                                msgToSend = "8_user taken";
                            }
                            else
                            {
                                userID = tempId;
                                msgToSend = "7_ok";
                            }
                        }
                        else
                            msgToSend = "11_msg not in format";
                    }
                    catch
                    {
                        msgToSend = "11_msg not in format";
                    }
                    finally
                    {
                        clientSocket.Send(Encoding.ASCII.GetBytes(msgToSend));
                    }
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
