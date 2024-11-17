using UnityEngine;
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
                gridHandler.setupGrid(gridWidth, gridHeight);
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

    public void onPauseButtonClick()
    {
        pauseButton.SetActive(false);
        pausePanel.SetActive(true);
        GameManager.Instance.PauseTimer();

    }

    public void onStartButtonClick()
    {
        paramPanel.SetActive(true);
        startPanel.SetActive(false);

    }

    public void onLoadButtonClick()
    {

    }

    public void onSaveButtonClick()
    {


    }

    public void onQuitButtonClick()
    {
        Debug.Log("Quit game requested.");
        Application.Quit();
    }

    public void onResumeButtonClick()
    {
        pauseButton.SetActive(true);
        pausePanel.SetActive(false);
        GameManager.Instance.StartTimer();
    }

    public void onRestartButtonClick()
    {
        paramPanel.SetActive(true);
        pausePanel.SetActive(false);
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
