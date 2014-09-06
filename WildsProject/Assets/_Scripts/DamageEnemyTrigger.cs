using UnityEngine;
using System.Collections;


// -----------------------------------------------------------------------
/// <summary>
/// Generic Damage Trigger
/// </summary>
public class DamageEnemyTrigger : Entity, IPoolable {

    protected bool m_active = true;

    // -----------------------------------------------------------------------

    public void Activate()
    {
        m_active = true;
    }

    // -----------------------------------------------------------------------
    public void Deactivate()
    {
        m_active = false;

        /*
        // Return to pool
        Message msg = new Message();
        PoolRequestData pr = new PoolRequestData();
        pr.item = this;
        msg.data = pr;
        game.DispatchMessage(msg);
         * */
    }

    // -----------------------------------------------------------------------

    void OnTriggerEnter(Collider other)
    {
        if (m_active)
        {
            Debug.Log("Hit " + other.name);
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Reset when return to the pool
    /// </summary>
    void IPoolable.PoolReset()
    {
        transform.position = Vector3.up * (1000.0f + Random.Range(0.0f, 100.0f));
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Reset when return to the pool
    /// </summary>
    void IPoolable.PoolStart()
    {
    }

    // -----------------------------------------------------------------------
}
