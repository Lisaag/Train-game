using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState
{
    public enum State
    {
        isIdle,
        isWalking,
        isRunning,
        isStandingStill,
        isFalling,
        isJumping,
        isGrabbing,
        isHanging,
        isWallSliding,
        isClimbing,
        isWhistling
    }
    protected enum Phase
    {
        Enter, Update, Exit
    }

    public State currentState;
    protected Phase phase;
    protected PlayerState nextState;
    protected bool canJump;
    protected PlayerInputBehaviour playerInputBehaviour;

    public int currentDir;

    public PlayerState(PlayerInputBehaviour playerInputBehaviour, bool canJump = true)
    {
        this.canJump = canJump;
        this.playerInputBehaviour = playerInputBehaviour;
        currentDir = (int)Mathf.Sign(playerInputBehaviour.transform.localScale.x);
        phase = Phase.Enter;
    }

    public virtual void EnterState()
    {
        //Play animation
        phase = Phase.Update;
    }

    public virtual void Update()
    {
        phase = Phase.Update;
    }

    public virtual void ExitState()
    {
        phase = Phase.Exit;
    }

    public virtual PlayerState Process()
    {
        if (phase == Phase.Enter) EnterState();
        if (phase == Phase.Update) Update();
        if (phase == Phase.Exit)
        {
            ExitState();
            return nextState;
        }
        return this;
    }

    public virtual void HandleInput(InputAction.CallbackContext obj)
    {
        switch (obj.action.name)
        {
            case "Jump":
                if (!canJump) break;
                if (obj.action.phase == InputActionPhase.Performed)
                {
                    nextState = new JumpingState(playerInputBehaviour);
                    phase = Phase.Exit;
                }
                break;
            case "Movement":
                //Debug.Log("Move");
                break;
            case "Grab":
                if (obj.action.phase == InputActionPhase.Performed)
                {
                    if (PlayerCollisionHandler.instance.IsTouchingHandle())
                    {
                        nextState = new HangingState(playerInputBehaviour);
                        phase = Phase.Exit;
                    }
                    else if (PlayerCollisionHandler.instance.IsTouchingPole())
                    {
                        nextState = new ClimbingState(playerInputBehaviour);
                        phase = Phase.Exit;
                    }

                }
                break;
            case "SwitchWindow":
                if (PlayerCollisionHandler.instance.IsTouchingWall())
                {
                    if (PlayerCollisionHandler.instance.IsTouchingGround())
                    {
                        Collider2D currentWall = PlayerCollisionHandler.instance.currentWall;
                        float dir = Mathf.Sign(currentWall.transform.position.x - playerInputBehaviour.transform.position.x);
                        float inputDir = Mathf.Sign(obj.ReadValue<float>());
                        if (dir == inputDir)
                        {
                            if (currentWall.transform.GetSiblingIndex() == 0 || currentWall.transform.GetSiblingIndex() == currentWall.transform.parent.childCount - 1) return;
                            Vector2 newPos = new Vector2(playerInputBehaviour.transform.localPosition.x + ((Mathf.Abs(playerInputBehaviour.transform.localScale.x) + currentWall.transform.localScale.x) * dir), 0);
                            playerInputBehaviour.playerRB.position = newPos;
                            playerInputBehaviour.cameraMovement.MoveCamera((int)inputDir);
                        }
                    }
                }
                break;
            default:
                //Debug.Log("No action matches this input right now");
                break;
        }
    }

    protected void MoveCharacter()
    {
        Vector2 inputVector = playerInputBehaviour.playerInputActions.Player.Movement.ReadValue<Vector2>();
        if ((int)Mathf.Sign(inputVector.x) != currentDir && inputVector.x != 0f)
        {
            currentDir *= -1;
            Vector2 scale = playerInputBehaviour.transform.localScale;
            playerInputBehaviour.transform.localScale = new Vector2(scale.x * -1, scale.y);
        }

        float targetSpeed = inputVector.x * playerInputBehaviour.playerProperties._movementSpeed;

        float speedDif = targetSpeed - playerInputBehaviour.playerRB.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerInputBehaviour.playerProperties._acceleration : playerInputBehaviour.playerProperties._decceleration;

        Vector2 movement = speedDif * accelRate * Vector2.right;

        playerInputBehaviour.playerRB.AddForce(movement, ForceMode2D.Force);
    }
}

