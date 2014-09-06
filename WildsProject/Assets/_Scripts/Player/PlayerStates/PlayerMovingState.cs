using UnityEngine;
using System.Collections;

public class PlayerMovingState : PlayerBaseState {
    
    protected Vector3 m_destination;

    /// <summary>
    /// Generic Message Handler for states.
    /// Derived classes should cast owner to the proper type to interact.
    /// </summary>
    /// <param name="message"></param>
    /// <returns>true if message is successfully handled</returns>
    public override bool OnMessage(Message message, Agent owner)
    {
        if (owner == null)
        {
            return false;
        }
        switch (message.mType)
        {
            case eMessageType.INPUT:
               
                InputData iData = (InputData)message.data;
                if (iData.worldDestination.sqrMagnitude > 0.01f)
                {
                    if (iData.leftClick == eButtonState.DOWN || iData.leftClick == eButtonState.HOLD)
                    {
                        // If we have actually hit terrain, instead of UI

                        if (iData.shiftClick)
                        {
                            owner.Attack(eAbilitySlot.SLOT1, iData.worldDestination);
                        }
                        else
                        {
                            owner.Destination = iData.worldDestination;
                        }

                    }
                    else if (iData.leftClick == eButtonState.UP)
                    {
                        owner.StopAttack();
                    }
                }
                return true;
            default:
                break;
        }
        return false;
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Called by State Machine when state is entered.
    /// </summary>
    /// <param name="owner"></param>
    public override void Enter(Agent owner)
    {
        if (owner == null)
        {
            return;
        }
        base.Enter(owner);
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Called by State Machine when state is exited.
    /// </summary>
    /// <param name="owner"></param>
    public override void Exit(Agent owner)
    {
        if (owner == null)
        {
            return;
        }
        base.Exit(owner);
        
        game.ui.HideDestinationMarker();
        owner.StopMoving();
    }

    // -----------------------------------------------------------------------

    // Execute is called once per frame
    public override void Execute(Agent owner)
    {
        if (owner == null)
        {
            return;
        }
        ///MOVEMENT IS TEMP
        float sqrDistance = Vector3.SqrMagnitude(this.transform.position - owner.Destination);
        if (sqrDistance < 0.4f)
        {
            owner.StateTransition(eStateName.PLAYER_IDLE);
        }
        else
        {
            owner.Move();            
        }
    }
}
