using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Agent {
  
    // Unused currently
    protected Inventory m_inventory;
    protected Rigidbody m_rb;

    protected Vector3 m_normalizedMoveVector;

    // -----------------------------------------------------------------------

    public override void Attack(eAbilitySlot abilitySlot, Vector3 atWorldPosition)
    {

        atWorldPosition.y = this.transform.position.y;

        transform.LookAt(atWorldPosition);
        game.ui.HideDestinationMarker();
        abilities[0].DoAbility();
    }

    // -----------------------------------------------------------------------

    public override void StopAttack(eAbilitySlot abilitySlot = eAbilitySlot.SLOT1)
    {
        game.ui.HideDestinationMarker();
        abilities[0].Release();
    }
           
    // -----------------------------------------------------------------------
    
    public override void Move()
    {
        Vector3 lookAtVector = Destination;
        lookAtVector.y = this.transform.position.y;

        transform.LookAt(lookAtVector);
        
        // Cancel movement vector from last frame.
        CancelVelocityInDirection(m_normalizedMoveVector);

        // Calculate new vector, and apply force
        m_normalizedMoveVector = (Destination - transform.position).normalized;
        float magnitude = movementSpeed;
        float currentMagnitude = Vector3.Dot(m_normalizedMoveVector, m_rb.velocity);

        m_rb.velocity += m_normalizedMoveVector * (magnitude - currentMagnitude);

    }

    // -----------------------------------------------------------------------

    public void CancelVelocityInDirection(Vector3 dir)
    {
        float oldVelocityMagnitude = Vector3.Dot(dir, m_rb.velocity);
        m_rb.velocity -= oldVelocityMagnitude * dir;
    }

    // -----------------------------------------------------------------------

    public override void StopMoving()
    {
        m_normalizedMoveVector = Vector3.zero;
        m_rb.velocity = Vector3.zero;
    }
    
    // -----------------------------------------------------------------------
    /// <summary>
    /// Handle 1 message
    /// </summary>
    /// <param name="message"></param>
    protected override void HandleMessage(Message message)
    {
        switch (message.mType)
        {
            case eMessageType.NONE:              
                break;
            default:
                break;
        }
        // Send the messages to our state machine, so the individual states can handle them.
        if (m_stateMachine)
        {
            m_stateMachine.HandleMessage(message);
        }
    }

    // -----------------------------------------------------------------------

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        m_inventory = GetComponent<Inventory>();
     
        m_rb = GetComponent<Rigidbody>();
        
        abilities[0].SetOwner(this);
    }

    // -----------------------------------------------------------------------

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
              
    }

    // -----------------------------------------------------------------------
    /*

    void FixedUpdate()
    {
        m_rb.velocity = Velocity;        
    }
    */
}