public class IdleState : PlayerState
{
    public IdleState(PlayerInputBehaviour playerInputBehaviour) : base(playerInputBehaviour)
    {
        currentState = State.isIdle;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void Update()
    {
        Vector2 inputVector = playerInputBehaviour.playerInputActions.Player.Movement.ReadValue<Vector2>();
        if (inputVector.x != 0)
        {
            nextState = new WalkingState(playerInputBehaviour);
            phase = Phase.Exit;
        }
    }
}

public class WalkingState : PlayerState
{
    public WalkingState(PlayerInputBehaviour playerInputBehaviour) : base(playerInputBehaviour)
    {
        currentState = State.isWalking;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void Update()
    {
        MoveCharacter();

        Vector2 inputVector = playerInputBehaviour.playerInputActions.Player.Movement.ReadValue<Vector2>();

        if (Mathf.Abs(inputVector.x) > playerInputBehaviour.playerProperties._runningTreshhold)
        {
            nextState = new RunningState(playerInputBehaviour);
            phase = Phase.Exit;
        }
        else if (inputVector.x == 0f && playerInputBehaviour.playerRB.velocity.x == 0f)
        {
            nextState = new IdleState(playerInputBehaviour);
            phase = Phase.Exit;
        }
    }
}

public class RunningState : PlayerState
{
    public RunningState(PlayerInputBehaviour playerInputBehaviour) : base(playerInputBehaviour)
    {
        currentState = State.isRunning;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void Update()
    {
        MoveCharacter();

        Vector2 inputVector = playerInputBehaviour.playerInputActions.Player.Movement.ReadValue<Vector2>();

        if (Mathf.Abs(inputVector.x) < playerInputBehaviour.playerProperties._runningTreshhold)
        {
            nextState = new WalkingState(playerInputBehaviour);
            phase = Phase.Exit;
        }
        else if (inputVector.x == 0f && playerInputBehaviour.playerRB.velocity.x == 0f)
        {
            nextState = new IdleState(playerInputBehaviour);
            phase = Phase.Exit;
        }
    }
}

public class JumpingState : PlayerState
{
    bool canWallSlide = false;

    public JumpingState(PlayerInputBehaviour playerInputBehaviour) : base(playerInputBehaviour)
    {
        canJump = false;
        currentState = State.isJumping;
    }

    public override void EnterState()
    {
        playerInputBehaviour.playerRB.AddForce(Vector2.up * playerInputBehaviour.playerProperties._jumpPower, ForceMode2D.Impulse);
        if (!PlayerCollisionHandler.instance.IsTouchingWall()) canWallSlide = true;
        base.EnterState();
    }

    public override void HandleInput(InputAction.CallbackContext obj)
    {
        if (obj.action.name == "Jump" && obj.action.phase == InputActionPhase.Canceled)
        {
            if (playerInputBehaviour.playerRB.velocity.y > 0f)
            {
                playerInputBehaviour.playerRB.gravityScale = 2.8f;
                nextState = new FallingState(playerInputBehaviour, canWallSlide);
                phase = Phase.Exit;
            }
        }

        base.HandleInput(obj);
    }

    public override void Update()
    {
        MoveCharacter();
        if (PlayerCollisionHandler.instance.IsTouchingWall() && canWallSlide)
        {
            nextState = new WallSlidingState(playerInputBehaviour);
            phase = Phase.Exit;
        }
        if (playerInputBehaviour.playerRB.velocity.y < 0)
        {
            playerInputBehaviour.playerRB.gravityScale = playerInputBehaviour.playerProperties._jumpGravity;
            nextState = new FallingState(playerInputBehaviour, canWallSlide);
            phase = Phase.Exit;
        }
    }
}

public class FallingState : PlayerState
{
    private bool canWallSlide;

