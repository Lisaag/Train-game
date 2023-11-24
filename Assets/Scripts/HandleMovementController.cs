using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandleMovementController : MonoBehaviour
{
    [SerializeField] private float impulseStrength = 1.0f;

    Rigidbody2D rb = null;
    PlayerInputActions playerInputActions;

    float currentDir = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = playerInputActions.Player.Swing.ReadValue<Vector2>();
        if (inputVector.x != 0) {
            if(Mathf.Sign(inputVector.x) != currentDir)
            {
                Debug.Log(Mathf.Sign(inputVector.x));
                currentDir = Mathf.Sign(inputVector.x);
                rb.AddForce(new Vector2(currentDir * impulseStrength, 0f), ForceMode2D.Impulse);
            }

        }
    }
}
