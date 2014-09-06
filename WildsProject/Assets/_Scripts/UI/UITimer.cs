using UnityEngine;
using System.Collections;

public class UITimer : TextBox {

    public Color stoppedColor = Color.white;
    public Color countingColor = Color.yellow;

    protected bool m_isCounting = false;
    protected float m_timer = 10.0f;

    // Cast to int for display each frame
    protected int m_displayTime = 0;
    
    protected eTimerState m_currentState = eTimerState.STOPPED;
    protected enum eTimerState { STOPPED, COUNTDOWN, COUNTUP }

    // -----------------------------------------------------------------------
    
    public void AddTime(float amount)
    {
        m_timer += amount;
    }

    // -----------------------------------------------------------------------

    public bool IsStopped()
    {
        return m_currentState == eTimerState.STOPPED;
    }

    // -----------------------------------------------------------------------

    public void Stop()
    {
        m_currentState = eTimerState.STOPPED;
        SetTextColor(stoppedColor);
    }

    // -----------------------------------------------------------------------

    public void SetTimer(float timeRemaining)
    {
        m_timer = timeRemaining;
    }

    // -----------------------------------------------------------------------

    public void BeginCountdown(float timeRemaining)
    {
        m_timer = timeRemaining;
        BeginCountdown();
    }

    // -----------------------------------------------------------------------

    public void BeginCountdown()
    {
        m_currentState = eTimerState.COUNTDOWN;
        SetTextColor(countingColor);
    }

    // -----------------------------------------------------------------------

	// Update is called once per frame
	protected void Update () {
        if (m_currentState == eTimerState.COUNTDOWN)
        {
            m_timer -= Time.deltaTime;
            if (m_timer < 0.0f)
            {
                Stop();
            }
            m_displayTime = Mathf.CeilToInt(m_timer);
            SetText(m_displayTime.ToString());
        }
	}
}
