using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProperties", menuName = "ScriptableObjects/PlayerPropertiesScriptableObject", order = 1)]
public class PlayerProperties : ScriptableObject
{
    [Header("Running")]
    [SerializeField] public float _movementSpeed = 5f;
    [SerializeField] public float _acceleration = 7f;
    [SerializeField] public float _decceleration = 14f;
    [SerializeField] public float _lerpAmount = 0.5f;
    [SerializeField] public float _runningTreshhold = 0.8f;

    [Space(20)]

    [Header("Jumping")]
    [SerializeField] public float _jumpPower = 10f;
    [SerializeField] public float _maxFallSpeed = 2f;
    [SerializeField] public float _jumpGravity = 2.2f;
    [SerializeField] public float _defaultGravity = .8f;
    [SerializeField] public Vector2 _wallJumpForce = new Vector2(7f, 5f);

    [Space(20)]

    [Header("Swinging")]
    [SerializeField] public float _swingStrength = 3f;
    [SerializeField] public Vector2 _swingJumpForce = new Vector2(5f, 6f);
    [SerializeField] public Vector2 _swingJumpMax = new Vector2(8f, 8f);
    [SerializeField] public Vector2 _swingJumpMin = new Vector2(4f, 4f);

    [Space(20)]
    [Header("Climbing")]
    [SerializeField] public float _climbingSpeed = 5f;
    [SerializeField] public Vector2 _poleJumpForce = new Vector2(5f, 5f);

}
