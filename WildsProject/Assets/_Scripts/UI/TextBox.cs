using UnityEngine;
using System.Collections;

public class TextBox : Entity
{
    public eStringName stringName = eStringName.ARE_YOU_SURE_QUIT;
    public bool useStringName = true;

    protected TextMesh m_text = null;
    protected float m_y = 0;

    // -----------------------------------------------------------------------

    public void MoveOut()
    {
        Vector3 p = this.transform.localPosition;
        p.y = m_y + 1000;
        this.transform.localPosition = p;
    }

    // -----------------------------------------------------------------------
    
    public void MoveIn()
    {
        Vector3 p = this.transform.localPosition;
        p.y = m_y;
        this.transform.localPosition = p;
    }
     
    // -----------------------------------------------------------------------

    public void SetTextColor(Color c)
    {
        m_text.color = c;
    }

    // -----------------------------------------------------------------------

    public void SetText(string s)
    {
        m_text.text = s;
    }

    // -----------------------------------------------------------------------

    public string GetText()
    {
        return m_text.text;
    }
    // -----------------------------------------------------------------------

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_text = GetComponentInChildren<TextMesh>();
        m_y = this.transform.localPosition.y;
        if (useStringName)
        {
            SetText(game.ui.GetLocalizedString(stringName));
        }
    }

    // -----------------------------------------------------------------------

    

}