using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScrolling : MonoBehaviour
{
    [SerializeField] private RawImage _bgimg = null;
    [SerializeField] private float _x, _y;
    [SerializeField] private float acceleration = 0.1f;

    private bool isScrolling = false;

    /// <summary>
    /// /
    /// </summary>

    private float startTime = 0f;
    private float timeToSpeedUp = 10f;
    private float timeToSlowDown = 5f;
    private float maxSpeed = 150f;

    bool speedUp = true;
    bool speedDown = false;


    private void Start()
    {
        GameManager.OnChangeLevel += GameManager_OnChangeLevel;
        GameManager.OnGameOver += GameManager_OnGameOver;
    }

    private void GameManager_OnGameOver()
    {
        isScrolling = false;
    }

    private void GameManager_OnChangeLevel()
    {
        Reset();
        isScrolling = true;
    }

    private void Reset()
    {
        speedUp = true;
        speedDown = false;
        startTime = Time.time;
    }

    private void Update()
    {
        if (isScrolling)
        {
            if (speedUp)
            {
                float time = Time.time - startTime;

                float speed = Mathf.Sqrt(maxSpeed) * (time / timeToSpeedUp);

                speed = Mathf.Pow(speed, 2f);

                speed = Mathf.Clamp(speed, 0f, maxSpeed);

                _bgimg.uvRect = new Rect(_bgimg.uvRect.position - new Vector2(speed * 0.001f, _y) * Time.deltaTime, _bgimg.uvRect.size);

                if (speed == 150f) speedUp = false;
            }
            else if (speedDown)
            {
                float time = Time.time - startTime;

                float speed = Mathf.Sqrt(maxSpeed) * (time / timeToSlowDown);

                speed = Mathf.Pow(speed, 2f);

                speed = Mathf.Clamp(speed, 0f, maxSpeed);

                _bgimg.uvRect = new Rect(_bgimg.uvRect.position - new Vector2((maxSpeed - speed) * 0.001f, _y) * Time.deltaTime, _bgimg.uvRect.size);

                if (speed == 0f)
                {
                    isScrolling = false;
                }
            }
            else
            {
                _bgimg.uvRect = new Rect(_bgimg.uvRect.position - new Vector2(maxSpeed * 0.001f, _y) * Time.deltaTime, _bgimg.uvRect.size);
                if (Time.time - GameManager.Instance.levelStartTime > GameManager.Instance.currentLevelDuration - timeToSlowDown)
                {
                    startTime = Time.time;
                    speedDown = true;
                }
            }
        }
    }
}
