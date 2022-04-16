using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    //board pos
    int xBoard;
    int yBoard;

    public bool attack = false;

    public void Start()
    {
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }
    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        ChessMan piece = reference.GetComponent<ChessMan>();

        if (attack)
        {
            GameObject enemyChessPiece = controller.GetComponent<Game>().GetPosition(xBoard, yBoard);

            if (enemyChessPiece.name == "whiteKing")
                controller.GetComponent<Game>().Winner("black");
            if (enemyChessPiece.name == "blackKing")
                controller.GetComponent<Game>().Winner("white");

            Destroy(enemyChessPiece);
        }

        //set empty in the old piece's board
        controller.GetComponent<Game>().SetPositionEmpty(piece.GetXBoard(),piece.GetYBoard());

        piece.SetXBoard(xBoard);
        piece.SetYBoard(yBoard);
        piece.SetCoords();

        controller.GetComponent<Game>().SetPosition(reference);

        controller.GetComponent<Game>().NextTurn();

        piece.DestroyMovePlates();
        controller.GetComponent<Client>().SendBoard();
    }
    public void SetCoords(int x, int y)
    {
        xBoard = x;
        yBoard = y;
    }
    public void SetRefrence(GameObject obj)
    {
        reference = obj;
    }
    public GameObject GetReference()
    {
        return reference;
    }
}
