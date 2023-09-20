using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private int[] shipSizes = {2, 3, 3, 4, 5};
    private int shipSize = 2;
    private bool isVertical = false;

    public BoardPlayer boardPlayer;
    public BoardAI boardAI;

    public GameObject BoardUnitPrefab;
    public GameObject BoardUnitAttackPrefab;
    public GameObject BlockVisualizerPrefab;

    [Header("Player Piece Model Prefab Reference")]
    public List<GameObject> boardPiecesPref;

    [Header("----")]
    //public int blockSize = 3;
    //public bool Orientation = false;

    bool PLACE_BLOCK = true;

    [SerializeField]
    private int currentShipID;

    GameObject tmpHighlight = null;
    RaycastHit tmpHitHighlight;

    GameObject tmpBlockHolder = null;

    private bool OK_TO_PLACE = true;
    [SerializeField]

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

    private void OnChangeShip(int id)
    {
        currentShipID = id;
        shipSize = shipSizes[currentShipID];
    }

    private void OnChangeOrientation()
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

        currentShipID = 0;
        shipSize = shipSizes[currentShipID];
    }


    // Update is called once per frame
    void Update()
    {
        PlacePlayerPieces();
    }

    private void PlacePlayerPieces() //to be renamed
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Ray from camera to mouse position
        if (Input.mousePosition != null)
        {
            if (Physics.Raycast(ray, out tmpHitHighlight, 100))
            {
                BoardUnit tmpUI = tmpHitHighlight.transform.GetComponent<BoardUnit>();

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
                        OK_TO_PLACE = true;

                        if (!isVertical && (tmpUI.row <= 10 - shipSize))
                        {
                            for (int i = 0; i < shipSize; i++)
                            {
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

                        if (isVertical && (tmpUI.col <= 10 - shipSize))
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
                    }
                }
            }
        }
    }
    
}
