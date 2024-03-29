using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Linq;

public class Game : MonoBehaviour
{
    //States
    public bool waitForServer = false;
    public GameObject chesspiece;
    private GameObject[,] board = new GameObject[8, 8];
    Client client;
    private bool whiteTurn = true;
    private bool gameOver = false;

    public bool isWhiteMoving = false;
    public bool isBlackMoving = false;
    public void Start()
    {
        client = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Client>();
        BuildBoard(client.GetFEN);
    }
    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            Restart(true);
        }
    }
    public void Restart(bool isEndOfGame = false)
    {
        gameOver = false;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = false;
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = false;
        string msg = client.SendAndWaitForResponce("4_" + (isEndOfGame ? "end of game" : "restart"));
        BuildBoard(msg.Split('_')[2]);
    }
    public string GetBoard()
    {
        string fen = "";
        int countEmpty = 0;
        for (int y = 7; y >= 0; y--)
        {
            countEmpty = 0;
            for (int x = 0; x < 8; x++)
            {
                if (board[x, y] != null)
                {
                    if (countEmpty != 0)
                    {
                        fen += countEmpty;
                        countEmpty = 0;
                    }
                    fen += GetCharFromPType(board[x, y].name);
                }
                else
                    countEmpty++;
            }
            if (countEmpty > 0)
                fen += countEmpty;
            fen += '/';
        }
        return fen.Substring(0, fen.Length - 1);
    }
    public void BuildBoard(string fen)
    {
        fen = string.Concat(fen.Where(c => !char.IsWhiteSpace(c)));
        foreach (GameObject go in board)
            Destroy(go);
        foreach (GameObject movePlate in GameObject.FindGameObjectsWithTag("MovePlate"))
            Destroy(movePlate);
        int y = 7;
        int x = 0;
        foreach (char ch in fen)
        {
            if (ch == '/')
            {
                y--;
                x = 0;
            }
            else if (ch - '0' >= 0 && ch - '0' < 9)
            {
                x += ch - '0';
            }
            else
            {
                if (ch != ' ')
                { 
                    //Add piece
                    string ch2 = ch.ToString();
                    string name; bool isBlack;
                    (name, isBlack) = GetPTypeFromChar(ch);
                    name = (isBlack ? "black" : "white") + name;
                    SetPosition(Create(name, new Point(x, y)));
                    x++;
                }
            }
        }
    }
    private char GetCharFromPType(string name)
    {
        string piece = name.Substring(5, name.Length - 5);
        char ch = (char)0;
        if (name.StartsWith("white"))
            ch = (char)(ch + 'A' - 'a');
        switch (piece)
        {
            case "King": return (char)(ch + 'k');
            case "Queen": return (char)(ch + 'q');
            case "Bishop": return (char)(ch + 'b');
            case "Knight": return (char)(ch + 'n');
            case "Rook": return (char)(ch + 'r');
            case "Pawn": return (char)(ch + 'p');
        }
        return ch;
    }
    private (string, bool) GetPTypeFromChar(char ch)
    {
        //(PType, isBlack)
        bool isBlack = true;
        if (ch <= 'Z' && ch >= 'A')
        {
            isBlack = false;
            ch = (char)(ch - 'A' + 'a');
        }
        switch (ch)
        {
            case 'k':
                return ("King", isBlack);
            case 'q':
                return ("Queen", isBlack);
            case 'r':
                return ("Rook", isBlack);
            case 'b':
                return ("Bishop", isBlack);
            case 'n':
                return ("Knight", isBlack);
            case 'p':
                return ("Pawn", isBlack);
            default:
                return ("Pawn", isBlack);
        }
    }
    public GameObject Create(string name, Point p)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -2f), Quaternion.identity);
        Piece piece = obj.GetComponent<Piece>();
        piece.name = name;
        piece.SetPBoard(p);
        piece.Activate();
        return obj;
    }
    public void SetPosition(GameObject obj)
    {
        Piece piece = obj.GetComponent<Piece>();
        board[(int)piece.GetPBoard().x, (int)piece.GetPBoard().y] = obj;
    }
    public void SetEmptyPosition(Point p)
    {
        board[(int)p.x, (int)p.y] = null;
    }
    public void DestroyAllMovePlates()
    {
        //Destroy all the MovePlates on the screen
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
            Destroy(movePlates[i]);
    }
    public GameObject GetGameObjectOnPosition(Point p) => board[(int)p.x, (int)p.y];
    //is a postion on the board
    public bool IsPositionOnBoard(Point p)
    {
        return !(p.x < 0 || p.y < 0 || p.x >= board.GetLength(0) || p.y >= board.GetLength(1));
    }
    public bool GetWhiteTurn() => whiteTurn;
    public bool IsGameOver() => gameOver;
    public void NextTurn()
    { 
        whiteTurn = !whiteTurn;
    }
    public void Winner(string playerWinner)
    {
        gameOver = true;

        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner!";
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }
    public bool MakeMove(Move m)
    {
        //returns if successful
        try
        {
            GameObject pieceRefrence = GetGameObjectOnPosition(m.GetStartPoint());
            Piece piece = pieceRefrence.GetComponent<Piece>();

            GameObject enemyChessPiece = GetGameObjectOnPosition(m.GetTargetPoint());
            if (enemyChessPiece != null)
            {
                if (enemyChessPiece.name == "whiteKing")
                    Winner("black");
                if (enemyChessPiece.name == "blackKing")
                    Winner("white");
                Destroy(enemyChessPiece);
            }
            SetEmptyPosition(m.GetStartPoint());
            piece.SetPBoard(new Point(m.GetTargetPoint()));
            SetPosition(pieceRefrence);
            piece.MoveToTarget();
            return true;
        }
        catch
        {
            return false;
        }
    }
    public void ReturnToManu()
    {
        client.SendMsg("7_close");
        client.SetWaitForServer(false);
        Destroy(GameObject.FindGameObjectWithTag("NetworkManager"));
        SceneManager.LoadScene("EnterIP");
    }
}
