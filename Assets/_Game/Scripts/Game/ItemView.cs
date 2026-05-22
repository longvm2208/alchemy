using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemView : PooledMonoBehaviour<ItemView>,
    IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Vector2 strength;
    [SerializeField] int vibrato;
    [SerializeField] Image iconImage;
    [SerializeField] RectTransform iconRect;
    [SerializeField] RectTransform mergeCircle;
    [SerializeField] RectTransform glowRect;
    [SerializeField] Image glowImage;

    RectTransform _rect;
    public RectTransform rect => _rect ? _rect : _rect = GetComponent<RectTransform>();
    ItemId id;
    public ItemId Id => id;
    ItemView mergeCandidate;

    public void Init(ItemId id)
    {
        this.id = id;

        ItemDefinition itemDefinition = DatabaseManager.Ins.GetItemDefinition(id);

        if (itemDefinition == null) return;

        iconImage.sprite = itemDefinition.Icon;
    }

    public void SetMergeCandidate(ItemView nextCandidate)
    {
        if (mergeCandidate == nextCandidate) return;

        mergeCandidate?.ShowMergeCircle(false);

        mergeCandidate = nextCandidate;

        mergeCandidate?.ShowMergeCircle(true);
    }

    public void ShowMergeCircle(bool show)
    {
        mergeCircle.DOKill();
        mergeCircle.DOScale(show ? Vector3.one : Vector3.zero, 0.25f);
    }

    public void StartHint()
    {
        glowImage.DOKill();
        glowImage.SetAlpha(0);
        glowImage.DOFade(1, 0.1f);

        iconRect.DOKill();
        iconRect.localScale = Vector3.one;
        iconRect.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        glowRect.DOKill();
        glowRect.localRotation = Quaternion.identity;
        glowRect.DORotate(-360f * Vector3.forward, 8, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    public void StopHint()
    {
        glowImage.DOKill();
        glowImage.DOFade(0, 0.25f);

        iconRect.DOKill();
        iconRect.DOScale(1, 0.25f);

        glowRect.DOKill();
    }

    public void Remove()
    {
        rect.DOKill();
        rect.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
        {
            Destroy();
        });
    }

    public void Destroy()
    {
        rect.DOKill();
        rect.localScale = Vector3.one;

        mergeCircle.DOKill();
        mergeCircle.localScale = Vector3.zero;

        iconRect.DOKill();
        iconRect.localScale = Vector3.one;

        glowImage.DOKill();
        glowImage.SetAlpha(0);

        glowRect.DOKill();
        glowRect.localRotation = Quaternion.identity;

        mergeCandidate = null;

        Recycle();
    }

    #region Handle Input
    public void OnPointerDown(PointerEventData eventData)
    {
        VibrationManager.Ins.Vibrate();
        UIManager.Ins.EnableBlocker(true);
        rect.SetAsLastSibling();

        rect.DOKill();
        rect.DOScale(1.2f * Vector3.one, 0.25f);

        BoardManager.Ins.UpdateMergeCandidate(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / BoardManager.Ins.ScaleFactor;
        BoardManager.Ins.UpdateMergeCandidate(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIManager.Ins.EnableBlocker(false);

        if (BoardManager.Ins.IsFullyInside(rect))
        {
            if (mergeCandidate != null)
            {
                if (!BoardManager.Ins.TryMerge(this, mergeCandidate))
                {
                    rect.DOKill();
                    rect.DOScale(Vector3.one, 0.25f);
                    rect.DOShakeAnchorPos(0.25f, strength, vibrato, 0);
                }

                mergeCandidate.ShowMergeCircle(false);
                mergeCandidate = null;
            }
            else
            {
                rect.DOKill();
                rect.DOScale(Vector3.one, 0.25f);
            }
        }
        else
        {
            if (mergeCandidate != null)
            {
                mergeCandidate.ShowMergeCircle(false);
                mergeCandidate = null;
            }

            BoardManager.Ins.RemoveItem(this);

            BoosterManager.Ins.UpdateHint();

            Remove();
        }
    }
    #endregion
}
