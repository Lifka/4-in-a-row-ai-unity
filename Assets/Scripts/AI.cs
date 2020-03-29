using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    /**
     * GameController.field es el tablero. Podemos acceder a una posición concreta mediante GameController.field[x][y]
     * Cada posición contiene un valor que nos indica si está ocupada o no, y qué jugador la ha ocupado.
     * Valores posibles:
     *    GameController.Piece.Empty ---> Casilla libre
     *    GameController.Piece.PlayerOne ---> Casilla con una ficha del Player One
     *    GameController.Piece.PlayerTwo ---> Casilla con una ficha del Player Two
    **/

    /**
     *  [Esta función tiene que existir obligatoriamente y tiene que devolver un entero. Modificar solo la funcionalidad.]
     *  Devuelve el índice de la columna en la que caerá la ficha
     *  La columna que está más a la izquierda es la 0, la columna que está más a la derecha es Config.numColumns-1 (==> 6)
     **/
    public int nextMove()
    {
        int column = -1; // Valor nulo
        List<int> possibleMoves = GetPossibleMoves();

        // Si hay espacios disponibles en el tablero
        if (possibleMoves.Count > 0)
        {
            // Elegimos una columana aleatoria
            column = possibleMoves[Random.Range(0, possibleMoves.Count)];
        }

        return column;
    }

    /**
     *  Devuelve todas las posiciones del tablero vacías 
     **/
    public List<int> GetPossibleMoves()
    {
        List<int> possibleMoves = new List<int>();
        for (int x = 0; x < Config.numColumns; x++)
        {
            for (int y = 0; y < Config.numRows; y++)
            {
                if (GameController.field[x][y] == GameController.Piece.Empty)
                {
                    possibleMoves.Add(x);
                }
            }
        }

        return possibleMoves;
    }
}

