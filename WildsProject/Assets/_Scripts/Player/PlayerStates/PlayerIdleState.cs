using UnityEngine;
using System.Collections;

public class PlayerIdleState : PlayerBaseState {
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
                            owner.StateTransition(eStateName.PLAYER_WALK);
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
    }

    // -----------------------------------------------------------------------

    // Execute is called once per frame
    public override void Execute(Agent owner)
    {
        if (owner == null)
        {
            return;
        }

    }
}
