using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputBehaviour : MonoBehaviour
{
    [SerializeField] public PlayerProperties playerPropertiesField = null;
    

    public PlayerProperties playerProperties = null;
    public PlayerInputActions playerInputActions;
    public Rigidbody2D playerRB;
    public CameraMovement cameraMovement = null;
    public AudioSource playerAudioSource = null;

    public PlayerState playerState;

    public delegate void WhistleAction();
    public static event WhistleAction OnWhistle;

    private void Awake()
    {
        playerProperties = playerPropertiesField;

        playerRB = GetComponent<Rigidbody2D>();
        playerAudioSource = GetComponent<AudioSource>();

        cameraMovement = Camera.main.GetComponent<CameraMovement>();

        playerRB.gravityScale = playerProperties._defaultGravity;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerState = new IdleState(this);

        playerInputActions.Player.Jump.performed += HandleInput;
        playerInputActions.Player.Jump.canceled += HandleInput;
        playerInputActions.Player.Grab.performed += HandleInput;
        playerInputActions.Player.SwitchWindow.performed += HandleInput;
        playerInputActions.Player.Whistle.performed += Whistle_performed;
    }

    private void Whistle_performed(InputAction.CallbackContext obj)
    {
        if (!playerAudioSource.isPlaying)Whistle();
    }

    private void Whistle()
    {
        playerAudioSource.Play();
        OnWhistle();
    }

    public void HandleInput(InputAction.CallbackContext obj)
    {
        playerState.HandleInput(obj);
    }

    private void FixedUpdate()
    {
        playerState = playerState.Process();
    }

    //private void MoveCharacterToHandlePosition()
    //{
    //    float spd = 2.5f * Time.deltaTime;

    //    Vector2 target = PlayerCollisionHandler.instance.currentHandle.transform.GetChild(0).position;
    //    Vector2 f = Vector2.MoveTowards(transform.position, target, spd);
    //    Vector2 dif = transform.position - transform.position;
    //    transform.position = dif + f;

    //    if (Vector2.Distance(transform.position, target) < 0.004f)
    //    {
    //       // AttachPlayerToHandle();
    //    }
    //}
}