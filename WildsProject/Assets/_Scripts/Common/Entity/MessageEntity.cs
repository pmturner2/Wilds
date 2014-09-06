using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Able to receive messages.
/// </summary>
public abstract class MessageEntity : Entity , IPoolable {

    // Our message queue. 
    protected Queue<Message> m_messageQueue;

    // Registers Listeners 
    public eMessageType[] listenForThese;

    // -----------------------------------------------------------------------

    public virtual void QueueMessage(Message message)
    {
        if (m_messageQueue != null)
        {
            m_messageQueue.Enqueue(message);
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Handles all messages. Called by Update once per frame.
    /// </summary>
    protected virtual void HandleMessages()
    {
        if (m_messageQueue == null)
        {
            return;
        }

        while (m_messageQueue.Count > 0)
        {
            Message message = m_messageQueue.Dequeue();

            HandleMessage(message);
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    ///  Template for what this function should look like. This should be override in child class.
    ///  Handles 1 Message.
    /// </summary>
    /// <param name="message"></param>
    protected virtual void HandleMessage(Message message)
    {
        switch (message.mType)
        {
            default:
                break;
        }
    }

    // -----------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
        base.Start();
        m_messageQueue = new Queue<Message>();

        RegisterListeners();
	}

    // -----------------------------------------------------------------------

	// Update is called once per frame
	protected virtual void Update () {
        HandleMessages();
	}

    // -----------------------------------------------------------------------

    void OnDisable()
    {
        UnregisterListeners();
    }

    // -----------------------------------------------------------------------

    void OnDestroy()
    {
        UnregisterListeners();
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Unregister ourself with game manager
    /// </summary>
    protected virtual void UnregisterListeners()
    {
        if (listenForThese != null && listenForThese.Length > 0)
        {
            foreach (eMessageType listenFor in listenForThese)
            {
                game.UnregisterListener(listenFor, this);
            }
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Register ourself with game manager
    /// </summary>
    protected virtual void RegisterListeners()
    {
        if (listenForThese != null && listenForThese.Length > 0)
        {
            foreach (eMessageType listenFor in listenForThese)
            {
                game.RegisterListener(listenFor, this);
            }
        }
    }
        
    // POOLING

    // -----------------------------------------------------------------------
    /// <summary>
    /// Reset when return to the pool
    /// </summary>
    void IPoolable.PoolReset()
    {
        UnregisterListeners();  
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Reset when return to the pool
    /// </summary>
    void IPoolable.PoolStart()
    {
        RegisterListeners();
    }

    // -----------------------------------------------------------------------
}
