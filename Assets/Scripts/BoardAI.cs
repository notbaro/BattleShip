using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAI : Board
{
    GameObject cubePrefab;
    int[] aiShipSizes = new int[5] { 2, 3, 3, 4, 5 };
    public BoardAI(GameObject unitPrefab, GameObject prefab)
    {
        this.boardUnitPrefab = unitPrefab;
        this.cubePrefab = prefab;
        ClearBoard();
    }

    public void CreateAIBoard()
    {
        //create AI Controlled Board - 10x10
        int row = 1;
        int col = 1;
        for (int i = 11; i < 21; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                //instantiate boardunit prefab and place on scene
                GameObject tmp = GameObject.Instantiate(boardUnitPrefab, new Vector3(i, 0, j), boardUnitPrefab.transform.rotation);
                BoardUnit tmpUI = tmp.GetComponentInChildren<BoardUnit>();
                string name = string.Format($"B2[{row - 1}, {col - 1}]", row, col);
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
