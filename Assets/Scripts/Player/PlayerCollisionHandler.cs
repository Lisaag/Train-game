using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [Header("Collision checks")]
    [SerializeField] private Transform _groundCheck;
    private Vector2 groundCheckSize = new Vector2(0.3f, 0.03f);
    [SerializeField] private Transform _frontWallCheck;
    [SerializeField] private Transform _backWallCheck;
    private Vector2 wallCheckSize = new Vector2(.5f, .5f);
    [SerializeField] private Transform _grabCheck;
    [SerializeField] private Vector2 handCheckSize = new Vector2(.2f, .2f);
    [SerializeField] private Transform _handCheck;

    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _handleLayer;
    [SerializeField] private LayerMask _poleLayer;
    [SerializeField] private LayerMask _wallLayer;

    public static PlayerCollisionHandler instance = null;

    [HideInInspector] public Collider2D currentWall = null;
    [HideInInspector] public Collider2D currentHandle = null;
    [HideInInspector] public Rigidbody2D currentHandleRB = null;
    [HideInInspector] public BoxCollider2D playerCollider = null;

    private void Awake()
    {
        instance = this;
        playerCollider = GetComponent<BoxCollider2D>();
    }

    public bool IsTouchingWall()
    {
        currentWall = Physics2D.OverlapBox(_frontWallCheck.position, wallCheckSize, 0, _wallLayer);
        if (currentWall != null) return true;
        return false;
    }

    public bool IsTouchingGround()
    {
        if (Physics2D.OverlapBox(_groundCheck.position, groundCheckSize, 0, _groundLayer)) return true;
        return false;
    }

    public bool IsTouchingPole()
    {
        if (Physics2D.OverlapBox(transform.position, playerCollider.bounds.size, 0, _poleLayer)) return true;
        return false;
    }

    public bool IsTouchingHandle()
    {
        currentHandle = Physics2D.OverlapBox(_grabCheck.position, handCheckSize, 0, _handleLayer);
        if (currentHandle != null)
        {
            currentHandleRB = currentHandle.GetComponent<Rigidbody2D>();
            return true;
        }
        return false;
    }

    
}
