using UnityEngine;
using System.Collections;

public class AgentEnergy : Entity {

    public float maxEnergy = 100.0f;
    public float rechargeRate = 10.0f; // energy per sec

    protected float m_currentEnergy = 0.0f;

    // -----------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
        m_currentEnergy = maxEnergy;
	}

    // -----------------------------------------------------------------------

	// Update is called once per frame
	protected void Update () {
        IncreaseEnergy(Time.deltaTime * rechargeRate);
        RenderEnergy();
	}
    
    // -----------------------------------------------------------------------

    public virtual void IncreaseMaxEnergy(float increase)
    {
        maxEnergy += increase;
    }

    // -----------------------------------------------------------------------

    public virtual void DecreaseMaxEnergy(float decrease)
    {
        maxEnergy -= decrease;
    }

    // -----------------------------------------------------------------------

    public void IncreaseRegenRate(float increase)
    {
        rechargeRate += increase;
    }

    // -----------------------------------------------------------------------

    public void DecreaseRegenRate(float decrease)
    {
        rechargeRate -= decrease;
    }

    // -----------------------------------------------------------------------

    public void IncreaseEnergy(float amount)
    {
        m_currentEnergy += amount;
        if (m_currentEnergy > maxEnergy)
        {
            m_currentEnergy = maxEnergy;
        }
    }

    // -----------------------------------------------------------------------

    public bool TryDecreaseEnergy(float amount)
    {
        if (m_currentEnergy < amount)
        {
            return false;
        }
        m_currentEnergy -= amount;
        return true;
    }

    // -----------------------------------------------------------------------

    protected virtual void RenderEnergy()
    {
    }
}
