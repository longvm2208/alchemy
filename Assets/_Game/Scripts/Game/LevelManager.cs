using UnityEngine;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    [SerializeField] LevelContainer levelContainer;

    LevelData currentLevel;
    public LevelData CurrentLevel => currentLevel;

    bool isFirstInteraction;

    public void LoadLevel()
    {
        int levelIndex = GamePref.Ins.LevelIndex % levelContainer.Levels.Length;
        currentLevel = levelContainer.Levels[levelIndex];
        GameCanvas.Ins.UpdateBottomPos();
        BoardManager.Ins.UpdateBoardPosAndSize();
        DatabaseManager.Ins.Init();
        BoardManager.Ins.Init();
        BoosterManager.Ins.Init();
        GameCanvas.Ins.Timer.Init(levelIndex);
        isFirstInteraction = true;
    }

    public void OnUserInteract()
    {
        if (isFirstInteraction)
        {
            isFirstInteraction = false;
            GameCanvas.Ins.Timer.Play();
        }
    }

    public void LoseLevel()
    {
        if (GamePref.Ins.LevelIndex > 0)
        {
            SceneController.Ins.ToHome();
        }
        else
        {
            SceneController.Ins.ToGame();
        }
    }

    public void ReviveLevel()
    {
        GameCanvas.Ins.Timer.Revive();
    }

    public void WinLevel(float delay = 0)
    {
        GamePref.Ins.LevelIndex++;

        UIManager.Ins.EnableBlocker(true);

        this.Invoke(delay, () =>
        {
            UIManager.Ins.EnableBlocker(false);
            UIManager.Ins.Open<PopupWin>();
        });
    }
}
