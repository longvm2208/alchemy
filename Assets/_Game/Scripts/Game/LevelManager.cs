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

        GameTut.Ins.Init();
        GameCanvas.Ins.Init();
        BoardManager.Ins.UpdateBoardPosAndSize();
        BoardManager.Ins.Init();
        BoosterManager.Ins.Init();
        GameCanvas.Ins.Timer.Init(levelIndex);
        isFirstInteraction = true;

        GameTut.Ins.NextMergeStep();
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
