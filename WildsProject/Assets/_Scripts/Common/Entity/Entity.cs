using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base entity for everything in our game.
/// </summary>
public abstract class Entity : MonoBehaviour {
    
    protected GameManager game;
    
    // -----------------------------------------------------------------------

	// Use this for initialization
	protected virtual void Start () {
        game = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();       
	}	
}
