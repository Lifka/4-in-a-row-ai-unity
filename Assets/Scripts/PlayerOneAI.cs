using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneAI : AI
{
    /**
     * board es el tablero. Podemos acceder a una posición concreta mediante board[x][y]
     * Cada posición contiene un valor que nos indica si está ocupada o no, y qué jugador la ha ocupado.
     * Valores posibles:
     *    Piece.Empty ---> Casilla libre
     *    Piece.PlayerOne ---> Casilla con una ficha del Player One
     *    Piece.PlayerTwo ---> Casilla con una ficha del Player Two
    **/

    /**
     *  [Esta función tiene que existir obligatoriamente y tiene que devolver un entero. Modificar solo la funcionalidad.]
     *  Devuelve el índice de la columna en la que caerá la ficha
     *  La columna que está más a la izquierda es la 0, la columna que está más a la derecha es Config.numColumns-1 (==> 6)
     **/
    public override int nextMove()
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
     *  [Función de ejemplo, se puede borrar]
     *  Devuelve todas las columnas del tablero vacías. Devolverá la misma columna tantas veces como casillas
     *  tenga disponibles. Por lo tanto, si escogemos la columna aleatoriamente, habrá más
     *  posibilidad de seleccionar nuestro próximo movimiento como aquella columna que tenga más casillas vacías.
     **/
    public List<int> GetPossibleMoves()
    {
        List<int> possibleMoves = new List<int>();

        // Recorremos todo el tablero
        for (int x = 0; x < Config.numColumns; x++)
        {
            for (int y = 0; y < Config.numRows; y++)
            {
                // Si la casilla vale Piece.Empty, significa que está libre
                if (board[x][y] == Piece.Empty)
                {
                    // Guardamos la columna
                    possibleMoves.Add(x);
                }
            }
        }

        return possibleMoves;
    }
}
