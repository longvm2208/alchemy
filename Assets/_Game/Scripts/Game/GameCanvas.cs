using TMPro;
using UnityEngine;

public class GameCanvas : SingletonCanvas<GameCanvas>
{
    [SerializeField] RectTransform bottom;
    [SerializeField] GameTimer timer;
    public GameTimer Timer => timer;
    [SerializeField] TMP_Text levelText;

    public void Init()
    {
        float bannerHeight = 165f;
        bottom.SetAnchorPosY(bannerHeight);
        levelText.text = $"Lv.{GamePref.Ins.LevelIndex + 1}";
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
        UIManager.Ins.Open<PopupBook>();
    }
    #endregion
}
