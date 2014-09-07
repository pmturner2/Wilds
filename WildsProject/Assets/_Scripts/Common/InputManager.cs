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
       // Read Input

       // Instead of sending an input message to game manager, process input and game state here and send appropriate messages.
       // Message msg = new Message();
       
        //msg.mType = eMessageType.INPUT;

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

        ProcessInput(ref input);
    }

    void ProcessInput(ref InputData input) {
        // This is used for game camera clicks.
        Collider clickCollider = null;

        // This is the stored click object from a previous click. (Cleared on release)
        Clickable currentClick = game.ui.currentClick;

        RaycastHit hit;

        bool shouldProcessWorldClick = false;

        switch (input.leftClick)
        {
            case eButtonState.DOWN:
                shouldProcessWorldClick = !game.ui.CheckClick(input.screenSpaceClick, true, false);
                // TODO: Process DOWN event. This system needs updating / separation of behavior. 
                // Imagine for gamepad / touch and separate accordingly.
                break;
            case eButtonState.HOLD:
                shouldProcessWorldClick = !game.ui.CheckClick(input.screenSpaceClick, false, false);
                break;
            case eButtonState.UP:
                shouldProcessWorldClick = true;
                // If we have clicked a UI Element earlier
                if (currentClick != null)
                {
                    // If we have a matching UI that we are releasing on, then do it. Otherwise, do world.
                    if (game.ui.CheckClick(input.screenSpaceClick, true, true))
                    {
                        shouldProcessWorldClick = false;
                        // if UI, process it
                        currentClick.OnClick();
                    }
                    else
                    {
                        // If not UI, do World Stuff.
                        // Or do nothing, since it's up. Change this
                    }
                }

                // Clear UI State
                game.ui.currentClick = null;

                game.ui.HideDestinationMarker();
                break;
            case eButtonState.INACTIVE:
                shouldProcessWorldClick = false;
                break;
        }


        // If not UI, do World Stuff.
        if (shouldProcessWorldClick)
        {
            clickCollider = game.GetColliderFromCameraRaycast(game.gameCamera, input.screenSpaceClick, out hit);
            if (clickCollider && clickCollider.CompareTag("Terrain"))
            {
                input.worldDestination = hit.point;
                game.ui.MarkPlayerDestination(hit.point);
                Message message = new Message();
                message.mType = eMessageType.INPUT;
                message.data = input;
                game.player.QueueMessage(message);
            }
        }


        //msg.data = input;
        //game.DispatchMessage(msg);
	}
}

