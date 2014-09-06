using UnityEngine;
using System.Collections;

public class BitButton : Clickable {
    
    // Position in Byte [0, 7]. Assigned in Editor. TODO: Possibly compute based on screen position ? 
    public int bitPosition = 0;

    // -----------------------------------------------------------------------

    protected bool _bitValue = false;
    public bool BitValue
    {
        get { return _bitValue; }

        set { _bitValue = value; }
    }

    protected Renderer m_renderer1;
    protected Renderer m_renderer0;
    protected TextMesh m_text;

    // -----------------------------------------------------------------------

    public void SetBit(int bit)
    {
        if (bit > 0)
        {
            SetBit(true);
        }
        else
        {
            SetBit(false);
        }
    }

    // -----------------------------------------------------------------------

    public void SetBit(bool tf)
    {
        BitValue = tf;

        // Disabling renderers for 2 separate objects for now.
        // Later, could make a texture and slide UVs
        m_renderer0.enabled = !BitValue;
        m_renderer1.enabled = BitValue;

        int bitValue = (int)Mathf.Pow(2.0f, (float)bitPosition);

        if (BitValue)
        {
            m_text.text = bitValue.ToString() + "/" + bitValue.ToString();
        }
        else
        {
            m_text.text = "0/" + bitValue.ToString();
        }
    }

    // -----------------------------------------------------------------------

    public override void OnClick()
    {
        RespondToClick();
        SetBit(!BitValue);
    }

    // -----------------------------------------------------------------------

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_renderer0 = transform.FindChild("Bit0").renderer;
        m_renderer1 = transform.FindChild("Bit1").renderer;
        
        // Init
        SetBit(0);


        int bitValue = (int) Mathf.Pow(2.0f, (float)bitPosition);
        if (BitValue)
        {
            m_text.text = bitValue.ToString() + "/" + bitValue.ToString();
        }
        else
        {
            m_text.text = "0/"+bitValue.ToString();
        }
       // m_text.renderer.enabled = false;
    }

    void Awake()
    {

        m_text = GetComponentInChildren<TextMesh>();
    }
}
