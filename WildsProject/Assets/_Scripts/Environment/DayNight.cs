using UnityEngine;
using System.Collections;

/// <summary>
/// Specify a series of points for how to tint the scene (day / night / etc )
/// Probably UNUSED in OofN
/// </summary>
public class DayNight : MonoBehaviour {

    [System.Serializable]
    public class DayPoint
    {
        public Color color = Color.white;
        public float time = 0.0f;
        public float lightAngle = 90;
        [HideInInspector]
        public Quaternion lightQuaternion;
    }

    // If false, we can make light static for optimization.
    // Not sure how important this is, really.
    public bool rotateLight = true;

    public DayPoint[] points;
    public int defaultIndex = 0;

    private Light m_directionalLight;
    private int index = 0;
    private int maxIndex = 0;

    private float m_timer = 0.0f;

    private float m_startTime = 0.0f;
    private float m_endTime = 0.0f;
    private float m_duration = 0.0f;

    private Color m_startColor, m_endColor;
    private Quaternion m_startQuaternion, m_endQuaternion;

    // -----------------------------------------------------------------------

	// Use this for initialization
	void Start () {
        maxIndex = points.Length - 1;
        m_directionalLight = GetComponentInChildren<Light>();
        for (index = 0; index < defaultIndex; index++)
        {
            m_timer += points[index].time;
        }

        Vector3 lightEulers = m_directionalLight.transform.rotation.eulerAngles;

        // Compute Quaternions based on input angle
        for (index = 0; index < points.Length; index++)
        {
            points[index].lightQuaternion = Quaternion.Euler(points[index].lightAngle, lightEulers.y, lightEulers.z);
        }
        Advance();
	}

    // -----------------------------------------------------------------------

    // Update is called once per frame
    void Update()
    {
        if (points.Length < 1)
        {
            Debug.LogWarning("No Points Specified for Day/Time Cycle");
            return;
        }
        
        m_timer += Time.deltaTime;

        if (m_timer > m_endTime)
        {
            Advance();
        }

        float t = 1.0f - (m_endTime - m_timer) / m_duration;

        m_directionalLight.color = Color.Lerp(m_startColor, m_endColor, t);
        m_directionalLight.transform.localRotation = Quaternion.Slerp(m_startQuaternion, m_endQuaternion, t);
        //m_directionalLight.transform.Rotate(
	}

    // -----------------------------------------------------------------------

    private void Advance()
    {
        if (++index > maxIndex)
        {
            index = 0;
            m_timer = 0.0f;
        }
        m_startTime = points[index].time;
        m_startColor = points[index].color;
        m_startQuaternion = points[index].lightQuaternion;
        if (index + 1 <= maxIndex)
        {
            m_endTime = points[index + 1].time;
            m_endColor = points[index + 1].color;
            m_endQuaternion = points[index + 1].lightQuaternion;
        }
        else
        {
            m_endTime = points[0].time;
            m_endColor = points[0].color;
            m_endQuaternion = points[0].lightQuaternion;
        }
        m_duration = m_endTime - m_startTime;
    }
}
