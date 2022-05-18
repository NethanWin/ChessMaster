using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    GameObject controller;
    GameObject pieceObject = null;
    Game game;
    Client client;
    //board pos
    Point pBoard = new Point(0,0);
    bool attack = false;

    public void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        client = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Client>();

        game = controller.GetComponent<Game>();

        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }
    public void OnMouseUp()
    {
        Piece piece = pieceObject.GetComponent<Piece>();
        if (attack)
        {
            GameObject enemyChessPiece = game.GetGameObjectOnPosition(pBoard);

            if (enemyChessPiece.name == "whiteKing")
                game.Winner("black");
            if (enemyChessPiece.name == "blackKing")
                game.Winner("white");
            Destroy(enemyChessPiece);
        }

        //set empty in the old piece's board

        Point tempP = piece.GetPBoard();
        game.SetEmptyPosition(piece.GetPBoard());
        piece.SetPBoard(new Point(pBoard));
        piece.MoveToTarget();
        game.SetPosition(pieceObject);
        game.DestroyAllMovePlates();
        client.SetWaitForServer(false);
        client.SendMsg(string.Format("1_{0}_{1}", tempP, pBoard));
    }
    public void SetPoint(Point p)
    {
        pBoard = new Point(p);
    }
    public void SetVars(Move m, GameObject go, bool attack, Client client2)
    {
        pBoard = new Point(m.GetTargetPoint());
        pieceObject = go;
        this.attack = attack;
        client = client2;
    }
}
