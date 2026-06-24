using TMPro;
using Toolkit;
using UnityEngine;

public class CheatWin : CheatAction
{
    public override void Execute()
    {
        if (SceneController.Ins.CurrentScene == Scene.Game)
        {
            LevelManager.Ins.WinLevel();
        }
    }
}

public class CheatLevel : CheatAction
{
    [SerializeField] TMP_InputField input;

    public override void Init()
    {
        base.Init();
        input.text = (GamePref.Ins.LevelIndex + 1).ToString();
    }

    public override void Execute()
    {
        if (int.TryParse(input.text, out int value) && value >= 1)
        {
            GamePref.Ins.LevelIndex = value - 1;
            SceneController.Ins.ToGame();
        }
    }
}

public class CheatCoin : CheatAction
{
    [SerializeField] TMP_InputField input;

    public override void Init()
    {
        base.Init();
        input.text = "100000";
    }

    public override void Execute()
    {
        if (int.TryParse(input.text, out int value))
        {
            GamePref.Ins.AddCoin(value, null, true);
        }
    }
}

public class CheatHint : CheatAction
{
    [SerializeField] TMP_InputField input;

    public override void Init()
    {
        base.Init();
        input.text = "100";
    }

    public override void Execute()
    {
        if (int.TryParse(input.text, out int value))
        {
            GamePref.Ins.AddHint(value, null, true);
        }
    }
}

public class CheatExtraTime : CheatAction
{
    [SerializeField] TMP_InputField input;

    public override void Init()
    {
        base.Init();
        input.text = "100";
    }

    public override void Execute()
    {
        if (int.TryParse(input.text, out int value))
        {
            GamePref.Ins.AddExtraTime(value, null, true);
        }
    }
}
