using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.Log("UIManager is Null!");

            return _instance;
        }
    }

    [SerializeField] GameObject _startmenuCanvas = null;
    [SerializeField] TextMeshProUGUI tmpro = null;
    [SerializeField] TextMeshProUGUI timeTmpro = null;

    private int timerTime = 0;

    private bool startTimer = false;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        GameManager.OnStart += GameManager_OnStart;
        GameManager.OnChangeLevel += GameManager_OnChangeLevel;
        GameManager.OnGameOver += GameManager_OnGameOver;
    }

    private void GameManager_OnGameOver()
    {
        startTimer = false;

    }

    private void GameManager_OnChangeLevel()
    {
        tmpro.text = GameManager.Instance.currentLevel.ToString();
        timerTime = 0;
        startTimer = true;
    }

    private void GameManager_OnStart()
    {
        _startmenuCanvas.SetActive(false);
    }

    private void Update()
    {
        if (startTimer)
        {
            if (Time.time - GameManager.Instance.levelStartTime > timerTime + 1)
            {
                timerTime++;
                SetTimerText();
                if (Time.time - GameManager.Instance.levelStartTime >= GameManager.Instance.currentLevelDuration) startTimer = false;
            }
        }
    }

    private void SetTimerText()
    {
        timeTmpro.text = timerTime.ToString();
    }
}
