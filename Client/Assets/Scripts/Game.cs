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
    private bool whiteTurn = true;
    private bool gameOver = false;

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
        fen = String.Concat(fen.Where(c => !Char.IsWhiteSpace(c)));
        foreach (GameObject go in board)
        {
            Destroy(go);
        }
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
                //Add piece
                //TODO
                string ch2 = ch.ToString();
                string name; bool isBlack;
                (name, isBlack) = GetPTypeFromChar(ch);
                if (isBlack)
                {
                    name = "black" + name;
                    SetPosition(Create(name, x, y));
                }
                else
                {
                    name = "white" + name;
                    SetPosition(Create(name, x, y));
                }
                x++;
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
    void Start()
    {
        GameObject[] playerWhite = new GameObject[] { Create("whiteRook", 0, 0), Create("whiteKnight", 1, 0),
            Create("whiteBishop", 2, 0), Create("whiteQueen", 3, 0), Create("whiteKing", 4, 0),
            Create("whiteBishop", 5, 0), Create("whiteKnight", 6, 0), Create("whiteRook", 7, 0),
            Create("whitePawn", 0, 1), Create("whitePawn", 1, 1), Create("whitePawn", 2, 1),
            Create("whitePawn", 3, 1), Create("whitePawn", 4, 1), Create("whitePawn", 5, 1),
            Create("whitePawn", 6, 1), Create("whitePawn", 7, 1)
        };
        
        GameObject[] playerBlack = new GameObject[] { Create("blackRook", 0, 7), Create("blackKnight",1,7),
            Create("blackBishop",2,7), Create("blackQueen",3,7), Create("blackKing",4,7),
            Create("blackBishop",5,7), Create("blackKnight",6,7), Create("blackRook",7,7),
            Create("blackPawn", 0, 6), Create("blackPawn", 1, 6), Create("blackPawn", 2, 6),
            Create("blackPawn", 3, 6), Create("blackPawn", 4, 6), Create("blackPawn", 5, 6),
            Create("blackPawn", 6, 6), Create("blackPawn", 7, 6) 
        };
        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }

    }
    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            SceneManager.LoadScene("Game");
        }
    }
    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        ChessMan cm = obj.GetComponent<ChessMan>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }
    public void SetPosition(GameObject obj)
    {
        ChessMan cm = obj.GetComponent<ChessMan>();

        board[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }
    public void SetPositionEmpty(int x, int y)
    {
        board[x, y] = null;
    }
    public GameObject GetPosition(int x, int y)
    {
        return board[x, y];
    }
    //is a postion on the board
    public bool IsPositionOnBoard(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= board.GetLength(0) || y >= board.GetLength(1));
    }
    public bool GetWhiteTurn() => whiteTurn;
    public bool IsGameOver()
    {
        return gameOver;
    }
    public void NextTurn()
    {
        if (whiteTurn)
            whiteTurn = false;
        else
            whiteTurn = true;
    }
    public void Winner(string playerWinner)
    {
        gameOver = true;

        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner!";
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }
}
