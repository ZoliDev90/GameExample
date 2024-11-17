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
    private Dictionary<int, Sprite> imageDictionary;

    [SerializeField] private CardHandler cardHandler;

    void Start()
    {
        backgroundTable = transform.GetChild(0);
        currentOrientation = Screen.orientation;

        LoadCardImages();
        //setupGrid(rows, columns);
    }

    void Update()
    {
        // Check for orientation changes and adjust camera and cards to fit within the screen
        if (currentOrientation != Screen.orientation)
        {
            currentOrientation = Screen.orientation;
            AdjustCameraAndCardSize();
        }
    }

    
    public void SetupGrid(int gridRows, int gridColumns)
    {

        int wider = Mathf.Max(gridRows, gridColumns);
        int thinner = Mathf.Min(gridRows, gridColumns);

        // rows * columns must be even because of the card matching
        if ((gridRows * gridColumns) % 2 == 1)
        {
            thinner += 1;
            Debug.Log("The smaller grid dimension is increased by one in order to get even card number");
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

        ClearGrid();
        GenerateGrid();
        AdjustCameraAndCardSize(); // Adjust camera and card size to fit any screen size
        AssignImagesToCards(); //Assign images and uniqe keys to the cards
    }

    public IEnumerator SetupGridFromSavedData(int gridRows, int gridColumns, List<Vector3> cardPositions)
    {
        rows = gridRows;
        columns = gridColumns;
        ClearGrid();
        AdjustCameraAndCardSize();
        //Instantiate and put the cards to the saved positions
        for (int i = 0; i < cardPositions.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardPositions[i], Quaternion.identity);
            card.transform.parent = this.transform;
            cards.Add(card);
            yield return new WaitForSeconds(0.1f);

        }
        
        AssignImagesToCards();
    }

    public void GenerateGrid()
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
        cardHandler.clearCardQueue();
        foreach (GameObject card in cards)
        {
            Destroy(card);
        }
        cards.Clear();
    }



    private void AdjustCameraAndCardSize()
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

    //public void UpdateGridSize(int newRows, int newColumns)
    //{
    //    rows = newRows;
    //    columns = newColumns;
    //    setupGrid();
    //}

    void LoadCardImages()
    {
        // Load all images from the Resources folder
        Sprite[] loadedImages = Resources.LoadAll<Sprite>("Textures/Cards");
        imageDictionary = new Dictionary<int, Sprite>();

        // Create a dictionary from the images with unique keys. These uniqe keys will serve as IDs for the cards.
        for (int i = 0; i < loadedImages.Length; i++)
        {
            imageDictionary.Add(i, loadedImages[i]);
        }

    }


    void AssignImagesToCards() // randomly assign images to the cards 
    {
        int totalCards = rows * columns;

        List<int> imageKeys = new List<int>();

        // Randomly select images to fill all card slots
        for (int i = 0; i < totalCards / 2; i++)
        {
            int randomKey = Random.Range(0, imageDictionary.Count);
            imageKeys.Add(randomKey);
            imageKeys.Add(randomKey);
        }

        // Shuffle the pair keys
        ShuffleList(imageKeys);

        // Assign images to cards
        for (int i = 0; i < cards.Count; i++)
        {
            int imageKey = imageKeys[i];
            Sprite assignedImage = imageDictionary[imageKey];

            // Set the texture on the card's front face
            Transform frontFace = cards[i].transform.Find("Front");
            if (frontFace != null)
            {
                Renderer renderer = frontFace.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.mainTexture = assignedImage.texture;
                }
            }

            // Assign the ID to the card for matching logic
            Card cardComponent = cards[i].GetComponent<Card>();
            if (cardComponent != null)
            {
                cardComponent.id = imageKey;
            }
        }
    }


    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


    public List<GameObject> getCards()
    {
        return cards;
    }

}
