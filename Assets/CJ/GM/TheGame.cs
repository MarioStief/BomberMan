using UnityEngine;
using System.Collections;

public abstract class GM_State
{
    public enum UpdateRet
    {
        CONTINUE,
        NEXT_STATE
    }

    public virtual void Start() { }
    public virtual void Stop() { }
    public virtual void OnGUI() { }
    public virtual UpdateRet Update() { return UpdateRet.CONTINUE; }
    public virtual void HandleMessage(NET_Message msg) { }

    public virtual GM_State NextState() { return null; }
}

public class TheGame : MonoBehaviour {
    private GM_State state = null;

    public void SetState(GM_State state)
    {
        if (null != this.state) this.state.Stop();
        this.state = state;
        this.state.Start();
    }

    public TheGame()
    {
        //SetState(new GM_GS_MainMenu());
		SetState(new MenuState());
    }

    public void OnGUI()
    {
        state.OnGUI();
    }

    public void Update()
    {
        if (GM_State.UpdateRet.NEXT_STATE == state.Update())
            SetState(state.NextState());
    }

    public void HandleMessage(NET_Message msg)
    {
        if (null != state) state.HandleMessage(msg);
    }
}
