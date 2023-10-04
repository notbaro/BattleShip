using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public delegate void StartGame(bool val);
    public static event StartGame OnStartGame;

    public delegate void BoardPiecePlaced(int id);
    public static event BoardPiecePlaced OnBoardPiecePlaced;

    private int[] shipSizes = { 2, 3, 3, 4, 5 };
    private int shipSize = 0;
    private bool isVertical = false;
    private bool[] shipPlaced = { false, false, false, false, false };

    public BoardPlayer boardPlayer;
    public BoardAI boardAI;

    public GameObject BoardUnitPrefab;
    public GameObject BoardUnitAttackPrefab;
    public GameObject BlockVisualizerPrefab;

    [Header("Player Piece Model Prefab Reference")]
    public List<GameObject> boardPiecesPref;

    [Header("----")]


    bool PLACE_BLOCK = true;

    [SerializeField]
    private int currentShipID = -1;

    GameObject tmpHighlight = null;
    RaycastHit tmpHitHighlight;

    GameObject tmpBlockHolder = null;

    private bool OK_TO_PLACE = true;
    [SerializeField]
    private int shipPlacedCount = 0;
    bool placeEnemyShip = true;
    // GameObject tmpAttackHighlight = null;
    // RaycastHit tmpHitAttackHighlight;
    // GameObject tmpAttackBlockHolder = null;

    private void OnEnable()
    {
        UIBoardManager.OnChangeShip += OnChangeShip;
        UIBoardManager.OnChangeOrientation += OnChangeOrientation;
    }

    private void OnDisable()
    {
        UIBoardManager.OnChangeShip -= OnChangeShip;
        UIBoardManager.OnChangeOrientation -= OnChangeOrientation;
    }

    private void OnChangeShip(int id, int size)
    {
        currentShipID = id;
        shipSize = size;
    }

    private void OnChangeOrientation(bool Orientation)
    {
        isVertical = !isVertical;
    }
    // Start is called before the first frame update
    void Start()
    {
        boardPlayer = new BoardPlayer(BoardUnitPrefab);
        boardPlayer.CreatePlayerBoard();

        boardAI = new BoardAI(BoardUnitAttackPrefab, BlockVisualizerPrefab);
        boardAI.CreateAIBoard();

        currentShipID = -1;
        shipSize = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (isBusy)
            return;
        if (shipPlacedCount < 5)
            PlacePlayerPieces();
        else
        {
            if (placeEnemyShip)
            {
                boardAI.PlaceShips();
                placeEnemyShip = false;
                OnStartGame?.Invoke(true);
            }
        }
    }

    private void PlacePlayerPieces()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Ray from camera to mouse position
        if (Input.mousePosition != null)
        {
            if (Physics.Raycast(ray, out tmpHitHighlight, 100))
            {
                BoardUnit tmpUI = tmpHitHighlight.transform.GetComponent<BoardUnit>();
                OK_TO_PLACE = true;
                if (tmpHitHighlight.transform.tag.Equals("BoardUnit") && !tmpUI.isOccupied)
                {
                    BoardUnit boardData = boardPlayer.board[tmpUI.row, tmpUI.col].transform.GetComponentInChildren<BoardUnit>();
                    if (tmpHighlight != null)
                    {
                        if (boardData.isOccupied)
                            tmpHighlight.GetComponent<Renderer>().material.color = Color.red;
                        else
                            tmpHighlight.GetComponent<Renderer>().material.color = Color.white;
                    }

                    if (tmpBlockHolder != null)
                    {
                        Destroy(tmpBlockHolder);
                    }

                    if (PLACE_BLOCK)
                    {
                        tmpBlockHolder = new GameObject();


                        if (!isVertical && (tmpUI.row <= 10 - shipSize)) //horizontal placing
                        {
                            for (int i = 0; i < shipSize; i++)
                            {
                                //create a visual block
                                GameObject visual = GameObject.Instantiate(BlockVisualizerPrefab, new Vector3(tmpUI.row + i, BlockVisualizerPrefab.transform.position.y, tmpUI.col), BlockVisualizerPrefab.transform.rotation) as GameObject;

                                GameObject bp = boardPlayer.board[tmpUI.row + i, tmpUI.col];
                                BoardUnit bpUI = bp.GetComponentInChildren<BoardUnit>();
                                if (!bpUI.isOccupied)
                                {
                                    visual.GetComponent<Renderer>().material.color = Color.gray; //ok to place
                                }
                                else
                                {
                                    visual.GetComponent<Renderer>().material.color = Color.red; //can't be placed
                                    OK_TO_PLACE = false;
                                }
                                visual.transform.parent = tmpBlockHolder.transform;
                            }
                        }

                        else if (isVertical && (tmpUI.col <= 10 - shipSize)) //vertical placing
                        {
                            for (int i = 0; i < shipSize; i++)
                            {

                                GameObject visual = GameObject.Instantiate(BlockVisualizerPrefab, new Vector3(tmpUI.row, BlockVisualizerPrefab.transform.position.y, tmpUI.col + i), BlockVisualizerPrefab.transform.rotation) as GameObject;

                                GameObject bp = boardPlayer.board[tmpUI.row, tmpUI.col + i];
                                BoardUnit bpUI = bp.GetComponentInChildren<BoardUnit>();
                                if (!bpUI.isOccupied)
                                {
                                    visual.GetComponent<Renderer>().material.color = Color.gray; //ok to place
                                }
                                else
                                {
                                    visual.GetComponent<Renderer>().material.color = Color.red; //can't be placed
                                    OK_TO_PLACE = false;
                                }
                                visual.transform.parent = tmpBlockHolder.transform;
                            }
                        }
                        else //if the location is out of bounds
                        {
                            OK_TO_PLACE = false;
                        }
                    }
                }
                else
                {
                    OK_TO_PLACE = false;
                }
            }


        }
        if (Input.GetMouseButtonDown(0)) //
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.tag.Equals("BoardUnit"))
                {
                    BoardUnit tmpUI = hit.transform.GetComponentInChildren<BoardUnit>();
                    if (PLACE_BLOCK && OK_TO_PLACE)
                    {
                        if (!isVertical)
                        {
                            for (int i = 0; i < shipSize; i++)
                            {
                                GameObject boardElement = boardPlayer.board[tmpUI.row + i, tmpUI.col];
                                BoardUnit boardUnit = boardElement.transform.GetComponentInChildren<BoardUnit>();
                                boardUnit.isOccupied = true;
                                //boardUnit.CubePrefab.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                                //boardUnit.CubePrefab.gameObject.SetActive(true);
                                boardUnit.GetComponent<MeshRenderer>().material.color = Color.green;

                                boardPlayer.board[tmpUI.row + i, tmpUI.col] = boardElement;
                            }
                        }
                        if (isVertical)
                        {
                            for (int i = 0; i < shipSize; i++)
                            {
                                GameObject boardElement = boardPlayer.board[tmpUI.row, tmpUI.col + i];
                                BoardUnit boardUnit = boardElement.transform.GetComponentInChildren<BoardUnit>();
                                boardUnit.isOccupied = true;
                                //boardUnit.CubePrefab.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                                //boardUnit.CubePrefab.gameObject.SetActive(true);
                                boardUnit.GetComponent<MeshRenderer>().material.color = Color.green;

                                boardPlayer.board[tmpUI.row, tmpUI.col + i] = boardElement;
                            }
                        }
                        CheckWhichShipPlaced(tmpUI.row, tmpUI.col);
                        OK_TO_PLACE = true;
                        tmpHighlight = null;
                    }
                    if (shipPlacedCount == 5 && tmpBlockHolder != null)
                    {
                        Destroy(tmpBlockHolder);

                    }

                }
            }
        }
    }
    
    private void CheckWhichShipPlaced(int row, int col) //placing ship models
    {
        switch (currentShipID)
        {
            //id 0, size 2
            case 0:
                if (!isVertical)
                {
                    GameObject shipVisual = GameObject.Instantiate(boardPiecesPref[currentShipID], new Vector3(row + .4f, boardPiecesPref[currentShipID].transform.position.y, col), boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    shipVisual.transform.localScale = new Vector3(.7f, .7f, .7f);
                    shipVisual.transform.RotateAround(shipVisual.transform.position, Vector3.up, 90f);
                }
                else
                {
                    GameObject shipVisual = GameObject.Instantiate(boardPiecesPref[currentShipID], new Vector3(row, boardPiecesPref[currentShipID].
                    transform.position.y, col + .4f), boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    shipVisual.transform.localScale = new Vector3(.7f, .7f, .7f);

                }
                shipPlacedCount++;
                break;
            //id 1, size 3
            case 1:
                if (!isVertical)
                {
                    GameObject shipVisual = GameObject.Instantiate(boardPiecesPref[currentShipID], new Vector3(row + 1, boardPiecesPref[currentShipID].transform.position.y +.15f, col), boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    shipVisual.transform.localScale = new Vector3(.5f, .5f, .5f);
                    shipVisual.transform.RotateAround(shipVisual.transform.position, Vector3.up, 90f);
                }
                else
                {
                    GameObject shipVisual = GameObject.Instantiate(boardPiecesPref[currentShipID], new Vector3(row, boardPiecesPref[currentShipID].
                    transform.position.y + .15f, col + 1), boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    shipVisual.transform.localScale = new Vector3(.5f, .5f, .5f);

                }
                shipPlacedCount++;
                break;
            //id 2, size 3
            case 2:
            if (!isVertical)
                {
                    GameObject shipVisual = GameObject.Instantiate(boardPiecesPref[currentShipID], new Vector3(row + .9f, boardPiecesPref[currentShipID].transform.position.y, col), boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    shipVisual.transform.localScale = new Vector3(.5f, .5f, .5f);
                    shipVisual.transform.RotateAround(shipVisual.transform.position, Vector3.up, 90f);
                }
                else
                {
                    GameObject shipVisual = GameObject.Instantiate(boardPiecesPref[currentShipID], new Vector3(row, boardPiecesPref[currentShipID].
                    transform.position.y, col + .9f), boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    shipVisual.transform.localScale = new Vector3(.5f, .5f, .5f);

                }
                shipPlacedCount++;
                break;
            //id 3, size 4
            case 3:
            if (!isVertical)
                {
                    GameObject shipVisual = GameObject.Instantiate(boardPiecesPref[currentShipID], new Vector3(row + 1.5f, boardPiecesPref[currentShipID].transform.position.y, col), boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    shipVisual.transform.RotateAround(shipVisual.transform.position, Vector3.up, 90f);
                }
                else
                {
                    GameObject shipVisual = GameObject.Instantiate(boardPiecesPref[currentShipID], new Vector3(row, boardPiecesPref[currentShipID].
                    transform.position.y, col + 1.5f), boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                }
                shipPlacedCount++;
                break;    
            //id 4, size 5
            case 4:
                if (!isVertical)
                {
                    GameObject shipVisual = GameObject.Instantiate(boardPiecesPref[currentShipID], new Vector3(row - 0.5f, boardPiecesPref[currentShipID].transform.position.y + .04f, col + .2f), boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    shipVisual.transform.localScale = new Vector3(.03f, .03f, .03f);
                    
                }
                else
                {
                    GameObject shipVisual = GameObject.Instantiate(boardPiecesPref[currentShipID], new Vector3(row + .2f, boardPiecesPref[currentShipID].
                    transform.position.y +.04f, col + 4.5f), boardPiecesPref[currentShipID].transform.rotation) as GameObject;
                    shipVisual.transform.localScale = new Vector3(.03f, .03f, .03f);
                    shipVisual.transform.RotateAround(shipVisual.transform.position, Vector3.up, 90f);

                }
                shipPlacedCount++;
                break;
        }
        //invoke place event
        OnBoardPiecePlaced?.Invoke(currentShipID);

        //reset parameters
        currentShipID = -1;
        shipSize = 0;
        Destroy(tmpBlockHolder); //remove the visual block after placing model
    }

    public static bool isBusy = false;
    IEnumerator Wait(float seconds = 0.5f)
    {
        isBusy = true;
        yield return new WaitForSeconds(seconds);
        isBusy = false;
    }
}
