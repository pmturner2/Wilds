using UnityEngine;
using System.Collections;

/// <summary>
/// Slides UVs on the material. Meant to appear "Continuous". Could augment for sprite anim.
/// </summary>
public class UVTranslate : MonoBehaviour {
    /// <summary>
    /// If useSharedMaterial is false, each instance will require its own draw call.
    /// We can augment all of this with actual UV displacement in the geometry, and still share the material.
    /// Processing then scales with number of vertices per frame, which can be great. Choose carefully.
    /// </summary>
    public bool useSharedMaterial = false;

    public Vector2 translationSpeed = Vector2.zero;

    // -----------------------------------------------------------------------

    protected Material m_mat;
    protected Vector2 m_currentTranslation;

	// Use this for initialization
	void Start () {
        if (useSharedMaterial)
        {
            m_mat = renderer.sharedMaterial;
        }
        else
        {
            m_mat = renderer.material;
        }
        m_currentTranslation = m_mat.GetTextureOffset("_MainTex");
	}

    // -----------------------------------------------------------------------
    	
	// Update is called once per frame
	void Update () {
        m_currentTranslation += translationSpeed * Time.deltaTime;
        m_mat.SetTextureOffset("_MainTex", m_currentTranslation);
	}
}
