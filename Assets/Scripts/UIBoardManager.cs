using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class UIBoardManager : MonoBehaviour
{
    public delegate void ChangeShip(int id, int size);
    public static event ChangeShip OnChangeShip;

    public delegate void ChangeOrientation(bool Vertical);
    public static event ChangeOrientation OnChangeOrientation;

    [Header("Player Pieces References")]
    public List<Button> collectionOfPlayerPieceButtons;
    [Header("Game Buttons References")]
    public Button btnOrientation;
    public Button btnUIReset;
    public Button btnExit;
    [Header("Sprite References")]
    public Sprite spriteHorizontal;
    public Sprite spriteVertical;
    [Header("Basic Flag Settings")]
    public bool DisplayInstructions;
    public bool isVertical = false;

    public Dictionary<int, int> shipSizes = new Dictionary<int, int>()
    {
        {0, 2},
        {1, 3},
        {2, 3},
        {3, 4},
        {4, 5}
    };

    private void OnEnable()
    {
        BoardManager.OnBoardPiecePlaced += OnBoardPiecePlaced;
    }

    private void OnDisable()
    {
        BoardManager.OnBoardPiecePlaced -= OnBoardPiecePlaced;
    }

    private void OnBoardPiecePlaced(int id)
    {
        if (id < 0)
        {
            Debug.LogError("Invalid ID. Possibly caused by not pressing a ship button yet.");
            return;
        } 
        else
            collectionOfPlayerPieceButtons[id].gameObject.SetActive(false);
    }

    void Start()
    {
        UpdateOrientationUI();
    }


    public void OnShipButtonClick(int id)
    {
        int shipSize = shipSizes.ContainsKey(id) ? shipSizes[id] : -1;
        if (shipSize == -1)
        {
            Debug.LogError("Ship size not found");
        }
        else
        {
            OnChangeShip?.Invoke(id, shipSize);
        }
    }

    public void OnOrientationButtonClick()
    {
        isVertical = !isVertical;
        UpdateOrientationUI();
    }
    public void UpdateOrientationUI()
    {
        if (!isVertical)
        {
            btnOrientation.image.sprite = spriteVertical;
        }
        else
        {
            btnOrientation.image.sprite = spriteHorizontal;
        }
        OnChangeOrientation?.Invoke(isVertical);
    }

}
