using UnityEngine;
using System.Collections;

/// <summary>
/// Generic State
/// These could be shared among different agents to save memory.
/// Pass in the owner to modify agent data.
/// </summary>
public class StateGeneric : Entity
{
    public eStateName stateName = eStateName.PLAYER_FALLING;
    
    // -----------------------------------------------------------------------

    /// <summary>
    /// Generic Message Handler for states.
    /// Derived classes should cast owner to the proper type to interact.
    /// </summary>
    /// <param name="message"></param>
    /// <returns>true if message is successfully handled</returns>
    public virtual bool OnMessage(Message message, Agent owner)
    {
        if (owner == null)
        {
            return false;
        }

        return false;
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Called by State Machine when state is entered.
    /// </summary>
    /// <param name="owner"></param>
    public virtual void Enter(Agent owner)
    {
        if (owner == null)
        {
            return;
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Called by State Machine when state is exited.
    /// </summary>
    /// <param name="owner"></param>
    public virtual void Exit(Agent owner)
    {
        if (owner == null)
        {
            return;
        }
    }

    // -----------------------------------------------------------------------

    // Execute is called once per frame
    public virtual void Execute(Agent owner)
    {
        if (owner == null)
        {
            return;
        }
    }

    // -----------------------------------------------------------------------

    protected override void Start()
    {
        base.Start();
    }
}
