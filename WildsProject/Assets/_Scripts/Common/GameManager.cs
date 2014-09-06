using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    
    // Player?
    public Player player = null;
    
    [HideInInspector]
    public UIManager ui = null;
    
    // Layermask that specifies Terrain
    public LayerMask terrainLM;
    public LayerMask clickableLM;
    
    [HideInInspector]
    public Camera gameCamera;
    
    [HideInInspector]    
    public Camera menuCamera; // Orthographic

    // -----------------------------------------------------------------------

    protected Dictionary<eMessageType, List<MessageEntity>> listeners = null;

    // -----------------------------------------------------------------------

    public void DispatchToListeners(Message msg)
    {
        if (listeners.ContainsKey(msg.mType))
        {
            List<MessageEntity> msgListeners = listeners[msg.mType];
            foreach (MessageEntity listener in msgListeners)
            {
                listener.QueueMessage(msg);
            }
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Listeners will receive all instances of the message type, regardless of the "Destination" field in the message.
    /// </summary>
    /// <param name="msg">Message type to forward</param>
    /// <param name="listener">Forwarding Destination</param>
    public void RegisterListener(eMessageType msg, MessageEntity listener)
    {
        if (listeners.ContainsKey(msg))
        {
            List<MessageEntity> msgListeners = listeners[msg];
            msgListeners.Add(listener);
        }
        else
        {
            List<MessageEntity> msgListeners = new List<MessageEntity>();
            msgListeners.Add(listener);
            listeners.Add(msg, msgListeners);
        }
    }

    // -----------------------------------------------------------------------

    public void UnregisterListener(eMessageType msg, MessageEntity listener)
    {
        if (listeners.ContainsKey(msg))
        {
            List<MessageEntity> msgListeners = listeners[msg];
            msgListeners.Remove(listener);
        }
        else
        {
            Debug.LogWarning("Trying to remove non-existant Listener: " + listener.name + " msg: " + msg);
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// All messages should be funnelled through this function. They will be automatically distributed to the proper location.
    /// </summary>
    /// <param name="msg"></param>    
    public void DispatchMessage(Message message)
    {
        // TODO: If Destination is Set, check flag for ONLY_DESTINATION, and process accordingly.
        // If ONLY_DESTINATION is true, bail out after sending to destination, otherwise continue processing.
        
        DispatchToListeners(message);
        
        //Send input message to player
        switch (message.mType)
        {
            case eMessageType.DELIVER_FROM_POOL:
                PoolRequestData data = (PoolRequestData)message.data;
                if (data.requester != null)
                {
                    data.requester.QueueMessage(message);
                }
                break;
            case eMessageType.INPUT: 

                // Game Manager parses the clicks.
                InputData input = (InputData)message.data;

                // This is used for game camera clicks.
                Collider clickCollider = null;

                // This is the stored click object from a previous click. (Cleared on release)
                Clickable currentClick = ui.currentClick;
                
                RaycastHit hit;
                
                bool shouldProcessWorldClick = false;

                switch (input.leftClick)
                {
                    case eButtonState.DOWN:
                        shouldProcessWorldClick = !ui.CheckClick(input.screenSpaceClick, true, false);
                        // TODO: Process DOWN event. This system needs updating / separation of behavior. 
                        // Imagine for gamepad / touch and separate accordingly.
                        break;
                    case eButtonState.HOLD:
                        shouldProcessWorldClick = !ui.CheckClick(input.screenSpaceClick, false, false);
                        break;
                    case eButtonState.UP:
                        shouldProcessWorldClick = true;
                        // If we have clicked a UI Element earlier
                        if (currentClick != null)
                        {
                            // If we have a matching UI that we are releasing on, then do it. Otherwise, do world.
                            if (ui.CheckClick(input.screenSpaceClick, true, true))
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
                        ui.currentClick = null;

                        ui.HideDestinationMarker();
                        break;
                    case eButtonState.INACTIVE:
                        shouldProcessWorldClick = false;
                        break;
                }


                // If not UI, do World Stuff.
                if (shouldProcessWorldClick)
                {
                    clickCollider = GetColliderFromCameraRaycast(gameCamera, input.screenSpaceClick, out hit);
                    if (clickCollider && clickCollider.CompareTag("Terrain"))
                    {
                        input.worldDestination = hit.point;
                        ui.MarkPlayerDestination(hit.point);
                        message.data = input;
                        player.QueueMessage(message);
                    }
                }
                
                return;
            default:
                // TODO: possibly remove this if it is unnecessary.
                // If we do nothing else with it, send the message to the player.
                if (player != null)
                {
                    player.QueueMessage(message);
                }
                // Also send it to the UI system.
                ui.QueueMessage(message);
                return;
        }

    }

    // -----------------------------------------------------------------------

    private Collider GetColliderFromCameraRaycast(Camera fromCamera, Vector3 toPoint, out RaycastHit hit)
    {
        const float raycastDistance = 1000f;

        Ray ray = fromCamera.ScreenPointToRay(toPoint);
        // Try to get a clickable item
        if (Physics.Raycast(ray, out hit, raycastDistance, clickableLM))
        {
            return hit.collider;
        }
        return null;
    }

    // -----------------------------------------------------------------------


	// Use this for initialization
	void Awake () {
        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
      
        listeners = new Dictionary<eMessageType, List<MessageEntity>>();
	}

    // -----------------------------------------------------------------------

    void Start()
    {
        // Find our camera objects in the scene and grab references
        GameObject cameraObj = GameObject.FindGameObjectWithTag("GUICamera");
        if (cameraObj)
        {
            menuCamera = cameraObj.GetComponent<Camera>();
        }

        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (cameraObj)
        {
            gameCamera = cameraObj.GetComponent<Camera>();
        }
    }

    // -----------------------------------------------------------------------

	// Update is called once per frame
	void Update () {
        
        //TODO: Remove Player code can be removed later. For Testing.
        if (player == null)
        {
            //GameObject go = GameObject.FindGameObjectWithTag("Player");
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            //player = go.GetComponent<Player>();
        }
        if (player == null)
        {
            print("player is still null");
        }
        if (Time.frameCount > 20)
        {
            totalFrames += 1.0f;

            totalFPS += 1.0f / Time.deltaTime;
            avgFPS = totalFPS / totalFrames;
            displayFPS = Mathf.FloorToInt((float)avgFPS);
        }
	}

    // TEMP
    double avgFPS = 0;
    double totalFPS = 0;
    double totalFrames = 0;
    int displayFPS = 0;

   
    // -----------------------------------------------------------------------


    void OnGUI()
    {
        // TEMP. REMOVE
        GUI.Label(new Rect(Screen.width - 100, 0, 100, 100), "AVG FPS: " + displayFPS);
    }
}
