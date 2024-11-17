using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    public GameObject scorePopupPrefab;

    [SerializeField] private GameObject paramPanel;      
    [SerializeField] private GameObject startPanel;      
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private InputField widthInput;    // Input field for grid width
    [SerializeField] private InputField heightInput;   // Input field for grid height

    [SerializeField] private Text timerText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text turnsText;
    [SerializeField] private Text hitsText;

    [SerializeField] private GridHandler gridHandler;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnOkButtonClick() // This starts the game after it reads out the grid sizes
    {
        // Read grid values from the input fields
        if (heightInput.text != string.Empty && widthInput.text != string.Empty)
        {
            int gridWidth;
            int gridHeight;
            bool check1 = int.TryParse(widthInput.text, out gridWidth);
            bool check2 = int.TryParse(heightInput.text, out gridHeight);

            //Start the game
            if (check1 && check2)
            {

                GameManager.Instance.ResetParameters();
                GameManager.Instance.StartTimer();
                paramPanel.SetActive(false);
                pauseButton.SetActive(true);
                gridHandler.SetupGrid(gridWidth, gridHeight);
                SoundManager.Instance.PlaySound(SoundManager.SoundAction.GameStart);

            }
            else
            {
                Debug.LogError("Invalid input. Please enter a valid number.");
            }
        }
        else
        {
            Debug.LogError("Input field is empty or not assigned.");
        }

    }

    public void OnPauseButtonClick()
    {
        pauseButton.SetActive(false);
        pausePanel.SetActive(true);
        GameManager.Instance.PauseTimer();

    }

    public void OnStartButtonClick()
    {
        // If there is no cards yet go to the grid input panel
        if(FindObjectsOfType<Card>().Length == 0)
        {
            paramPanel.SetActive(true);
            startPanel.SetActive(false);
        }
        else //if there are already cards it means that the game is already loaded. So we can start it.

        {
            startPanel.SetActive(false);
            pauseButton.SetActive(true);
            GameManager.Instance.StartTimer();
        }



    }

    public void OnLoadButtonClick()
    {
        GameData gameData = GameManager.Instance.LoadGame();

        if (gameData != null)
        {
            StartCoroutine(gridHandler.SetupGridFromSavedData(gameData.Rows, gameData.Columns, gameData.CardPositions));


            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("There is no saved game!");
        }
    }

    public void OnSaveButtonClick()
    {

        GameManager.Instance.SaveGame(gridHandler.rows, gridHandler.columns);
        Debug.Log("Game is saved");

    }

    public void OnQuitButtonClick()
    {
        Debug.Log("Quit game requested.");
        Application.Quit();
    }

    public void OnResumeButtonClick()
    {
        pauseButton.SetActive(true);
        pausePanel.SetActive(false);
        GameManager.Instance.StartTimer();
    }

    public void OnRestartButtonClick()
    {
        paramPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        startPanel.SetActive(true);
    }

    public void UpdateScoreDisplay(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateHitsDisplay(int hits)
    {
        hitsText.text = "Hits: " + hits;
    }

    public void UpdateTurnsDisplay(int turns)
    {
        turnsText.text = "Turns: " + turns;
    }


    public void UpdateTimerDisplay(float elapsedTime)
    {
        timerText.text = FormatTime(elapsedTime);
    }

    public void ShowScorePopup(string text, Vector3 position, Color textColor)
    {
        GameObject popup = Instantiate(scorePopupPrefab, position, Quaternion.identity, transform);
        ScorePopup scorePopup = popup.GetComponent<ScorePopup>();
        scorePopup.Initialize(position, text, textColor);
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
