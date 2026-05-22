using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TargetItemFly : PooledMonoBehaviour<TargetItemFly>
{
    [SerializeField] Image iconImage;
    [SerializeField] AnimationCurve moveYCurve;

    public void Init(ItemId id, Vector3 pos)
    {
        ItemDefinition itemDefinition = DatabaseManager.Ins.GetItemDefinition(id);

        if (itemDefinition == null) return;

        iconImage.sprite = itemDefinition.Icon;
        transform.position = pos;
        transform.localScale = 0.5f * Vector3.one;
    }

    public void Move(Vector3 targetPos, Action onComplete = null)
    {
        transform.DOScale(1, 0.5f).SetEase(Ease.InQuad);
        transform.DOMoveX(targetPos.x, 0.5f).SetEase(Ease.InQuad);
        transform.DOMoveY(targetPos.y, 0.5f).SetEase(moveYCurve).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void Destroy()
    {
        Recycle();
    }
}
