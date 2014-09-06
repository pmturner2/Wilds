using UnityEngine;
using System.Collections;

public class PlayerBaseState : StateGeneric {
    public string animationTrigger = "";

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
        if (animationTrigger != "")
        {
            owner.TriggerAnimation(animationTrigger);
        }
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
    }

    // -----------------------------------------------------------------------
}
