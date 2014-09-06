using UnityEngine;
using System.Collections;

public class BitSet : Entity {

    protected int _numBits = 0;
    public int NumBits
    {
        get { return _numBits; }
        set { _numBits = value; }
    }

    
    public int MaxValue
    {
        get { return (int) Mathf.Pow(2.0f, (float) _numBits); }
        set {  }
    }
    // -----------------------------------------------------------------------

    protected BitButton[] m_bits;


    // -----------------------------------------------------------------------
    
    public void IncreaseSize(int increase)
    {
        SetNumBits(NumBits + increase);
    }

    // -----------------------------------------------------------------------    
 
    public void SetNumBits(int numBits)
    {
        NumBits = Mathf.Clamp(numBits, 0, m_bits.Length);

        for (int i = 0; i < NumBits; i++)
        {
            m_bits[i].gameObject.SetActive(true);
        }
        for (int i = NumBits; i < m_bits.Length; i++)
        {
            m_bits[i].gameObject.SetActive(false);
        }
        // activate appropriate number, and then center on X
    }

    // -----------------------------------------------------------------------

    public void SetByteValue(int newValue)
    {
        int currentBit = 0;

        while (newValue > 0)
        {
            int lowBit = newValue % 2;
            newValue = newValue / 2;
            m_bits[currentBit++].SetBit(lowBit);            
        }

        // Zero out remainder
        for (int i = currentBit; i < NumBits; i++)
        {
            m_bits[i].SetBit(0);
        }
    }

    // -----------------------------------------------------------------------

    public byte GetByteValue()
    {
        byte val = 0;
        for (int i = 0; i < NumBits; i++)
        {
            if (m_bits[i].BitValue)
            {
                val += (byte) Mathf.Pow(2.0f, (float)i);
            }
        }
        return val;
    }

    // -----------------------------------------------------------------------

    public void RandomizeBits()
    {
        for (int i = 0; i < NumBits; i++)
        {
            int value = Random.Range(0, 2);
            m_bits[i].SetBit(value);
        }
    }
    // -----------------------------------------------------------------------

    public void SetBit(int index, int value)
    {
        if (index >= 0 && index < NumBits)
        {
            m_bits[index].SetBit(value);
        }
    }

    // -----------------------------------------------------------------------

    public void SetBit(int index, bool tf)
    {
        if (index >= 0 && index < NumBits)
        {
            m_bits[index].SetBit(tf);
        }
    }

    // -----------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
        base.Start();

        BitButton[] unorderedBits = GetComponentsInChildren<BitButton>();
        NumBits = unorderedBits.Length;

        m_bits = new BitButton[NumBits];

        for (int i = 0; i < NumBits; i++)
        {
            // Sort them into m_bits by their position (specified in editor)
            m_bits[unorderedBits[i].bitPosition] = unorderedBits[i];
        }        
	}

    // -----------------------------------------------------------------------

}
