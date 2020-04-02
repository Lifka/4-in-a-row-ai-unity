using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    [Range(3, 8)]
    public static int numRows = 6;
    [Range(3, 8)]
    public static int numColumns = 7;

    [Tooltip("How many pieces have to be connected to win.")]
    public static int numPiecesToWin = 4;

    [Tooltip("Allow diagonally connected Pieces?")]
    public static bool allowDiagonally = true;

    public static float dropTime = 4f;
}
