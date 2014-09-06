using UnityEngine;
using System.Collections;

public abstract class Ability : MessageEntity {
    public float cooldownTime = 1f;

    protected float m_cooldownEndTime = 0;

    public enum AbilityState { INACTIVE, COOLDOWN, ACTIVATING, CHARGING, RELEASING };
    public AbilityState m_abilityState = AbilityState.INACTIVE;
    
    protected Agent m_owner;

    // -----------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
        base.Start();
	}

    // -----------------------------------------------------------------------
    
	// Update is called once per frame
	protected override void Update () 
    {
        base.Update();

        switch (m_abilityState)
        {
            case AbilityState.COOLDOWN:
                if (Time.time > m_cooldownEndTime)
                {
                    EndCooldown();
                }
                break;
        }
	}

    // -----------------------------------------------------------------------

    public void StartCooldown()
    {
        m_cooldownEndTime = Time.time + cooldownTime;
        m_abilityState = AbilityState.COOLDOWN;
        // Todo: UI Stuff for Cooldown
    }

    // -----------------------------------------------------------------------

    public void EndCooldown()
    {
        m_abilityState = AbilityState.INACTIVE;
        // Todo: UI Stuff for cooldown ending
    }

    // -----------------------------------------------------------------------

    public virtual void DoAbility()
    {
        switch (m_abilityState)
        {
            case AbilityState.INACTIVE:
                Activate();
                break;
            case AbilityState.ACTIVATING:
                Charge();
                break;
            case AbilityState.CHARGING:
                Charge();
                break;
        }
    }

    // -----------------------------------------------------------------------

    public virtual bool Activate()
    {
        if (m_abilityState != AbilityState.INACTIVE)
        {
            return false;
        }
        m_abilityState = AbilityState.ACTIVATING;

        return true;
    }

    // -----------------------------------------------------------------------

    public virtual void Charge()
    {
        m_abilityState = AbilityState.CHARGING;
    }

    // -----------------------------------------------------------------------

    public virtual bool Release()
    {
        if (m_abilityState == AbilityState.ACTIVATING || m_abilityState == AbilityState.CHARGING)
        {
            // Possibly unused, if we skip right to cooldown on release.
            // TODO: Remove if unused
            m_abilityState = AbilityState.RELEASING;

            StartCooldown();

            return true;
        }
        return false;
    }

    // -----------------------------------------------------------------------

    public void SetOwner(Agent owner)
    {
        m_owner = owner;
    }

    // -----------------------------------------------------------------------

}
