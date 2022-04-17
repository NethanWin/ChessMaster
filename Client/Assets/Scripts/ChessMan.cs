using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessMan : MonoBehaviour
{
    static float speed = 0.03f;


    static float mulUnity = 0.9f;
    static float addUnity = -3.15f;

    // References
    public GameObject controller;
    public GameObject movePlate;

    // Positions
    private int xBoard = -1;
    private int yBoard = -1;
    private float currentX;
    private float currentY;

    private float targetXBoard;
    private float targetYBoard;

    private bool isMoving = false;
    
    private float dx = 0;
    private float dy = 0;

    // Variable to keep track of white or black player
    private string player;

    // References for the chessPieces sprites
    public Sprite blackQueen, blackKnight, blackBishop, blackKing, blackRook, blackPawn;
    public Sprite whiteQueen, whiteKnight, whiteBishop, whiteKing, whiteRook, whitePawn;
    private void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetWhiteTurn() && player == "white")
        {
            DestroyMovePlates();

            InitiateMovePlates();
        }

    }
    private void Update()
    {
        Vector3 pos = this.transform.position;
        (currentX, currentY) = GetUnityCoords(xBoard, yBoard);
        if ((pos.x - currentX) * dx > 0 || (pos.y - currentY) * dy > 0)
            isMoving = false;
        if (isMoving)
        {
            Vector3 currentPos = this.transform.position;
            this.transform.position = new Vector3(currentPos.x + dx, currentPos.y + dy, -1.0f);
        }
    }
    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        //change game coordinates to unity's x,y
        SetCoords();

        switch (this.name)
        {
            case "blackQueen": this.GetComponent<SpriteRenderer>().sprite = blackQueen; player = "black"; break;
            case "blackKnight": this.GetComponent<SpriteRenderer>().sprite = blackKnight; player = "black"; break;
            case "blackBishop": this.GetComponent<SpriteRenderer>().sprite = blackBishop; player = "black"; break;
            case "blackKing": this.GetComponent<SpriteRenderer>().sprite = blackKing; player = "black"; break;
            case "blackRook": this.GetComponent<SpriteRenderer>().sprite = blackRook; player = "black"; break;
            case "blackPawn": this.GetComponent<SpriteRenderer>().sprite = blackPawn; player = "black"; break;

            case "whiteQueen": this.GetComponent<SpriteRenderer>().sprite = whiteQueen; player = "white"; break;
            case "whiteKnight": this.GetComponent<SpriteRenderer>().sprite = whiteKnight; player = "white"; break;
            case "whiteBishop": this.GetComponent<SpriteRenderer>().sprite = whiteBishop; player = "white"; break;
            case "whiteKing": this.GetComponent<SpriteRenderer>().sprite = whiteKing; player = "white"; break;
            case "whiteRook": this.GetComponent<SpriteRenderer>().sprite = whiteRook; player = "white"; break;
            case "whitePawn": this.GetComponent<SpriteRenderer>().sprite = whitePawn; player = "white"; break;
        }
    }
    public void MoveToTarget()
    {
        isMoving = true;
        Vector3 currentPos = this.transform.position;
        float targetX, targetY;
        (targetX, targetY) = GetUnityCoords(xBoard, yBoard);
        
        //set dx and dy
        float deltaX = targetX - currentPos.x;
        float deltaY = targetY - currentPos.y;
        float a = Mathf.Atan2(deltaY, deltaX);
        dx = speed * Mathf.Cos(a);
        dy = speed * Mathf.Sin(a);
    }
    public static int GetDir(float current, float target)
    {
        if (current > target)
            return -1;
        if (current < target)
            return 1;
        return 0;
    }
    private static (float, float) GetUnityCoords(int x, int y) => (x * mulUnity + addUnity, y * mulUnity + addUnity);
    public void SetCoords()
    {
        //teleports the piece to (xBoard,yBoard)
        isMoving = false;
        float x, y;
        (x, y) = GetUnityCoords(xBoard, yBoard);
        this.transform.position = new Vector3(x, y, -1.0f);
    }
    public int GetXBoard()
    {
        return xBoard;
    }
    public int GetYBoard()
    {
        return yBoard;
    }
    public void SetXBoard(int x)
    {
        xBoard = x;
    }
    public void SetYBoard(int y)
    {
        yBoard = y;
    }
    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
            Destroy(movePlates[i]);
    }
    public void InitiateMovePlates()
    {
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
                PawnMovePlate(xBoard, yBoard - 1);
                break;

            case "whitePawn":
                PawnMovePlate(xBoard, yBoard + 1);
                break;


        }
    }
    public void LineMovePlate(int dx, int dy)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + dx;
        int y = yBoard + dy;

        while (sc.IsPositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += dx;
            y += dy;
        }

        //can attack
        if (sc.IsPositionOnBoard(x,y) && sc.GetPosition(x,y).GetComponent<ChessMan>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }
    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard -1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }
    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard + 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
    }
    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.IsPositionOnBoard(x,y))
        {
            GameObject piece = sc.GetPosition(x, y);

            if (piece == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (piece.GetComponent<ChessMan>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }    
    }
    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.IsPositionOnBoard(x,y))
        {
            if (sc.GetPosition(x,y) == null)
            {
                MovePlateSpawn(x, y);
            }
            if (sc.IsPositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null &&
               sc.GetPosition(x + 1, y).GetComponent<ChessMan>().player != player)
            {
                MovePlateAttackSpawn(x + 1, y);
            }

            if (sc.IsPositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null &&
                sc.GetPosition(x - 1, y).GetComponent<ChessMan>().player != player)
            {
                MovePlateAttackSpawn(x - 1, y);
            }
        }
    }
    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        /*float x = matrixX;
        float y = matrixY;*/
        float x = matrixX * mulUnity + addUnity;
        float y = matrixY * mulUnity + addUnity;
        //to do
        /*
        x *= 0.66f;
        y *= 0.66f;
        y -= 2.3f;
        y -= 2.3f;
        */

        GameObject movPlate = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = movPlate.GetComponent<MovePlate>();
        mpScript.SetRefrence(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX * mulUnity + addUnity;
        float y = matrixY * mulUnity + addUnity;

        //to do
        /*
        x *= 0.66f;
        y *= 0.66f;
        y -= 2.3f;
        y -= 2.3f;
        */

        GameObject movPlate = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = movPlate.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetRefrence(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
}
