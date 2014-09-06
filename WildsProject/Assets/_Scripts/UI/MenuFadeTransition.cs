using UnityEngine;
using System.Collections;

/// <summary>
/// Fading on the renderer material.
/// UI elements should all use the same Material, so we reference it as a sharedMaterial and change the value once.
/// </summary>
public class MenuFadeTransition : MenuTransition
{
    public float fadeTime = 5.0f;
    
    protected Material[] m_materials    = null;
    protected TextMesh[] m_textMeshes   = null;
    protected float     fadeTimer       = 0.0f;
    protected Color     tempColor       = Color.white;
    // Alpha Value
    protected float     a           = 0.0f;
    protected bool      m_fadingIn  = false;

    // -----------------------------------------------------------------------

    public void FadeIn()
    {       
        BeginFade();
        ProcessFade();
    }

    // -----------------------------------------------------------------------

    public void FadeOut()
    {
        BeginFade();
        a = 1.0f - a;
        ProcessFade();
    }

    // -----------------------------------------------------------------------

    public override void Stop()
    {
        base.Stop();

        m_active = false;
    }

    // -----------------------------------------------------------------------

    public override void Begin(bool reset = true)
    {
        //base.Begin(reset);
        // For fading in, we want to animate first... and then fade in.
        // For fading out, we process this in ProcessFade if we are finished.
        if (m_fadingIn)
        {
            base.Begin(reset);
        }
        else
        {
            m_active = true;
        }

        if (!reset)
        {
            if (m_materials != null && m_materials.Length > 0)
            {
                fadeTimer = fadeTime - m_materials[0].GetFloat("_Alpha") * fadeTime;
                a = fadeTimer / fadeTime;
            }
        }
    }

    // -----------------------------------------------------------------------

    public override void Finish()
    {
        // Hide 
        a = 0.0f;
        ProcessFade();

    }

    // -----------------------------------------------------------------------

    protected void BeginFade()
    {

        fadeTimer += Time.deltaTime;
        a = fadeTimer / fadeTime;
    }

    // -----------------------------------------------------------------------

    protected virtual void ProcessFade()
    {
        // First, set alpha on UI elements using our custom shader
        for (int i = 0; i < m_materials.Length; i++)
        {           
            m_materials[i].SetFloat("_Alpha", a);
        }

        // Second, set alpha on TextMeshes using TextMaterial
        for (int i = 0; i < m_textMeshes.Length; i++)
        {
            tempColor = m_textMeshes[i].color;
            tempColor.a = a;
            m_textMeshes[i].color = tempColor;
        }

        if (fadeTimer > fadeTime)
        {
            m_active = false;
            fadeTimer = 0.0f;

            if (!m_fadingIn)
            {
                // Perform normal Transition w/Animation AFTER fading.
                if (anim && animClip)
                {
                    anim.clip = animClip;
                    anim.Play();
                }
            }
        }
    }

    // -----------------------------------------------------------------------

    protected override void Start()
    {
        base.Start();
        if (transitionType == eTransitionType.OUT)
        {
            m_fadingIn = false;
        }
        else
        {
            m_fadingIn = true;
        }

        Renderer[] r = GetComponentsInChildren<Renderer>();
        int length = r.Length;
        if (renderer != null)
        {
            length++;
        }
        m_materials = new Material[length];
        for (int i = 0; i < r.Length; i++)
        {
            m_materials[i] = r[i].sharedMaterial;
            m_materials[i].SetFloat("_Alpha", 0);
        }
        if (renderer != null)
        { 
            m_materials[length - 1] = this.renderer.sharedMaterial;
        }

        m_textMeshes = GetComponentsInChildren<TextMesh>();
        if (!m_fadingIn)
        {
            Finish();
        }
    }

    // -----------------------------------------------------------------------

	// Update is called once per frame
	protected virtual void Update () {
        if (m_active)
        {            
            if (m_fadingIn)
            {
                FadeIn();
            }
            else
            {
                FadeOut();
            }
        }
	}
}