    public FallingState(PlayerInputBehaviour playerInputBehaviour, bool canWallSlide = false) : base(playerInputBehaviour)
    {
        canJump = false;
        currentState = State.isFalling;
        this.canWallSlide = canWallSlide;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void Update()
    {
        MoveCharacter();
        if (PlayerCollisionHandler.instance.IsTouchingGround())
        {
            playerInputBehaviour.playerRB.gravityScale = playerInputBehaviour.playerProperties._defaultGravity;
            nextState = new WalkingState(playerInputBehaviour);
            phase = Phase.Exit;
        }
        else if (PlayerCollisionHandler.instance.IsTouchingWall() && canWallSlide)
        {
            nextState = new WallSlidingState(playerInputBehaviour);
            phase = Phase.Exit;
        }
    }
}

public class WallSlidingState : PlayerState
{
    public WallSlidingState(PlayerInputBehaviour playerInputBehaviour) : base(playerInputBehaviour)
    {
        currentState = State.isWallSliding;
        canJump = false;
    }

    public override void EnterState()
    {
        playerInputBehaviour.playerRB.gravityScale = .1f;
        playerInputBehaviour.playerRB.velocity = new Vector2(playerInputBehaviour.playerRB.velocity.x, 0f);
        base.EnterState();
    }

    public override void HandleInput(InputAction.CallbackContext obj)
    {
        if (obj.action.name == "Jump" && obj.action.phase == InputActionPhase.Performed)
        {
            playerInputBehaviour.playerRB.AddForce(new Vector2(playerInputBehaviour.playerProperties._wallJumpForce.x * -currentDir, playerInputBehaviour.playerProperties._wallJumpForce.y), ForceMode2D.Impulse);
            playerInputBehaviour.playerRB.gravityScale = playerInputBehaviour.playerProperties._jumpGravity;
        }

        base.HandleInput(obj);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void Update()
    {
        MoveCharacter();
        if (PlayerCollisionHandler.instance.IsTouchingGround())
        {
            Debug.Log("ONFLOOR");
            playerInputBehaviour.playerRB.gravityScale = playerInputBehaviour.playerProperties._defaultGravity;
            nextState = new IdleState(playerInputBehaviour);
            phase = Phase.Exit;
        }
        else if (!PlayerCollisionHandler.instance.IsTouchingWall())
        {
            playerInputBehaviour.playerRB.gravityScale = playerInputBehaviour.playerProperties._jumpGravity;
            nextState = new FallingState(playerInputBehaviour, true);
            phase = Phase.Exit;
        }
    }
}

public class ClimbingState : PlayerState
{
    public ClimbingState(PlayerInputBehaviour playerInputBehaviour) : base(playerInputBehaviour)
    {
        currentState = State.isClimbing;
    }

    public override void EnterState()
    {
        playerInputBehaviour.playerRB.velocity = new Vector2(0f, 0f);
        playerInputBehaviour.playerRB.gravityScale = 0f;
        base.EnterState();
    }

    public override void HandleInput(InputAction.CallbackContext obj)
    {
        if (obj.action.name == "Grab" && obj.action.phase == InputActionPhase.Performed)
        {
            playerInputBehaviour.playerRB.gravityScale = playerInputBehaviour.playerProperties._jumpGravity;
            nextState = new FallingState(playerInputBehaviour, true);
            phase = Phase.Exit;
        }
        else if (obj.action.name == "Jump" && obj.action.phase == InputActionPhase.Performed)
        {
            Vector2 inputVector = playerInputBehaviour.playerInputActions.Player.Movement.ReadValue<Vector2>();
            float jumpDir = inputVector.x == 0f ? 0f : Mathf.Sign(inputVector.x);
            playerInputBehaviour.playerRB.AddForce(new Vector2(playerInputBehaviour.playerProperties._poleJumpForce.x * jumpDir, playerInputBehaviour.playerProperties._poleJumpForce.y), ForceMode2D.Impulse);
            playerInputBehaviour.playerRB.gravityScale = playerInputBehaviour.playerProperties._jumpGravity;
            nextState = new FallingState(playerInputBehaviour, true);
            phase = Phase.Exit;
        }
    }

