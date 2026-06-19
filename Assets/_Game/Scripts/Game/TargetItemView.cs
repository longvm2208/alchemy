using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetItemView : PooledMonoBehaviour<TargetItemView>
{
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text progressText;
    [SerializeField] RectTransform progressRect;
    [SerializeField] RectTransform completedRect;
    [SerializeField] RectTransform progress;

    int requiredCount;
    int currentCount;

    public void Init(LevelTarget target)
    {
        iconImage.sprite = null;
        //BranchDefinition branch = DatabaseManager.Ins.GetBranch(target.GroupId, target.BranchId);
        //nameText.text = branch.Name;
        requiredCount = target.RequiredAmount;
        currentCount = 0;
        progressText.text = $"{currentCount}/{requiredCount}";
    }

    public void UpdateProgress()
    {
        currentCount++;
        progressText.text = $"{currentCount}/{requiredCount}";

        progress.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(progress.DOScale(Vector3.one * 1.5f, 0.125f));
        seq.Append(progress.DOScale(Vector3.one, 0.125f));

        if (currentCount >= requiredCount)
        {
            progressRect.DOKill();
            completedRect.DOKill();
            completedRect.localScale = new Vector3(0, 1, 1);
            Sequence seq1 = DOTween.Sequence();
            seq1.Append(progressRect.DOScale(new Vector3(0, 1, 1), 0.125f));
            seq1.Append(completedRect.DOScale(Vector3.one, 0.125f));
        }
    }

    public void Destroy()
    {
        completedRect.DOKill();
        completedRect.localScale = Vector3.zero;

        progressRect.DOKill();
        progressRect.localScale = Vector3.one;

        Recycle();
    }
}
