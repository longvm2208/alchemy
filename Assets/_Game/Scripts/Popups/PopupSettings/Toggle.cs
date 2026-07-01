using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class Toggle : MonoBehaviour
    {
        [SerializeField] Vector2 posXRange;
        [SerializeField] Vector2 rotZRange;
        [SerializeField] RectTransform toggleRect;
        [SerializeField] Image bgImage;
        [SerializeField] Sprite onSprite;
        [SerializeField] Sprite offSprite;

        float currentRotZ;

        public void Switch(bool on, bool tween)
        {
            float posX = on ? posXRange.y : posXRange.x;
            float rotZ = on ? rotZRange.y : rotZRange.x;

            bgImage.sprite = on ? onSprite : offSprite;

            toggleRect.DOKill();

            if (tween)
            {
                toggleRect.DOAnchorPosX(posX, 0.5f);
                DOVirtual.Float(currentRotZ, rotZ, 0.5f, value =>
                {
                    currentRotZ = value;
                    toggleRect.eulerAngles = currentRotZ * Vector3.forward;
                });
            }
            else
            {
                toggleRect.SetAnchorPosX(posX);
                currentRotZ = rotZ;
                toggleRect.eulerAngles = currentRotZ * Vector3.forward;
            }
        }
    }
}
