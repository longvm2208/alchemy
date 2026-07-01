using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PaginationButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image image;

    public void OnClick(UnityAction onClick)
    {
        button.onClick.RemoveListener(onClick);
        button.onClick.AddListener(onClick);
    }

    public void SetEnable(bool enable)
    {
        button.interactable = enable;
        image.SetAlpha(enable ? 1 : 0.5f);
    }
}
