using UnityEngine;
using System.Collections;

public class CameraCenterPlayer : Entity {

    // Preferred offset from the camera. This is set based on initial Placement.
    protected Vector3 m_offset;
    
    // Local Player
    protected Transform m_player;
    protected bool m_init = false;

    // -----------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
        base.Start();
       
	}

    // -----------------------------------------------------------------------

	// Update is called once per frame
	protected void Update () {
        if (!m_init)
        {
            m_player = game.player.transform;
            m_offset = this.transform.position - m_player.position;
            m_init = true;
        }
        transform.position = m_player.position + m_offset;
	}
}
