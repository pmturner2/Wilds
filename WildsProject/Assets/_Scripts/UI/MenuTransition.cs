using UnityEngine;
using System.Collections;

/// <summary>
/// If animation is attached, it plays on transition
/// </summary>
public class MenuTransition : Entity
{
    // What type of transition is this?
    public eTransitionType transitionType = eTransitionType.IN;
    
    // Animation to play on transition
    public AnimationClip animClip = null;
    
    protected Animation anim = null;

    // Currently mid-transition?
    protected bool m_active = false;

    // -----------------------------------------------------------------------
    
    public bool IsActive()
    {
        // TODO: Check implementation for non-fading transitions. Check animation?
        return m_active;
    }
     
    // -----------------------------------------------------------------------
   
    public virtual void Finish()
    {

    }
     // -----------------------------------------------------------------------

    public virtual void Stop()
    {
        if (anim && animClip)
        {
            anim.clip = animClip;
            anim.Stop();
        }

        m_active = false;
    }

    // -----------------------------------------------------------------------

    public virtual void Begin(bool reset = true)
    {        
        if (anim && animClip)
        {
            anim.clip = animClip;
            anim.Play();
        }

        m_active = true;
    }

    // -----------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
        base.Start();
        anim = GetComponent<Animation>();
	}

    // -----------------------------------------------------------------------

}
