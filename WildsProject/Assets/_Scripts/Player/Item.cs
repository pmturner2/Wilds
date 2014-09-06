using UnityEngine;
using System.Collections;

/// <summary>
/// UNUSED CURRENTLY. Code from different project.
/// 
/// </summary>
public class Item : Entity {
        
    public eItemType itemType = eItemType.WEAPON;
    public bool hideParent = false;
    public bool positionInner = false;
    public string animationTrigger = "";

	// Use this for initialization
	protected override void Start () {
        base.Start();
	}
}
