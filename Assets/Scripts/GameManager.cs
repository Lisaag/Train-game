using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.Log("Game manager is Null!");

            return _instance;
        }
    }
    public delegate void StartAction();
    public static event StartAction OnStart;

    public delegate void LevelFinishedAction();
    public static event LevelFinishedAction OnLevelFinished;

    public delegate void ChangeLevel();
    public static event ChangeLevel OnChangeLevel;

    public delegate void GameOverAction();
    public static event GameOverAction OnGameOver;


    [SerializeField] public int currentLevel = 0;
    [SerializeField] public List<LevelProperties> _levels = new List<LevelProperties>();

    public float currentLevelDuration = 0f;

    public float levelStartTime;

    public bool isLevelStarted = false;

    private void Awake()
    {
        _instance = this;
    }

    public void StartGame()
    {
        if (OnStart != null) OnStart();
    }

    public void GameOver()
    {
        isLevelStarted = false;
        if (OnGameOver != null)
        {
            OnGameOver();
        }
    }

    public void StartLevel()
    {
        currentLevelDuration = _levels[currentLevel].levelDuration;

        Debug.Log("START NEW LEVEL, seconds: " + currentLevelDuration);

        levelStartTime = Time.time;
        isLevelStarted = true;
        if (OnChangeLevel != null) OnChangeLevel();
    }

    private void Update()
    {
        if (isLevelStarted)
        {
            if (Time.time - levelStartTime >= currentLevelDuration)
            {
                Debug.Log("LEVEL END: " + (Time.time - levelStartTime));
                isLevelStarted = false;
                currentLevel++;
                if (OnLevelFinished != null) OnLevelFinished();
            }
        }
    }

}
