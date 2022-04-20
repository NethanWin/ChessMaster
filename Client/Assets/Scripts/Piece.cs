using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    //Constants
    static float mulUnity = 0.9f;
    static float addUnity = -3.15f;
    static float baseSpeed = 0.03f;

    // References
    public GameObject controller;
    public GameObject movePlate;
    public Game game;

    // Positions
    Point pBoard = new Point(0,0);
    Point currentP;

    //movment animation
    bool isMoving = false;
    float dx;
    float dy;
    
    // Variable to keep track of white or black isWhite
    bool isWhite;

    // References for the chessPieces sprites
    public Sprite blackQueen, blackKnight, blackBishop, blackKing, blackRook, blackPawn;
    public Sprite whiteQueen, whiteKnight, whiteBishop, whiteKing, whiteRook, whitePawn;

    void OnMouseUp()
    {
        if (!game.IsGameOver() && game.GetWhiteTurn() && isWhite)
        {
            game.DestroyAllMovePlates();
            InitiateMovePlates();
        }
    }
    void Update()
    {
        Vector3 pos = transform.position;
        currentP = GetUnityCoords(pBoard);
        if ((pos.x - currentP.x) * dx > 0 || (pos.y - currentP.y) * dy > 0)
            isMoving = false;
        if (isMoving)
        {
            Vector3 currentPos = transform.position;
            transform.position = new Vector3(currentPos.x + dx, currentPos.y + dy, -1.0f);
        }
    }
    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
        TeleportToCoords();

        string colorStr = name.Substring(0, 5);
        isWhite = colorStr == "white" ? true : false;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        switch (name)
        {
            case "blackQueen": spriteRenderer.sprite = blackQueen; break;
            case "blackKnight": spriteRenderer.sprite = blackKnight; break;
            case "blackBishop": spriteRenderer.sprite = blackBishop; break;
            case "blackKing": spriteRenderer.sprite = blackKing; break;
            case "blackRook": spriteRenderer.sprite = blackRook; break;
            case "blackPawn": spriteRenderer.sprite = blackPawn; break;


            case "whiteQueen": spriteRenderer.sprite = whiteQueen; break;
            case "whiteKnight": spriteRenderer.sprite = whiteKnight; break;
            case "whiteBishop": spriteRenderer.sprite = whiteBishop; break;
            case "whiteKing": spriteRenderer.sprite = whiteKing; break;
            case "whiteRook": spriteRenderer.sprite = whiteRook; break;
            case "whitePawn": spriteRenderer.sprite = whitePawn; break;
        }
    }
    public Point GetPBoard() => new Point(pBoard);
    public void MoveToTarget()
    {
        isMoving = true;
        Vector3 currentPos = this.transform.position;
        Point targetP = GetUnityCoords(pBoard);

        //set dx and dy
        float deltaX = targetP.x - currentPos.x;
        float deltaY = targetP.y - currentPos.y;
        float a = Mathf.Atan2(deltaY, deltaX);
        dx = baseSpeed * Mathf.Cos(a);
        dy = baseSpeed * Mathf.Sin(a);
    }
    public void TeleportToCoords()
    {
        //Teleports the piece to pBoard
        isMoving = false;
        Point p = GetUnityCoords(pBoard);
        transform.position = new Vector3(p.x, p.y, -1.0f);
    }
    void InitiateMovePlates()
    {
        //Choose which MovePlate To create
        switch (this.name)
        {
            case "blackQueen":
            case "whiteQueen":
                LineMovePlate(1, 0);
                LineMovePlate(1, -1);
                LineMovePlate(1, 1);
                LineMovePlate(0, -1);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                break;

            case "blackKnight":
            case "whiteKnight":
                LMovePlate();
                break;

            case "blackBishop":
            case "whiteBishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;

            case "blackKing":
            case "whiteKing":
                SurroundMovePlate();
                break;

            case "blackRook":
            case "whiteRook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;

            case "blackPawn":
                PawnMovePlate(new Point(pBoard.x, pBoard.y - 1));
                break;

            case "whitePawn":
                PawnMovePlate(new Point(pBoard.x, pBoard.y + 1));
                break;


        }
    }
    void CreateMovePlate(Move m)
    {
        //Checks if there's a piece in the targeted position and calls the correct MovePlate
        if (game.IsPositionOnBoard(m.GetTargetPoint()))
        {
            GameObject piece = game.GetGameObjectOnPosition(m.GetTargetPoint());
            if (piece == null)
                CreateMovePlate(m, false);
            else if (piece.GetComponent<Piece>().isWhite != isWhite)
                CreateMovePlate(m, true);
        }
    }
    void CreateMovePlate(Move m, bool isAttackMove)
    {
        Point p = GetUnityCoords(m.GetTargetPoint());

        GameObject movPlate = Instantiate(movePlate, new Vector3(p.x, p.y, -3.0f), Quaternion.identity);
        movPlate.GetComponent<MovePlate>().SetVars(m, gameObject,  isAttackMove);
    }
    public void SetPBoard(Point p)
    {
        pBoard.x = p.x;
        pBoard.y = p.y;
    }

    //check where to put
    Point GetUnityCoords(Point pBoard)
    {
        //Returns a Point of the engines location with the board location
        float newX = pBoard.x * mulUnity + addUnity;
        float newY = pBoard.y * mulUnity + addUnity;
        return new Point(newX, newY);
    }


    //to delete after integration with core
    //
    //
    //
    public void LineMovePlate(int dx, int dy)
    {
        Game game = controller.GetComponent<Game>();
        Point p = new Point(pBoard.x + dx, pBoard.y + dy);
        while (game.IsPositionOnBoard(p) && game.GetGameObjectOnPosition(p) == null)
        {
            CreateMovePlate(new Move(pBoard, p));
            p.x += dx;
            p.y += dy;
        }

        //can attack
        //if (game.IsPositionOnBoard(x, y) && game.GetPosition(x, y).GetComponent<Piece>().isWhite != isWhite)
        //{
          //  MovePlateAttackSpawn(x, y);
        //}
    }
    public void LMovePlate()
    {
        CreateMovePlate(new Move(new Point(pBoard), new Point(pBoard.x + 1, pBoard.y + 2)));
        /*PointMovePlate(xBoard -1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);*/
    }
    public void SurroundMovePlate()
    {
        /*PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard + 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard - 1, yBoard - 1);*/
    }
    void PawnMovePlate(Point p)
    {
        //not working
        Game game = controller.GetComponent<Game>();
        if (game.IsPositionOnBoard(p))
        {
            if (game.GetGameObjectOnPosition(p) == null)
            {
                //MovePlateSpawn(x, y);
            }
            //if (game.IsPositionOnBoard(x + 1, y) && game.GetPosition(x + 1, y) != null &&
            //   game.GetPosition(x + 1, y).GetComponent<Piece>().isWhite != isWhite)
           // {
           //     MovePlateAttackSpawn(x + 1, y);
            /*}

            if (game.IsPositionOnBoard(x - 1, y) && game.GetPosition(x - 1, y) != null &&
                game.GetPosition(x - 1, y).GetComponent<Piece>().isWhite != isWhite)
            {
                MovePlateAttackSpawn(x - 1, y);
            }*/
        }
    }
}
