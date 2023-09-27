using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardUnit : MonoBehaviour
{
    public int row;
    public int col;
    public bool isVertical;
    public bool isOccupied;
    public TMP_Text tmpBoardUnitLabel;
    public GameObject CubePrefab;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
