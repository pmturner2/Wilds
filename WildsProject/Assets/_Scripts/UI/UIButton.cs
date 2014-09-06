using UnityEngine;
using System.Collections;

public class UIButton : Clickable {

    // Do we transition on click?
    public bool transitionOnClick = true;

    // If transition on click is true, which state do we transition to?
    public eStateName clickTransition = eStateName.MENU_NONE;

    // What other messages do we send out on click?
    public eMessageType[] clickMessages = null;

    protected TextMesh  m_text = null;

    // For clicked texture change. looks bad.
    protected Mesh m_mesh = null;
   
    protected Color m_color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    protected float m_y = 0;

    public object msg_data = null;

    // -----------------------------------------------------------------------
    /// <summary>
    /// Bring this button out of the screen
    /// </summary>
    public void MoveOut()
    {
        Vector3 p = this.transform.localPosition;
        p.y = m_y + 1000;
        this.transform.localPosition = p;
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Bring this button into the screen
    /// </summary>
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

    public override void OnClick()
    {
        RespondToClick();

        if (clickMessages != null)
        {
            ProcessMessages();
        }

        // Common behavior: Move to a new UI Screen on click
        if (transitionOnClick)
        {
            Message message = new Message();
            message.mType = eMessageType.MENU_TRANSITION;
            message.data = (int)clickTransition;
            game.ui.QueueMessage(message);
        }
    }

    // -----------------------------------------------------------------------
	/// <summary>
	/// Sending messages out. Override this to customize Button message sending for more complex behavior
	/// </summary>
    protected virtual void ProcessMessages()
    {
        foreach (eMessageType message in clickMessages)
        {
            Message msg = new Message();
            msg.mType = message;
            msg.data = msg_data;
            game.DispatchMessage(msg);
        }
    }

    // -----------------------------------------------------------------------

    void Awake()
    {
        m_y = this.transform.localPosition.y;
    }

    // -----------------------------------------------------------------------

    // Use this for initialization
	protected override void Start () {
        base.Start();
        m_text = GetComponentInChildren<TextMesh>();        
    }

    // -----------------------------------------------------------------------

    
	

}