    public override void Update()
    {
        Vector2 inputVector = playerInputBehaviour.playerInputActions.Player.Movement.ReadValue<Vector2>();

        float movement = inputVector.y * playerInputBehaviour.playerProperties._climbingSpeed;

        playerInputBehaviour.playerRB.velocity = new Vector2(0f, movement);
    }
}

public class HangingState : PlayerState
{
    private bool canSwing = true;

    public HangingState(PlayerInputBehaviour playerInputBehaviour) : base(playerInputBehaviour)
    {
        currentState = State.isHanging;
    }

    public override void EnterState()
    {
        GrabHandle();
        base.EnterState();
    }

    public override void Update()
    {
        Vector2 inputVector = playerInputBehaviour.playerInputActions.Player.Movement.ReadValue<Vector2>();

        if (!canSwing)
        {
            if (inputVector.x == 0) canSwing = true;
        }

        if (canSwing && inputVector.x != 0)
        {
            canSwing = false;
            float swingDir = Mathf.Sign(inputVector.x);
            PlayerCollisionHandler.instance.currentHandleRB.AddForce(new Vector2(swingDir * playerInputBehaviour.playerProperties._swingStrength, 0f), ForceMode2D.Impulse);
        }
    }

    public override void HandleInput(InputAction.CallbackContext obj)
    {
        if (obj.action.name == "Grab" && obj.action.phase == InputActionPhase.Performed)
        {
            DetachFromHandle();
            playerInputBehaviour.playerRB.gravityScale = playerInputBehaviour.playerProperties._jumpGravity;
            nextState = new FallingState(playerInputBehaviour, true);
            phase = Phase.Exit;
        }
        else if (obj.action.name == "Jump" && obj.action.phase == InputActionPhase.Performed)
        {
            DetachFromHandle();
            Vector2 inputVector = playerInputBehaviour.playerInputActions.Player.Movement.ReadValue<Vector2>();
            float jumpDir = inputVector.x == 0f ? 0f : Mathf.Sign(inputVector.x);
            playerInputBehaviour.playerRB.AddForce(new Vector2(playerInputBehaviour.playerProperties._poleJumpForce.x * jumpDir, playerInputBehaviour.playerProperties._poleJumpForce.y), ForceMode2D.Impulse);
            playerInputBehaviour.playerRB.gravityScale = playerInputBehaviour.playerProperties._jumpGravity;
            nextState = new FallingState(playerInputBehaviour, true);
            phase = Phase.Exit;
        }
    }

    private void GrabHandle()
    {
        Collider2D currentHandle = PlayerCollisionHandler.instance.currentHandle;

        playerInputBehaviour.transform.SetParent(currentHandle.transform);
        FixedJoint2D joint = playerInputBehaviour.GetComponent<FixedJoint2D>();
        joint.connectedBody = PlayerCollisionHandler.instance.currentHandle.GetComponent<Rigidbody2D>();
        joint.enabled = true;

        playerInputBehaviour.playerRB.velocity *= 0f;
        playerInputBehaviour.playerRB.gravityScale = 0f;
        PlayerCollisionHandler.instance.currentHandleRB.AddForce(new Vector2(currentDir * playerInputBehaviour.playerProperties._swingStrength, 0f), ForceMode2D.Impulse);
    }

    private void DetachFromHandle()
    {
        playerInputBehaviour.transform.parent = null;
        FixedJoint2D joint = playerInputBehaviour.GetComponent<FixedJoint2D>();
        joint.connectedBody = null;
        joint.enabled = false;
    }
}