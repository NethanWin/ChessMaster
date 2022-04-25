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

    BasePiece basePiece;

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
    public static PType GetPType(string name)
    {
        //returns the PType for the appropriate piece's name
        string PName = name.Substring(5, name.Length - 5);
        switch (PName)
        {
            case "King": return PType.King;
            case "Queen": return PType.Queen;
            case "Bishop": return PType.Bishop;
            case "Knight": return PType.Knight;
            case "Pawn": return PType.Pawn;
            case "Rook": return PType.Rook;
        }
        return PType.Pawn;
    }
    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        game = controller.GetComponent<Game>();
        TeleportToCoords();

        string colorStr = name.Substring(0, 5);
        isWhite = colorStr == "white" ? true : false;

        PType type = GetPType(name);
        
        basePiece = new BasePiece(type, !isWhite, pBoard);

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
        basePiece.UpdateMoves(new Board(game.GetBoard()));
        foreach (Move m in basePiece.GetMoves())
            CreateMovePlate(m);
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
}
