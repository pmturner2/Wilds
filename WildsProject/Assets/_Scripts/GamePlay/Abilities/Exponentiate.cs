using UnityEngine;
using System.Collections;

public class Exponentiate : Ability {
    public Entity attackPrefab;

    public float energyOnActivate = 0.0f;
    public float energyPerSecCharge = 0.0f;
    public float energyOnRelease = 0.0f;

    // How long between smashes? 
    // Could decrease with exponent
    public float growthSpeed = 0.5f;

    // -----------------------------------------------------------------------

    protected TextMesh m_text;
    protected float n = 0.0f;
    protected float m_exponentialValue = 0.0f;

    protected float m_nextGrowthTime = 0.0f;

    protected Vector3 m_lastChargePos;

    // -----------------------------------------------------------------------

    protected override void HandleMessage(Message message)
    {
        switch (message.mType)
        {
            case eMessageType.DELIVER_FROM_POOL:
                PoolRequestData request = (PoolRequestData)message.data;
                DamageEnemyTrigger returnedItem = ((DamageEnemyTrigger) request.item);
                Vector3 randomPos = m_lastChargePos + Random.insideUnitSphere;
                randomPos.y = m_lastChargePos.y;
                returnedItem.transform.position = randomPos;
                returnedItem.Activate();
                break;
        }
    }

    // -----------------------------------------------------------------------

    public void RequestAttackPrefab()
    {

        Message msg = new Message();
        PoolRequestData requestdata = new PoolRequestData();
        requestdata.requester = this;
        requestdata.item = attackPrefab;
        msg.mType = eMessageType.TAKE_FROM_POOL;
        msg.data = requestdata;
        game.DispatchMessage(msg);
    }

    // -----------------------------------------------------------------------

    protected void DoExponentiate()
    {
        n += 1.0f;
        m_exponentialValue *= 2.0f;
        m_text.text = m_exponentialValue.ToString();
        m_lastChargePos = m_owner.transform.position + m_owner.transform.forward;


        m_nextGrowthTime = Time.time + growthSpeed;


        for (int i = 0; i < n; i++)
        {
            RequestAttackPrefab();
        }
    }

    // -----------------------------------------------------------------------

    public override bool Activate()
    {
        if (!base.Activate())
        {
            return false;
        }
                
        if (!m_owner.energy.TryDecreaseEnergy(energyOnActivate))
        {
            return false;
        }

        m_text.renderer.enabled = true;
        m_exponentialValue = 1.0f;
        n = 0.0f;

        DoExponentiate();
       
        return true;
    }

    // -----------------------------------------------------------------------

    public override void Charge()
    {
        base.Charge();
        
        if (m_owner.energy.TryDecreaseEnergy(m_exponentialValue * energyPerSecCharge * Time.deltaTime))
        {

            if (Time.time > m_nextGrowthTime)
            {
                DoExponentiate();
            }

        }
    }

    // -----------------------------------------------------------------------

    public override bool Release()
    {
        if (!base.Release())
        {
            return false;
        }
        m_text.renderer.enabled = false;
        return true;
    }

    // -----------------------------------------------------------------------

    protected override void Update()
    {
        base.Update();

        /* For Text
        transform.LookAt(Camera.main.transform.position);
         * */
    }

    // -----------------------------------------------------------------------

    protected override void Start()
    {
        base.Start();

        m_text = GetComponent<TextMesh>();
        game.RegisterListener(eMessageType.DELIVER_FROM_POOL, this);
    }

    // -----------------------------------------------------------------------

}
