using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Board 

{
    public GameObject[,] board = new GameObject[10, 10];
    public GameObject boardUnitPrefab;

    public void ClearBoard() 
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; i < 10; i++)
            {
                board[i, j] = null;
            }
        }
    }
}
