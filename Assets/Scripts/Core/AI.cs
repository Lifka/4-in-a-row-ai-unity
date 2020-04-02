using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI : MonoBehaviour
{
    public static List<List<Piece>> board;
    private void Start()
    {
        board = GameController.board;
    }

    public abstract int nextMove();
}

