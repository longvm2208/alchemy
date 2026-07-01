using DG.Tweening;
using UnityEngine;

public class GameTut : SingletonMonoBehaviour<GameTut>
{
    public enum TutType
    {
        None,
        Merge,
    }

    [SerializeField] Transform mergeBoardCenter;
    [SerializeField] StartItemMenu startItemMenu;
    [SerializeField] ClickAndDragHand cadHand;

    TutType type;
    int currentStep;
    StartItemView forceItem;

    public void Init()
    {
        int level = GamePref.Ins.LevelIndex + 1;

        type = TutType.None;
        currentStep = 0;

        if (GamePref.Ins.IsMergeTut && level == GameConf.Ins.MergeTutLevel)
        {
            type = TutType.Merge;
        }
    }

    public void NextMergeStep()
    {
        if (type != TutType.Merge) return;

        switch (currentStep++)
        {
            case 0:
                StartItemView startItem1 = startItemMenu.GetStartItem1();
                cadHand.gameObject.SetActive(true);
                cadHand.Play(startItem1.transform, mergeBoardCenter);
                forceItem = startItem1;
                break;
            case 1:
                ItemView item1 = BoardManager.Ins.GetItem1();
                StartItemView startItem2 = startItemMenu.GetStartItem2();
                cadHand.Play(startItem2.transform, item1.transform);
                forceItem = startItem2;
                break;
            case 2:
                ItemView item2 = BoardManager.Ins.GetItem2();
                item1 = BoardManager.Ins.GetItem1();
                cadHand.Play(item2.transform, item1.transform);
                forceItem = null;
                break;
            default:
                type = TutType.None;
                currentStep = 0;
                cadHand.Stop();
                cadHand.gameObject.SetActive(false);
                break;
        }
    }

    public bool CanUseStartItem(StartItemView startItem)
    {
        if (type == TutType.Merge)
        {
            return startItem == forceItem;
        }

        return true;
    }
}
