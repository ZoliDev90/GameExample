using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridHandler : MonoBehaviour
{
    public GameObject cardPrefab;
    public int rows = 4;          
    public int columns = 4;       
    public float spacing = 0.5f;  // Space between cards


    private List<GameObject> cards = new List<GameObject>();
    private ScreenOrientation currentOrientation;
    private Transform backgroundTable;

    void Start()
    {
        backgroundTable = transform.GetChild(0);
        currentOrientation = Screen.orientation;

        int wider = Mathf.Max(rows, columns);
        int thinner = Mathf.Min(rows, columns);

        // rows * columns must be even because of the card matching
        if ((rows * columns) % 2 == 1)
        {
            thinner += 1;
        }

        // the bigger grid parameter is used for the wider screen dimension for better fitting
        if (Screen.width > Screen.height)
        {
            rows = thinner;
            columns = wider;
        }
        else
        {
            rows = wider;
            columns = thinner;
        }

        setupGrid();
    }

    void Update()
    {
        // Check for orientation changes and adjust camera and cards to fit within the screen
        if (currentOrientation != Screen.orientation)
        {
            currentOrientation = Screen.orientation;
            AdjustCameraAndCardSize(rows, columns);
        }
    }


    public void setupGrid()
    {
        ClearGrid();
        GenerateGrid(rows, columns);
        AdjustCameraAndCardSize(rows, columns);
    }

    public void GenerateGrid(int rows, int columns)
    {
        float gridWidth, gridHeight;
        CalculateGridDimensions(out gridWidth, out gridHeight, rows, columns);

        // Calculate the starting position to center the grid
        Vector3 startPosition = new Vector3(
            -gridWidth / 2 + cardPrefab.transform.localScale.x / 2,
            0,
            -gridHeight / 2 + cardPrefab.transform.localScale.z / 2
        );


        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject card = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                card.transform.parent = this.transform;

                // Calculate card position
                Vector3 position = startPosition + new Vector3(
                    col * (cardPrefab.transform.localScale.x + spacing),
                    0,
                    row * (cardPrefab.transform.localScale.z + spacing)
                );
                card.transform.localPosition = position;

                cards.Add(card);
            }
        }
    }


    private void ClearGrid()
    {
        foreach (GameObject card in cards)
        {
            Destroy(card);
        }
        cards.Clear();
    }



    private void AdjustCameraAndCardSize(int rows, int columns)
    {
        Camera mainCamera = Camera.main;

        float gridWidth, gridHeight;
        CalculateGridDimensions(out gridWidth, out gridHeight, rows, columns);

        // Adjust the camera orthographic size to fit the grid including spacing
        float totalGridHeight = gridHeight + (spacing * (rows - 1));
        float totalGridWidth = gridWidth + (spacing * (columns - 1));

        // Ensure camera orthographic size fits the height
        mainCamera.orthographicSize = Mathf.Max(totalGridHeight / 2f, (totalGridWidth / mainCamera.aspect) / 2f);
        // Ensure the background is big enough to cover the camera view
        backgroundTable.localScale = new Vector3(mainCamera.orthographicSize / 2f, mainCamera.orthographicSize / 2f, mainCamera.orthographicSize / 2f);
        // Adjust card sizes to fit within grid cells
        float cardWidth = (gridWidth / columns) - (spacing * (columns - 1) / columns);
        float cardHeight = (gridHeight / rows) - (spacing * (rows - 1) / rows);

        foreach (GameObject card in cards)
        {
            card.transform.localScale = new Vector3(cardWidth, 0.05f, cardHeight);
        }
    }




    private void CalculateGridDimensions(out float gridWidth, out float gridHeight, int rows, int columns)
    {
        float cardWidth = cardPrefab.transform.localScale.x;
        float cardHeight = cardPrefab.transform.localScale.z;

        // Calculate the total width and height including spacing
        gridWidth = (columns * cardWidth) + (spacing * (columns - 1));
        gridHeight = (rows * cardHeight) + (spacing * (rows - 1));
    }

    public void UpdateGridSize(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;
        setupGrid();
    }



}