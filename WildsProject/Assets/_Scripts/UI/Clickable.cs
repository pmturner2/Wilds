using UnityEngine;
using System.Collections;

public class Clickable : Entity {
    
    protected Animation m_anim = null;

    // -----------------------------------------------------------------------

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_anim = GetComponent<Animation>();
	
	}
	
    // -----------------------------------------------------------------------

    protected virtual void RespondToClick()
    {
        if (m_anim)
        {
            m_anim.Play();
        }       
    }

    // -----------------------------------------------------------------------

    public virtual void OnHold(Vector3 worldPos)
    {
    }

    // -----------------------------------------------------------------------

    public virtual void OnClick()
    {
        RespondToClick();
    }

    // -----------------------------------------------------------------------
}
