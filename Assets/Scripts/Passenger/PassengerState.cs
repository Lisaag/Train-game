using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerState
{ 
    public enum State
    {
        unaware,
        confused,
        suspicious,
        highAlert
    }

    protected enum Phase
    {
        Enter, Update, Exit
    }

    public State currentState;
    public Sprite reactionSprite;
    protected Phase phase;
    protected PassengerState nextState;
    protected PassengerBehaviour passengerBehaviour;
    public PassengerState(PassengerBehaviour passengerBehaviour)
    {
        this.passengerBehaviour = passengerBehaviour;
        phase = Phase.Enter;
    }

    public virtual void EnterState()
    {
        //Play animation
        passengerBehaviour._emotionSprite.sprite = reactionSprite;
        phase = Phase.Update;

        passengerBehaviour.ReturnPreviousState();
    }

    public virtual void Update()
    {
        phase = Phase.Update;
    }

    public virtual void ReturnToPreviousState() { }

    public virtual void GoToNextState() { }

    public virtual void ExitState()
    {
        phase = Phase.Exit;
    }

    public virtual PassengerState Process()
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
}

public class UnawareState : PassengerState
{
    public UnawareState(PassengerBehaviour passengerBehaviour) : base(passengerBehaviour)
    {
        reactionSprite = null;
        currentState = State.unaware;
    }

    public override void GoToNextState()
    {
        nextState = new ConfusedState(passengerBehaviour);
        phase = Phase.Exit;
    }
}

public class ConfusedState : PassengerState
{
    public ConfusedState(PassengerBehaviour passengerBehaviour) : base(passengerBehaviour)
    {
        reactionSprite = PassengerManager.passengerProperties.confused;
        currentState = State.confused;
    }

    public override void EnterState()
    {
        base.EnterState();
    }
    public override void ReturnToPreviousState()
    {
        nextState = new UnawareState(passengerBehaviour);
        phase = Phase.Exit;
    }

    public override void GoToNextState()
    {
        nextState = new SuspiciousState(passengerBehaviour);
        phase = Phase.Exit;
    }
}

public class SuspiciousState : PassengerState
{
    public SuspiciousState(PassengerBehaviour passengerBehaviour) : base(passengerBehaviour)
    {
        reactionSprite = PassengerManager.passengerProperties.suspicious;
        currentState = State.suspicious;
    }

    public override void ReturnToPreviousState()
    {
        nextState = new ConfusedState(passengerBehaviour);
        phase = Phase.Exit;
    }

    public override void GoToNextState()
    {
        nextState = new HighAlertState(passengerBehaviour);
        phase = Phase.Exit;
    }
}

public class HighAlertState : PassengerState
{
    public HighAlertState(PassengerBehaviour passengerBehaviour) : base(passengerBehaviour)
    {
        reactionSprite = PassengerManager.passengerProperties.highAlert;
        currentState = State.highAlert;
    }

    public override void EnterState()
    {
        GameManager.Instance.GameOver();
        base.EnterState();
    }

    //public override void ReturnToPreviousState()
    //{
    //    nextState = new SuspiciousState(passengerBehaviour);
    //    phase = Phase.Exit;
    //}


}
