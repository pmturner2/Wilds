using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// State Machine. Controls transitioning between states.
/// Agents own an instance of this.
/// </summary>
public class StateMachine : Entity
{
    // Do we use a stack? Stack lets us pop back down, but COSTS MEMORY
    public bool useStack = false;

    // Default State
    public eStateName defaultState = eStateName.MENU_NONE;

    // Dump debug output?
    public bool stateDebug = false;

    // -----------------------------------------------------------------------

    protected StateGeneric m_baseState = null;
    protected StateGeneric currentState = null;
    protected StateGeneric nextState = null;
    protected StateGeneric prevState = null;

    protected Agent owner = null;

    protected float stateStartTime = 0.0f;
    protected Stack<StateGeneric> m_stack = null;
    protected bool m_goingBack = false;
    protected float transitionTimeRemaining = 0.0f;
    protected bool waitingTransition = false;

    // If there are child elements that have states, they are put here.
    protected Dictionary<eStateName, StateGeneric> allStates = null;
   
    // -----------------------------------------------------------------------

    /// <summary>
    /// Just throws a reference onto a stack
    /// </summary>
    public StateGeneric PopState()
    {
        return m_stack.Pop();
    }

    // -----------------------------------------------------------------------

    /// <summary>
    /// Just throws a reference onto a stack
    /// </summary>
    /// <param name="?"></param>
    public void PushState(StateGeneric s)
    {
        m_stack.Push(s);
    }

    // -----------------------------------------------------------------------

    public float GetCurrentStateTime()
    {
        return Time.time - stateStartTime;
    }

    // -----------------------------------------------------------------------

    public bool IsActive()
    {
        return currentState == null;
    }

    // -----------------------------------------------------------------------

    public virtual void SetNextState(StateGeneric next)
    {
        if (m_baseState == null)
        {
            m_baseState = next;
        }
        nextState = next;
    }

    // -----------------------------------------------------------------------

    public void SetNextState(eStateName newState)
    {
        if (allStates.ContainsKey(newState))
        {
            SetNextState(allStates[newState]);
        }
    }

    // -----------------------------------------------------------------------

    public virtual void GotoLastState()
    {
        if (m_stack.Count > 0)
        {
            StateGeneric lastState = useStack ? m_stack.Pop() : prevState;
            if (lastState != null)
            {
                m_goingBack = true;
                SetNextState(lastState);
            }
        }
        else
        {
            GotoBaseState();
        }
    }

    // -----------------------------------------------------------------------

    public virtual void GotoBaseState()
    {
        SetNextState(m_baseState);
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// (1) Check if we have marked a next state to transition to. If so, transition.
    /// (2) Run Execute() on the current state.
    /// </summary>
    public virtual void Execute()
    {
        
        if (nextState != null)
        {
            StateTransition(nextState);
            nextState = null;
        }

        // In some cases, we might set a delay for transitioning.
        // Usually, it will be 0
        transitionTimeRemaining -= Time.deltaTime;
        if (transitionTimeRemaining < 0.0f && waitingTransition)
        {
            currentState.Enter(owner);
            stateStartTime = Time.time;
            waitingTransition = false;           
        }

        if (currentState != null && !waitingTransition)
        {
            currentState.Execute(owner);
        }
    }

    // -----------------------------------------------------------------------

    public void SetOwner(Agent owner)
    {
        this.owner = owner;
    }

    // -----------------------------------------------------------------------

    public StateGeneric GetCurrentState()
    {
        return currentState;
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Handle messages
    /// </summary>
    /// <param name="message"></param>
    /// <returns>TRUE if we handle the message succesfully</returns>
    public virtual bool HandleMessage(Message message)
    {

        if (currentState && currentState.OnMessage(message, owner))
        {
            return true;
        }
        return false;
    }

    // -----------------------------------------------------------------------

    public void SetTransitionTime(float f)
    {
        transitionTimeRemaining = f;
    }

    // -----------------------------------------------------------------------

    public StateGeneric GetLastState()
    {
        return prevState;
    }

    // -----------------------------------------------------------------------

    public StateGeneric GetNextState()
    {
        return nextState;
    }

    // -----------------------------------------------------------------------

    public Dictionary<eStateName, StateGeneric> GetAllStates()
    {
        return allStates;
    }

    // -----------------------------------------------------------------------

   
    protected void StateTransition(eStateName newState)
    {
        if (allStates.ContainsKey(newState))
        {
            StateTransition(allStates[newState]);
        }
    }

    // -----------------------------------------------------------------------

    protected void StateTransition(StateGeneric newState)
    {
        if (stateDebug)
        {
            Debug.Log(name + " Transitioning From " + currentState + " To " + newState);
        }
        if (currentState)
        {
            if (!m_goingBack && useStack)
            {
                m_stack.Push(currentState);
            }
            m_goingBack = false;
            prevState = currentState;
            currentState.Exit(owner);

        }
        waitingTransition = true;
        currentState = newState;
    }

   
    // -----------------------------------------------------------------------

    protected override void Start()
    {
        base.Start();
        if (useStack)
        {
            m_stack = new Stack<StateGeneric>();
        }
        
        allStates = new Dictionary<eStateName, StateGeneric>();
        StateGeneric[] components = GetComponentsInChildren<StateGeneric>();
        foreach (StateGeneric c in components)
        {
            allStates.Add(c.stateName, c);
        }
        StateTransition(defaultState);
    }

}
