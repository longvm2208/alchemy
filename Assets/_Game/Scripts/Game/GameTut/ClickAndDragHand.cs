using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ClickAndDragHand : MonoBehaviour
{
    [SerializeField] GameObject click;
    [SerializeField] GameObject release;

    IEnumerator playCoroutine;

    public void Click()
    {
        click.SetActive(true);
        release.SetActive(false);
    }

    public void Release()
    {
        click.SetActive(false);
        release.SetActive(true);
    }

    public void Play(Transform start, Transform end)
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
        }

        transform.DOKill();

        playCoroutine = PlayCoroutine(start, end);

        StartCoroutine(playCoroutine);
    }

    IEnumerator PlayCoroutine(Transform start, Transform end)
    {
        while (true)
        {
            transform.position = start.position;
            Click();
            yield return WaitFor.Seconds(0.5f);
            transform.DOMove(end.position, 1f);
            yield return WaitFor.Seconds(1);
            Release();
            yield return WaitFor.Seconds(0.5f);
            transform.DOMove(start.position, 0.5f);
            yield return WaitFor.Seconds(0.5f);
        }
    }

    public void Stop()
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            transform.DOKill();
        }
    }
}
