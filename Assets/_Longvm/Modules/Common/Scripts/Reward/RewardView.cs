using TMPro;
using UnityEngine;

public class RewardView : ExtendedMonoBehaviour
{
    [SerializeField] TMP_Text countText;

    public void Init(RewardConfig config)
    {
        countText.text = config.GetCountString();
    }
}
