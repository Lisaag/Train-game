using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerBehaviour : MonoBehaviour
{
    [SerializeField] public SpriteRenderer _emotionSprite = null;


    [SerializeField] float _timeToCalmDown = 2f;

    public PassengerState passengerState;

    private void Awake()
    {
        passengerState = new UnawareState(this);
    }

    private void Start()
    {
        GameManager.OnChangeLevel += GameManager_OnChangeLevel;
        PlayerInputBehaviour.OnWhistle += PlayerInputBehaviour_OnWhistle;
    }

    private void PlayerInputBehaviour_OnWhistle()
    {
        if (GameManager.Instance.isLevelStarted) passengerState.GoToNextState();
    }

    private void GameManager_OnChangeLevel()
    {
        Reset();
    }

    private void Reset()
    {
        passengerState = new UnawareState(this);
    }

    private void Update()
    {
        passengerState = passengerState.Process();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.isLevelStarted)
        {
            if (other.CompareTag("Player"))
            {
                PlayerState.State currentState = other.GetComponent<PlayerInputBehaviour>().playerState.currentState;
                if (currentState == PlayerState.State.isRunning || currentState == PlayerState.State.isJumping || currentState == PlayerState.State.isFalling)
                {
                    passengerState.GoToNextState();
                }
            }
        }
    }

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        PlayerState.State currentState = other.GetComponent<PlayerInputBehaviour>().playerState.currentState;
    //        if (currentState == PlayerState.State.isRunning || currentState == PlayerState.State.isJumping || currentState == PlayerState.State.isFalling)
    //        {
    //            //StopCoroutine(GetTriggered());
    //            //StartCoroutine(GetTriggered());
    //        }
    //    }
    //}

    public void ReturnPreviousState()
    {
        StopAllCoroutines();
        StartCoroutine(WaitReturnPreviousState());
    }
    private IEnumerator WaitReturnPreviousState()
    {
        //_emotionSprite.sprite = _passengerProperties.confused;

        //_emotionSprite.sprite = passengerState.reactionSprite;

        yield return new WaitForSeconds(_timeToCalmDown);

        passengerState.ReturnToPreviousState();
        //_emotionSprite.sprite = passengerState.reactionSprite;
        //_emotionSprite.sprite = null;
    }
}
