using UnityEngine;

public class GameCanvas : SingletonCanvas<GameCanvas>
{
    [SerializeField] GameTimer timer;
    public GameTimer Timer => timer;

    #region Event Listeners
    public void OnClickPause()
    {
        UIManager.Ins.Open<PopupPause>();
    }

    public void OnClickClear()
    {
        BoardManager.Ins.ClearBoard();
    }

    public void OnClickEncyclopedia()
    {
        UIManager.Ins.Open<PopupEncyclopedia>();
    }
    #endregion
}
