using UnityEngine;
using System.Collections;

/// <summary>
/// Agents have state machines.
/// </summary>
public class Agent : MessageEntity {

    public AgentEnergy energy;

    public float maxHealth = 4f;
    public float movementSpeed = 3;
    
    // -----------------------------------------------------------------------
    
    protected float m_health = 2;

    protected StateMachine m_stateMachine = null;

    protected Animator m_anim = null;

    protected Vector3 m_destination;

    public Vector3 Destination
    {
        get { return m_destination; }
        set { m_destination = value; }
    }

    // public for now for testing.
    public Ability[] abilities;

    // -----------------------------------------------------------------------

    public virtual void Attack(eAbilitySlot abilitySlot, Vector3 atWorldPosition)
    {

    }

    // -----------------------------------------------------------------------

    public virtual void StopAttack(eAbilitySlot abilitySlot = eAbilitySlot.SLOT1)
    {

    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Does not IMMEDIATELY transition. 
    /// Sets the next state for transitioning in the next frame
    /// </summary>
    /// <param name="newState">Next State</param>
    public void StateTransition(eStateName newState)
    {
        if (m_stateMachine != null)
        {
            m_stateMachine.SetNextState(newState);
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    ///  Health bars or whatever
    /// </summary>
    public virtual void DisplayHealth()
    {
        
    }
    
    // -----------------------------------------------------------------------
    /// <summary>
    /// Agent takes damage and displays health.
    /// </summary>
    /// <param name="dmg">damage amount</param>
    /// <returns>remaining health</returns>
    public virtual float TakeDamage(float dmg = 1f)
    {
        m_health -= dmg;
        if (m_health <= 0)
        {
            // DEAD
        }

        DisplayHealth();
        return m_health;
    }

    // -----------------------------------------------------------------------

    public virtual void StopMoving()
    {
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Agent moves to their current destination at their movementSpeed
    /// Agent also turns and faces the proper direction.
    /// </summary>
    public virtual void Move()
    {
        transform.LookAt(Destination);
        transform.position += (Destination - transform.position).normalized * Time.deltaTime * movementSpeed;
    }
    
    // -----------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
        base.Start();

        m_stateMachine = GetComponent<StateMachine>();

        if (m_stateMachine)
        {
            m_stateMachine.SetOwner(this);
        }

        // --- Setup Health 
        m_health = maxHealth;

        m_anim = GetComponent<Animator>();       

        DisplayHealth();
	}

    // -----------------------------------------------------------------------

	// Update is called once per frame
	protected override void Update () {
        // Calls message entity for message handling
        base.Update();
              
        if (m_stateMachine)
        {
            m_stateMachine.Execute();
        }
	}

    // -----------------------------------------------------------------------
    /// <summary>
    /// Turns on the trigger in Mechanim for a state change.
    /// </summary>
    /// <param name="triggerName">Mechanim name of a trigger</param>
    public void TriggerAnimation(string triggerName)
    {
        if (m_anim)
        {
            m_anim.SetTrigger(triggerName);
        }
    }
    
    // -----------------------------------------------------------------------
}
