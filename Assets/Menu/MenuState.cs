using System;


public class MenuState : GM_State {
	
	private UpdateRet ret = UpdateRet.CONTINUE;
    private GM_State nextState = null;
	public static MenuState instance;
	
	public MenuState ()	{
		instance = this;
	}
	
	public void startGame() {
		nextState = new GM_GS_SvGame();
        ret = UpdateRet.NEXT_STATE;
	}
	
	public override UpdateRet Update() {
        return ret;
    }
    
	public override GM_State NextState() {
        return nextState;
    }
}
