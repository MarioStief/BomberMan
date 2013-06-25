using System;


public class MenuState : GM_State {
	
	private UpdateRet ret = UpdateRet.CONTINUE;
    private GM_State nextState = null;
	public static MenuState instance;
	
	public MenuState ()	{
		instance = this;
	}
	
	public void startGameServer() {
		nextState = new GM_GS_SvGame();
        ret = UpdateRet.NEXT_STATE;
	}
	
	public override UpdateRet Update() {
        return ret;
    }
    
	public override GM_State NextState() {
        return nextState;
    }
	
	public override void HandleMessage(NET_Message msg) 
	{
        if (UpdateRet.NEXT_STATE == ret)
        {
            // resend messages to the next state
            msg.resend = true;
            return;
        }

        if (NET_Message.MSG_STARTGAME == msg.GetMsgID())
        {
            nextState = new GM_GS_ClGame();
            ret = UpdateRet.NEXT_STATE;
			
			Menu.showGUI = false;
        }
    }
}
