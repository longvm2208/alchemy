using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] LoadProgress loadProgress;
    [SerializeField] ItemId[] baseItemIds;

    IEnumerator Start()
    {
        Application.targetFrameRate = 60;

        // Fake load
        Tween tween = DOVirtual.Float(0, 0.8f, 3, progress =>
        {
            loadProgress.SetProgress(progress);
        });

        TimeManager.Ins.FetchTime();
        yield return new WaitUntil(() => TimeManager.Ins.IsFetched);

        //yield return null;
        //FirebaseManager.Ins.Init();
        //AdsManager.Ins.Init();
        //PurchaseManager.Ins.Init();

        //bool timeout = false;
        //this.Invoke(5, () => timeout = true);

        //yield return new WaitUntil(() => AdsManager.Ins.Initialized || timeout);

        yield return null;
        UIManager.Ins.Init();
        yield return null;
        AudioManager.Ins.Init();
        yield return null;
        UpdateBaseItemIds();

        yield return new WaitUntil(() => !tween.active);

        UIManager.Ins.TransitionAnimation.Close(() =>
        {
            SceneManager.LoadSceneAsync(1);
        });
    }

    void UpdateBaseItemIds()
    {
        for (int i = 0; i < baseItemIds.Length; i++)
        {
            if (GamePref.Ins.DiscoveredItems.Contains(baseItemIds[i]))
            {
                continue;
            }

            GamePref.Ins.DiscoveredItems.Add(baseItemIds[i]);
        }
    }
}
