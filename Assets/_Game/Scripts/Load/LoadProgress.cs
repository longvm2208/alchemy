using TMPro;
using UnityEngine;

public class LoadProgress : MonoBehaviour
{
    [SerializeField] ProgressBar progressBar;
    [SerializeField] float loadingTextInterval = 0.5f;
    [SerializeField] RectTransform indicatorTransform;
    [SerializeField] TMP_Text percentText;

    public void SetProgress(float progress)
    {
        progressBar.SetProgress(progress);
        if (indicatorTransform != null) indicatorTransform.SetAnchorPosX(progressBar.Size * (progress - 0.5f));
        if (percentText != null) percentText.text = Mathf.RoundToInt(progress * 100).ToString() + "%";
    }
}
