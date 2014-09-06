using UnityEngine;
using System.Collections;

/// <summary>
/// UNUSED CURRENTLY. Code from different project.
/// 
/// </summary>
public class Inventory : MessageEntity {
    public Item[] items;
    public Item RH, LH;

    public ItemSlot rHand, lHand, rFoot, lFoot, back, head;

	// Use this for initialization
	protected override void Start () {
        base.Start();

        ItemSlot[] slots = GetComponentsInChildren<ItemSlot>();
        foreach (ItemSlot slot in slots)
        {
            if (slot.itemSlot == eItemSlot.R_HAND)
            {
                rHand = slot;
            }
            if (slot.itemSlot == eItemSlot.L_HAND)
            {
                lHand = slot;
            }
        }
        EquipRightHand(RH);
        EquipLeftHand(LH);
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public void EquipRightHand(Item item)
    {
        rHand.item = item;
        item.transform.parent = rHand.transform;

        Vector3 localOffset = new Vector3(0, 0, 0);
        item.transform.localPosition = localOffset;// item.positionOffset;

        Vector3 pos = rHand.transform.position;
        if (item.positionInner)
        {
            pos.z *= 0.95f;
        }
        else
        {
            pos.z *= 1.05f;
        }
        item.transform.position = pos;
        if (item.hideParent)
        {
            rHand.renderer.enabled = false;
        }
    }

    public void EquipLeftHand(Item item)
    {
        // TODO : Unequip
        // Generalize this

        lHand.item = item;
        item.transform.parent = lHand.transform;

        Vector3 pos = lHand.transform.position;
        if (item.positionInner)
        {
            pos.z *= 0.95f;
        }
        else
        {
            pos.z *= 1.05f;
        }
        item.transform.position = pos;

        if (item.hideParent)
        {
            lHand.renderer.enabled = false;
        }

    }
}
