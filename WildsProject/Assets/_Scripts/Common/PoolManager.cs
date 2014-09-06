using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MessageEntity {
    
    protected Dictionary<string, Stack<Entity>> pool;

    // -----------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
        base.Start();
        pool = new Dictionary<string, Stack<Entity>>();

        //Register listeners
        game.RegisterListener(eMessageType.RETURN_TO_POOL, this);
        game.RegisterListener(eMessageType.TAKE_FROM_POOL, this);
	}

    // -----------------------------------------------------------------------

    protected override void HandleMessage(Message message)
    {

        PoolRequestData request = (PoolRequestData)message.data;
        switch (message.mType)
        {
            case eMessageType.RETURN_TO_POOL:
                ReturnToPool((Entity)request.item);
                break;
            case eMessageType.TAKE_FROM_POOL:
                TakeFromPool((Entity)request.item, (MessageEntity)request.requester);
                break;
            default:
                break;
        }
    }

    // -----------------------------------------------------------------------

    protected void TakeFromPool(Entity item, MessageEntity requester)
    {
        PoolRequestData poolResponse = new PoolRequestData();
        poolResponse.requester = requester;

        string itemType = item.name;
        
        // A pool already exists
        if (pool.ContainsKey(itemType))
        {
            // ... and there is an item in the stack
            if (pool[itemType].Count > 0)
            {
                poolResponse.item = pool[itemType].Pop();
            }
            else
            {
                poolResponse.item = (Entity)Instantiate(item, transform.position, Quaternion.identity);
            }
        }
        else
        {
            poolResponse.item = CreatePoolAndItem(item);
        }

        // Turn on the pooled item (Register Listeners, etc)
        ((IPoolable)poolResponse.item).PoolStart();

        Message msg = new Message();
        msg.data = poolResponse;
        msg.mType = eMessageType.DELIVER_FROM_POOL;
        game.DispatchMessage(msg);
    }

    // -----------------------------------------------------------------------

    protected Entity CreatePoolAndItem(Entity item)
    {
        pool.Add(item.name, new Stack<Entity>());
        return (Entity)Instantiate(item, transform.position, Quaternion.identity);
    }

    // -----------------------------------------------------------------------

    protected void ReturnToPool(Entity item)
    {
        string itemType = item.name;
        
        // Reset the item. Free memory, Unregister listeners, etc
        ((IPoolable)item).PoolReset();

        // A pool already exists
        if (pool.ContainsKey(itemType))
        {
            pool[itemType].Push(item);        
        }
        else
        {
            CreatePool(item);
            pool[itemType].Push(item); 
        }
    }

    // -----------------------------------------------------------------------

    protected void CreatePool(Entity item)
    {
        pool.Add(item.name, new Stack<Entity>());
    }

    /*
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}*/
}
