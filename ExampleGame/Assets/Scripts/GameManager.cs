using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private GameData currentGameData;

    private float timeElapsed = 0f;
    private bool isRunning = false;
    private int hitCount = 0;
    private int turnCount = 0;
    private int score = 0;

    private int hitsInRow = 0; // this is for combo
    private int mishitsInRow = 0; // this is for penalty

    public int comboThreshold = 2; // After 2 hits in a row bonus points added
    public int penaltyThreshold = 2; // After 2 mishits in a row penalty points deducted
    public int baseReward = 10;


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
  

    void Update()
    {
        if (isRunning)
        {
           timeElapsed += Time.deltaTime;
           UIManager.Instance.UpdateTimerDisplay(timeElapsed);
        }
    }


    public void StartTimer()
    {
        isRunning = true;
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        timeElapsed = 0f;
        UIManager.Instance.UpdateTimerDisplay(timeElapsed);
    }


    public void UpdateScore(int points)
    {
        score += points;
        UIManager.Instance.UpdateScoreDisplay(score);
    }

    public void UpdateHitCount(Vector3 position)
    {
        hitCount += 1;
        UIManager.Instance.UpdateHitsDisplay(hitCount);
        
        if(hitsInRow >= comboThreshold)
        {
            int point = baseReward * hitsInRow;
            UIManager.Instance.ShowScorePopup($"{hitsInRow}X Combo +{point}", position, Color.magenta);
            UpdateScore(point);
        }
        else
        {
            UIManager.Instance.ShowScorePopup($"+{baseReward}", position, Color.green);
            UpdateScore(baseReward);
        }


    }

    public void UpdateTurnCount(Vector3 position)
    {
        turnCount += 1;
        UIManager.Instance.UpdateTurnsDisplay(turnCount);

        if (mishitsInRow >= penaltyThreshold)
        {
            int point = baseReward / 2;
            UIManager.Instance.ShowScorePopup($"-{point}", position, Color.red);
            UpdateScore(-point);
        }
    }

    public void ResetParameters()
    {
        ResetTimer();
        score = 0;
        hitCount = 0;
        turnCount = 0;
        hitsInRow = 0;
        mishitsInRow = 0;
        UIManager.Instance.UpdateScoreDisplay(score);
        UIManager.Instance.UpdateHitsDisplay(hitCount);
        UIManager.Instance.UpdateTurnsDisplay(turnCount);
    }

    public void addMishitInRow() { 
        mishitsInRow += 1;
        hitsInRow = 0;
    }
    public void addHitsInRow() { 
        hitsInRow += 1;
        mishitsInRow = 0;
    }
    //public void resetMishitInRow() { mishitsInRow = 0; }
    //public void resetHitsInRow() { hitsInRow = 0; }



    public void SaveGame(int rows, int columns)
    {
        currentGameData = new GameData
        {
            TimeElapsed = timeElapsed,
            Score =  score,
            Turns = turnCount,
            Hits = hitCount,
            Rows = rows,
            Columns = columns,
            CardPositions = new List<Vector3>()
        };

        foreach (Card card in FindObjectsOfType<Card>())
        {
            currentGameData.CardPositions.Add(card.transform.position);
        }

        string json = JsonUtility.ToJson(currentGameData);
        File.WriteAllText(Application.persistentDataPath + "/savegame.json", json);
    }



    public GameData LoadGame()
    {
        string path = Application.persistentDataPath + "/savegame.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentGameData = JsonUtility.FromJson<GameData>(json);

            // Load the game state
            timeElapsed = currentGameData.TimeElapsed;
            score = currentGameData.Score;
            turnCount = currentGameData.Turns;
            hitCount = currentGameData.Hits;

            UIManager.Instance.UpdateTimerDisplay(timeElapsed);
            UIManager.Instance.UpdateHitsDisplay(hitCount);
            UIManager.Instance.UpdateTurnsDisplay(turnCount);
            UIManager.Instance.UpdateScoreDisplay(score);

            //return game data to initialize the saved cards
            return currentGameData;
        }

        return null;
    }


}

