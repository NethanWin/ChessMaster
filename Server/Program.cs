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
    public static Socket s = null;
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
        Dictionary<int, int> idThreades = new Dictionary<int, int>();
        //id, thread id
        Dictionary<int, Socket> threadIDSocket= new Dictionary<int, Socket>();
        //threadID, socket


        //setup Host
        IPAddress ipAddress = GetHostsIP();
        Console.WriteLine("Welcome to ChessMaster!");
        Console.WriteLine("The server is now open for clients");
        Console.WriteLine();
        Console.WriteLine("Please copy this text to the client's IP field:");
        Console.WriteLine(ipAddress.ToString());
        Console.WriteLine();
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, SERVER_PORT);

        // Create a Socket that will use Tcp protocol
        Socket serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(localEndPoint);
        int id = 0;
        while (true)
        {
            serverSocket.Listen(10);
            Socket handler = serverSocket.Accept();
            threads.Add(new Thread(() => HandleClient(handler, db, idThreades, threads, threadIDSocket)) { Name = "t" + id });
            threadIDSocket[id] = handler;
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
    public static void HandleClient(Socket clientSocket, DBManager db, Dictionary<int, int> idThreades, List<Thread> threads,Dictionary<int, Socket> threadIDSocket)
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
                            db.AddMove(bestMove, userID);
                            msgToSend = string.Format("1_{0}", bestMove.ToString());
                        }
                        else if (arr[0] == "2")
                        {
                            int tempId = db.GetUserID(arr[1], arr[2]);
                            if (tempId == -1 || tempId == 0)
                            {
                                msgToSend = "10_wrong user or password";
                            }
                            else
                            {
                                userID = tempId;
                                ReplaceThreads(userID, Thread.CurrentThread.Name, idThreades, threads, threadIDSocket);
                                List<Move> moves = db.GetGameMoves(userID);
                                board = new AiBoard();
                                foreach (Move move in moves)
                                    board.MakeMove(move);
                                msgToSend = "9_ok_" + board.GetFen();
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
                        else if (arr[0] == "4")
                        {
                            string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
                            if (arr[1] != "restart")
                            {
                                //end of game
                                db.SetCurrentGameToLast(userID);
                            }
                            else
                            {
                                //reset button
                                db.ResetCurrentGame(userID);
                            }
                            board = new AiBoard();
                            msgToSend = "4_ok_" + FEN;
                        }
                        else if (arr[0] == "7")
                        {
                            threads.Remove(Thread.CurrentThread);
                           // Thread.CurrentThread.Abort();
                        }
                        else
                            msgToSend = "11_msg not in format";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
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
        catch (SocketException ex)
        {
            Console.WriteLine(ex);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
        finally 
        { 
            clientSocket.Close();
        }
    }
    public static void ReplaceThreads(int id, string currentThreadName, Dictionary<int, int> idThreades, List<Thread> threads, Dictionary<int, Socket> threadIDSocket)
    {
        //Remove threads with this id on db
        int threadInt;
        if (idThreades.TryGetValue(id, out threadInt))
        { 
            Thread removeThread = null;
            foreach (Thread t in threads)
            {
                if (t.Name == "t" + threadInt)
                {
                    removeThread = t; break;
                }
            }
            RemoveThread(removeThread, threads, threadIDSocket);
            if (removeThread != null)
            {
             /*   Console.WriteLine("closing: " + removeThread.Name);
                Socket socket = threadIDSocket[(int)removeThread.Name[1] - '0'];
                socket.Send(Encoding.ASCII.GetBytes("6_closed"));
                removeThread.Abort();
                threads.Remove(removeThread);*/
            }
        }
        idThreades[id] = currentThreadName[1] - '0';
    }
    public static void RemoveThread(Thread thread, List<Thread> threads, Dictionary<int, Socket> threadIDSocket)
    {
        if (thread == null)
            return;
        Console.WriteLine("closing: " + thread.Name);
        int threadID = (int)thread.Name[1] - '0';
        Socket socket = threadIDSocket[threadID];
        socket.Send(Encoding.ASCII.GetBytes("6_closed"));
        thread.Abort();
        threads.Remove(thread);
        threadIDSocket.Remove(threadID);
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
