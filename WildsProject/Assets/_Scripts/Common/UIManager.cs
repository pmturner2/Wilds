using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : Agent
{
    // Destination UI Element for our player
    public Transform m_destinationMarker;
    public WorldEnergy m_worldEnergy;
    public Clickable currentClick = null;
    // -----------------------------------------------------------------------

    protected Dictionary<eStateName, MenuScreen> allScreens = null;
    
    // Queue for messages. Should be handled per frame.
    protected LayerMask lm = 0;
    protected bool m_hudEnabled = true;
    protected SpriteRenderer m_destinationRenderer;
    protected Camera m_guiCamera;

    // -----------------------------------------------------------------------

    public bool CheckClick(Vector3 screenSpacePoint, bool assignClick = false, bool compareClicks = false)
    {
        const float raycastDistance = 1000f;
        RaycastHit hit;
        Ray ray = m_guiCamera.ScreenPointToRay(screenSpacePoint);

        // Try to get a clickable item
        if (Physics.Raycast(ray, out hit, raycastDistance, game.clickableLM))
        {
            Clickable button = hit.collider.GetComponent<Clickable>();

            // If we click UI, handle it
            if (button != null)
            {

                // We are comparing our current click with the processing click
                if (compareClicks && currentClick != hit.collider)
                {
                    if (assignClick)
                    {
                        currentClick = button;
                    }
                    return false;
                }
                if (assignClick)
                {
                    currentClick = button;
                }
                return true;
            }
        }
        
        return false;
    }
    
    // -----------------------------------------------------------------------

    public void EnableHUD(bool tf)
    {
        m_hudEnabled = tf;
    }

    // -----------------------------------------------------------------------

    public bool IsHUDEnabled()
    {
        return m_hudEnabled;
    }

    // -----------------------------------------------------------------------

    public void SetTransitionTime(float f)
    {
        if (m_stateMachine)
        {
            m_stateMachine.SetTransitionTime(f);
        }
    }

    // -----------------------------------------------------------------------

    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type != LogType.Log && type != LogType.Warning)
        {
            ShowError(logString);
        }
    }

    // -----------------------------------------------------------------------

    public void ShowError(string s)
    {
        char[] chars = s.ToCharArray();
        int countSinceNL = 0;
        int NL_marker = 20;
        int NL_limit = 50;
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == ' ')
            {
                if (countSinceNL > NL_marker)
                {
                    countSinceNL = 0;
                    chars[i] = '\n';
                }
            }
            else if (countSinceNL > NL_limit)
            {
                countSinceNL = 0;
                chars[i] = '\n';
            }

            countSinceNL++;
        }
        s = new string(chars);
        MenuScreen errorScreen = GetScreen(eStateName.MENU_ERROR);
        errorScreen.SetText(s);
        StateTransition(GetScreen(eStateName.MENU_ERROR));
    }
    
    // -----------------------------------------------------------------------

    public MenuScreen GetScreen(eStateName sn)
    {
        return allScreens[sn];
    }

    // -----------------------------------------------------------------------


    public void NavigateBack()
    {
        m_stateMachine.GotoLastState();
    }

    // -----------------------------------------------------------------------

    public void NavigateTo(eStateName sn)
    {
        NavigateTo(GetScreen(sn));
    }

    // -----------------------------------------------------------------------

    public void NavigateTo(MenuScreen screen)
    {
        if (screen == null)
        {
            Debug.LogWarning("UI Manager: Attempting to navigate to a null menu screen");
            return;
        }
        StateTransition(screen);
    }

    // -----------------------------------------------------------------------

    public void ShowHUD(bool fade = true)
    {
        // TODO
    }

    // -----------------------------------------------------------------------

    public void HideHUD(bool fade = true)
    {
        // TODO
    }

    // -----------------------------------------------------------------------

    public void ShowPause(bool fade = true)
    {
        NavigateTo(GetScreen(eStateName.MENU_PAUSE));
    }

    // -----------------------------------------------------------------------

    public void HidePause(bool fade = true)
    {
        HideCurrentScreen(fade);
    }

    // -----------------------------------------------------------------------

    public void HideCurrentScreen(bool fade = true)
    {
        StateTransition(GetScreen(eStateName.MENU_NONE));
    }

    // -----------------------------------------------------------------------
    /*
    public bool ProcessClick(Vector3 mousePos)
    {
        bool hitButton = false;
        RaycastHit hit;
        Ray ray = this.transform.parent.camera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit, 1000, lm))
        {
            Clickable button = hit.collider.GetComponent<Clickable>();
            if (button != null)
            {
                hitButton = true;
                button.OnClick(hit.point);
            }
        }
        return hitButton;
    }
    */
    // -----------------------------------------------------------------------

    public void ProcessHold(Vector3 mousePos)
    {
        RaycastHit hit;
        Ray ray = this.transform.parent.camera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit, 1000, lm))
        {
            Clickable button = hit.collider.GetComponent<Clickable>();
            if (button)
            {
                button.OnHold(hit.point);
            }
          
        }
    }

    // -----------------------------------------------------------------------

    public void HideDestinationMarker()
    {
        m_destinationMarker.renderer.enabled = false;
    }

    // -----------------------------------------------------------------------

    public void MarkPlayerDestination(Vector3 worldDestination)
    {
       
        m_destinationRenderer.enabled = true;
        m_destinationMarker.transform.position = worldDestination + Vector3.up * 0.15f;
    }
      
    // -----------------------------------------------------------------------

    public string GetLocalizedString(eStringName sn)
    {
        //TODO: Lookup in spreadsheet, based on language selected.
        switch (sn)
        {
            case eStringName.MAGE_INFO:
                return "Mage\n\nSkills: Manipulation\nSpeed: Fast\nAbilities: ...";
            case eStringName.HEAVY_INFO:
                return "Heavy\n\nSkills: Raw Power\nSpeed: Slow\nAbilities: ...";
            case eStringName.ARE_YOU_SURE_QUIT:
                return "Exit Geometric Primitive?";
            case eStringName.CREATE:
                return "Create";
            case eStringName.CONTINUE:
                return "Continue";
            case eStringName.FRIENDS_ONLY:
                return "Friends Only";
            case eStringName.GAME_NAME:
                return "Game Name";
            case eStringName.LEVEL:
                return "Level";
            case eStringName.MAX_PLAYERS:
                return "Max Players";
            case eStringName.VOICE_CHAT:
                return "Voice Chat";
            case eStringName.RESUME:
                return "Resume";
            case eStringName.QUIT:
                return "Quit";
            case eStringName.OPTIONS:
                return "Options";
            case eStringName.DONE:
                return "Done";
            case eStringName.NONE:
                return "";
        }
        return "<Error>";
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Does not IMMEDIATELY transition. 
    /// Sets the next state for transitioning in the next frame
    /// </summary>
    /// <param name="newState">Next State</param>
    public void StateTransition(StateGeneric newState)
    {
        if (m_stateMachine != null)
        {
            m_stateMachine.SetNextState(newState);
        }
    }
    
    // -----------------------------------------------------------------------
    /// <summary>
    ///  Template for what this function should look like. This should be override in child class.
    /// </summary>
    /// <param name="message"></param>
    protected override void HandleMessage(Message message)
    {
        // HandleMessage returns true if it handles the message successfully
        switch (message.mType)
        {
            case eMessageType.MENU_TRANSITION:
                eStateName nextState = (eStateName)(int)message.data;
                if (nextState == eStateName.MENU_BACK)
                {
                    m_stateMachine.GotoLastState();
                }
                else
                {
                    StateTransition(GetScreen((eStateName)(int)message.data));
                }
                break;
        }
        if (m_stateMachine)
        {
            if (!m_stateMachine.HandleMessage(message))
            {
                // other handlers go here
            }
        }
    }
  
    // -----------------------------------------------------------------------

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        allScreens = new Dictionary<eStateName, MenuScreen>();
        MenuScreen[] components = GetComponentsInChildren<MenuScreen>();
        foreach (MenuScreen c in components)
        {
            allScreens.Add(c.stateName, c);
        }
       
        // Destination of player
        m_destinationRenderer = m_destinationMarker.GetComponent<SpriteRenderer>();

        lm = 1 << LayerMask.NameToLayer("UILayer");

        // Find our camera objects in the scene and grab references
        GameObject cameraObj = GameObject.FindGameObjectWithTag("GUICamera");
        if (cameraObj)
        {
            m_guiCamera = cameraObj.GetComponent<Camera>();
        }

    }

    // -----------------------------------------------------------------------

    protected override void Update()
    {
        base.Update();

        if (m_stateMachine)
        {
            m_stateMachine.Execute();
        }
    }
}

