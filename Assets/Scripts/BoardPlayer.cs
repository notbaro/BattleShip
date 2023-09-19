using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardPlayer : Board
{
    public BoardPlayer(GameObject unitPrefab)
    {
        this.boardUnitPrefab = unitPrefab;
        ClearBoard();
    }

    public void CreatePlayerBoard()
    {
        //create Player Controlled Board - 10x10
        int row = 1;
        int col = 1;
        for (int i = 0; i < 10; i++) 
        {
            for (int j = 0; j < 10; j++)
            {
                //instantiate boardunit prefab and place on scene
                GameObject tmp = GameObject.Instantiate(boardUnitPrefab, new Vector3(i, 0, j), boardUnitPrefab.transform.rotation) as GameObject;
                BoardUnit tmpUI = tmp.GetComponentInChildren<BoardUnit>();
                string name = string.Format($"B1[{row - 1}, {col - 1}]");
                tmpUI.tmpBoardUnitLabel.text = name;
                tmpUI.row = row - 1;
                tmpUI.col = col - 1;
                board[tmpUI.row, tmpUI.col] = tmp;
                tmp.name = name;
                col++;
            }
            row++;
            col = 1;
        }
    }
}
