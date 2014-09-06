using UnityEngine;
using System.Collections;

public class MenuScreen : StateGeneric
{
    protected MenuTransition transitionIn = null;
    protected MenuTransition transitionOut = null;

    // -----------------------------------------------------------------------

    public void SetText(string s)
    {
        TextBox tb = GetComponentInChildren<TextBox>();
        if (tb)
        {
            tb.SetText(s);
        }
    }

    // -----------------------------------------------------------------------

    public override bool OnMessage(Message message, Agent entity)
    {

        switch (message.mType)
        {
           // case eMessageType.I_BUTTON_JUMP:
             //   return true;
            default:
                return false;
        }
    }


    // -----------------------------------------------------------------------

    public override void Enter(Agent entity)
    {
        if (transitionIn)
        {
            transitionIn.Begin();
        }
    }

    // -----------------------------------------------------------------------

    public override void Exit(Agent entity)
    {
        if (transitionOut)
        {
            if (transitionOut is MenuFadeTransition)
            {
                // Set a transition timer for the state machine that delays the next state while we fade
                game.ui.SetTransitionTime(((MenuFadeTransition)transitionOut).fadeTime);
                transitionOut.Begin();
            }
            else
            {
                transitionOut.Begin();
            }
        }
    }

    // -----------------------------------------------------------------------

    // Update is called once per frame
    public override void Execute(Agent entity)
    {

    }

    // -----------------------------------------------------------------------

    protected override void Start()
    {
        base.Start();
        MenuTransition[] trans = GetComponents<MenuTransition>();
        for (int i = 0; i < trans.Length; i++)
        {
            if (trans[i].transitionType == eTransitionType.IN)
            {
                transitionIn = trans[i];
            }
            else if (trans[i].transitionType == eTransitionType.OUT)
            {
                transitionOut = trans[i];
            }
        }
    }

}
