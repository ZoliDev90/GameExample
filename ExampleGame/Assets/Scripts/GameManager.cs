using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;


    private float elapsedTime = 0f;
    private bool isRunning = false;
    private int hitCount = 0;
    private int turnCount = 0;
    private int score = 0;


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
           elapsedTime += Time.deltaTime;
           UIManager.Instance.UpdateTimerDisplay(elapsedTime);
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
        elapsedTime = 0f;
        UIManager.Instance.UpdateTimerDisplay(elapsedTime);
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
        UIManager.Instance.ShowScorePopup("+10", position, Color.green);
        UpdateScore(10);
    }

    public void UpdateTurnCount()
    {
        turnCount += 1;
        UIManager.Instance.UpdateTurnsDisplay(turnCount);
    }

    public void ResetParameters()
    {
        ResetTimer();
        score = 0;
        hitCount = 0;
        turnCount = 0;
        UIManager.Instance.UpdateScoreDisplay(score);
        UIManager.Instance.UpdateHitsDisplay(hitCount);
        UIManager.Instance.UpdateTurnsDisplay(turnCount);
    }



}
