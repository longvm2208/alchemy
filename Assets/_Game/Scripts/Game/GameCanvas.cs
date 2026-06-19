using UnityEngine;

public class GameCanvas : SingletonCanvas<GameCanvas>
{
    [SerializeField] RectTransform bottom;
    [SerializeField] GameTimer timer;
    public GameTimer Timer => timer;

    public void UpdateBottomPos()
    {
        float bannerHeight = 165f;
        bottom.SetAnchorPosY(bannerHeight);
    }

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
