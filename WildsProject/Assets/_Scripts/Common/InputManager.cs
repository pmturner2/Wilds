using UnityEngine;
using System.Collections;

public class InputManager : Entity {
    // If the player holds a button, we want to gate their clicks to a certain rate.
    // Resets on release, so fast clickers aren't limited
    public float clicksPerSec = 2.0f;

    // -----------------------------------------------------------------------

    protected float m_timeBetweenClicks = 0.0f;
    protected float m_nextClickTime = 0.0f;

    // -----------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
        base.Start();
        m_timeBetweenClicks = 1.0f / clicksPerSec;
	}

    // -----------------------------------------------------------------------

	// Update is called once per frame
	void Update () {
        Message msg = new Message();
        msg.mType = eMessageType.INPUT;

        InputData input = new InputData();

        // --- Check Left Click
        
        input.leftClick = eButtonState.INACTIVE;
        
        if (Input.GetMouseButtonUp(0))
        {
            input.leftClick = eButtonState.UP;
            input.screenSpaceClick = Input.mousePosition;
           // Reset clicktime, so people can spam
            m_nextClickTime = 0;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (Time.time > m_nextClickTime)
            {
                input.leftClick = eButtonState.DOWN;
                input.screenSpaceClick = Input.mousePosition;
                m_nextClickTime = Time.time + m_timeBetweenClicks;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (Time.time > m_nextClickTime)
            {
                input.leftClick = eButtonState.HOLD;
                input.screenSpaceClick = Input.mousePosition;
                m_nextClickTime = Time.time + m_timeBetweenClicks;
                //m_nextClickTime = 0; // Disabling timer for now. 
            }
        }
        
        // --- Check For Keys (PC Mode)

        input.shiftClick = Input.GetKey(KeyCode.LeftShift);
       
        msg.data = input;
        game.DispatchMessage(msg);
	}
}

