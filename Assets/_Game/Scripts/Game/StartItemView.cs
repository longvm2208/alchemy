using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartItemView : PooledMonoBehaviour<StartItemView>,
    IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    enum Direction
    {
        None,
        Horizontal,
        Vertical
    }

    [SerializeField] float dragThreshold = 10;
    [SerializeField] float dragDirectionRatio = 1.5f;
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] RectTransform dragRect;

    ItemId id;
    StartItemMenu menu;
    Direction dragDirection;
    Vector2 dragStartPos;
    ItemView itemView;

    RectTransform _rect;
    public RectTransform rect => _rect ? _rect : _rect = GetComponent<RectTransform>();

    public void Init(ItemId id, StartItemMenu menu)
    {
        this.id = id;
        this.menu = menu;

        ItemDefinition itemDefinition = DatabaseManager.Ins.GetItemDefinition(id);

        if (itemDefinition == null) return;

        iconImage.sprite = itemDefinition.Icon;
        iconImage.gameObject.SetActive(itemDefinition.Icon != null);
        nameText.text = itemDefinition.Name;
    }

    public void TweenScale()
    {
        rect.DOKill();
        rect.localScale = Vector3.zero;
        rect.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
    }

    public void Destroy()
    {
        itemView = null;

        rect.DOKill();
        rect.localScale = Vector3.one;

        Recycle();
    }

    #region Handle Input
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.Ins.EnableBlocker(true);
        LevelManager.Ins.OnUserInteract();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragDirection = Direction.None;
        dragStartPos = eventData.position;
        dragRect.anchoredPosition = Vector2.zero;
        menu.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemView != null)
        {
            itemView.OnDrag(eventData);
        }
        else if (dragDirection == Direction.Horizontal)
        {
            menu.OnDrag(eventData);
        }
        else if (dragDirection == Direction.Vertical)
        {
            dragRect.anchoredPosition += eventData.delta / BoardManager.Ins.ScaleFactor;

            itemView = BoardManager.Ins.SpawnItem(id);
            itemView.rect.position = dragRect.position;
            itemView.rect.anchoredPosition += 50 * Vector2.up;
            itemView.OnPointerDown(eventData);
            itemView.OnBeginDrag(eventData);
        }
        else
        {
            Vector2 dragDelta = eventData.position - dragStartPos;

            if (dragDelta.magnitude > dragThreshold)
            {
                if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y) * dragDirectionRatio)
                {
                    dragDirection = Direction.Horizontal;
                }
                else
                {
                    dragDirection = Direction.Vertical;
                    menu.OnEndDrag(eventData);
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemView != null)
        {
            itemView.OnEndDrag(eventData);
            itemView.OnPointerUp(eventData);
            itemView = null;
        }
        else if (dragDirection == Direction.Horizontal)
        {
            menu.OnEndDrag(eventData);
        }

        dragDirection = Direction.None;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIManager.Ins.EnableBlocker(false);
    }
    #endregion
}
