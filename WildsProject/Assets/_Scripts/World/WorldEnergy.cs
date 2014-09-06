using UnityEngine;
using System.Collections;

public class WorldEnergy : AgentEnergy {
    
    public GameObject energySprite;
    public Color availableColor;
    public Color usedColor;
    public Color fullColor;

    protected GameObject[] healthRenders;

    // -----------------------------------------------------------------------
  
    protected override void RenderEnergy()
    {
        if (healthRenders == null)
        {
            return;
        }
        float verticalOffset = 0.75f;
        float widthPerUnit = 0.175f;
        //Vector3 heartPosition = new Vector3(this.transform.position.x - widthPerUnit * maxHealth / 2f, this.transform.position.y + verticalOffset, 0);
        Vector3 heartPosition = new Vector3(-widthPerUnit * maxEnergy / 2f, verticalOffset, 0);

        for (int i = 0; i < maxEnergy; i++)
        {
            if (m_currentEnergy == maxEnergy)
            {
                healthRenders[i].renderer.material.color = fullColor;
            }
            else if (m_currentEnergy > i)
            {
                healthRenders[i].renderer.material.color = availableColor;
            }
            else
            {
                healthRenders[i].renderer.material.color = usedColor;
            }
            healthRenders[i].transform.localPosition = heartPosition;

            heartPosition.x += widthPerUnit;
        }
    }

    // -----------------------------------------------------------------------

    protected override void Start()
    {
        base.Start();

        SetEnergyRenderers();
    }

    // -----------------------------------------------------------------------

    public override void IncreaseMaxEnergy(float increase)
    {
        base.IncreaseMaxEnergy(increase);

        SetEnergyRenderers();
    }

    // -----------------------------------------------------------------------

    public override void DecreaseMaxEnergy(float decrease)
    {
        base.DecreaseMaxEnergy(decrease);

        SetEnergyRenderers();
    }

    // -----------------------------------------------------------------------

    protected void SetEnergyRenderers()
    {
        // Destroy old. Could pool these instead, but I think this will be infrequent enough to not matter.
        if (healthRenders != null)
        {
            int oldRenderCount = healthRenders.Length;
            if (oldRenderCount > 0)
            {
                for (int i = 0; i < oldRenderCount; i++)
                {
                    Destroy(healthRenders[i]);
                }
            }
        }

        // Create New ones
        int renderCount = Mathf.FloorToInt(maxEnergy);
        healthRenders = new GameObject[renderCount];

        for (int i = 0; i < renderCount; i++)
        {
            healthRenders[i] = Instantiate(energySprite, this.transform.position, Quaternion.identity) as GameObject;
            healthRenders[i].transform.parent = this.transform;
            healthRenders[i].renderer.material.color = this.usedColor;
        }
    }
    
    // -----------------------------------------------------------------------

}
