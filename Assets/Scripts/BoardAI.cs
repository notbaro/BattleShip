using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BoardAI : Board
{
    GameObject cubePrefab;
    int[] aiShipSizes = new int[5] { 2, 3, 3, 4, 5 };
    private UnityEngine.Color[] visualColors = new UnityEngine.Color[5] { UnityEngine.Color.red, UnityEngine.Color.green, UnityEngine.Color.blue, UnityEngine.Color.magenta, UnityEngine.Color.cyan, };
    public BoardAI(GameObject unitPrefab, GameObject prefab)
    {
        this.boardUnitPrefab = unitPrefab;
        this.cubePrefab = prefab;
        ClearBoard();
    }
    //visual color for ai ships placement - red, green, blue, magenta, cyan {2, 3, 3, 4, 5}

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

    public void PlaceShips() 
    {
        for (int i = 0; i < 5; i++)
        {
            int row = Random.Range(0, 9);
            int col = Random.Range(0, 9);
            bool vertical = Random.Range(0, 2) == 0 ? true : false;
            CheckPlacement(row, col, i, vertical);
        }
    }

    private void CheckPlacement(int row, int col, int sizeIndex, bool vertical)
    {
        GameObject tmp = board[row, col];
        var boardUnit = tmp.GetComponentInChildren<BoardUnit>();
        //bounds check
        if (boardUnit.isOccupied || (row + aiShipSizes[sizeIndex] > 10) || (col + aiShipSizes[sizeIndex] > 10))
        {
            int newRow = Random.Range(0, 9);
            int newCol = Random.Range(0, 9);
            CheckPlacement(newRow, newCol, sizeIndex, vertical);
            return; 
        }

        bool OK_TO_PLACE = true;
        //occupied check
        if (vertical && (row + aiShipSizes[sizeIndex] < 10))
        {
            for (int i = 0; i < aiShipSizes[sizeIndex]; i++)
            {
                GameObject bp = board[row + i, col];
                var bpUI = bp.GetComponentInChildren<BoardUnit>();
                if (bpUI.isOccupied)
                {
                    OK_TO_PLACE = false;
                }
            }
        }
        if (!vertical && (col + aiShipSizes[sizeIndex] < 10))
        {
            for (int i = 0; i < aiShipSizes[sizeIndex]; i++)
            {
                GameObject bp = board[row, col + i];
                var bpUI = bp.GetComponentInChildren<BoardUnit>();
                if (bpUI.isOccupied)
                {
                    OK_TO_PLACE = false;
                }
            }
        }
        //placing ships
        if (OK_TO_PLACE)
        {
            if (vertical)
            {
                for (int i = 0; i < aiShipSizes[sizeIndex]; i++)
                {
                    GameObject visual = GameObject.Instantiate(cubePrefab, new Vector3(row + i + 11, 0.5f, col), cubePrefab.transform.rotation) as GameObject;
                    visual.GetComponent<Renderer>().material.color = visualColors[sizeIndex];

                    GameObject sB = board[row + i, col];
                    sB.GetComponentInChildren<BoardUnit>().isOccupied = true;
                    board[row + i, col] = sB;
                }
            }
            else
            {
                for (int i = 0; i < aiShipSizes[sizeIndex]; i++)
                {
                    GameObject visual = GameObject.Instantiate(cubePrefab, new Vector3(row + 11, 0.5f, col + i), cubePrefab.transform.rotation) as GameObject;
                    visual.GetComponent<Renderer>().material.color = visualColors[sizeIndex];

                    GameObject sB = board[row, col + i];
                    sB.GetComponentInChildren<BoardUnit>().isOccupied = true;
                    board[row, col + i] = sB;
                }
            }
        }
        else
        {
            int newRow = Random.Range(0, 9);
            int newCol = Random.Range(0, 9);
            CheckPlacement(newRow, newCol, sizeIndex, vertical);
        }
    }
}
