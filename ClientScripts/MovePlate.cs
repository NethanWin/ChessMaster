using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    GameObject controller;
    GameObject pieceObject = null;
    Game game;

    //board pos
    Point pBoard = new Point(0,0);
    bool attack = false;

    public void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
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
        game.SetEmptyPosition(piece.GetPBoard());
        controller.GetComponent<Client>().SendMsg(string.Format("1_{0}_{1}", piece.GetPBoard(), pBoard));
        piece.SetPBoard(new Point(pBoard));
        piece.MoveToTarget();
        game.SetPosition(pieceObject);
        game.NextTurn();
        game.DestroyAllMovePlates();
    }
    public void SetPoint(Point p)
    {
        pBoard = new Point(p);
    }
    public void SetVars(Move m, GameObject go, bool attack)
    {
        pBoard = new Point(m.GetTargetPoint());
        pieceObject = go;
        this.attack = attack;
    }
}
