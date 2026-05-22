using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public class RewardAnimator : MonoBehaviour
    {
        [SerializeField] bool isAutoTrigger;
        [SerializeField] float autoTriggerDelay = 2;
        [SerializeField] BackgroundAnimation backgroundAnimation;
        [SerializeReference] RewardArranger rewardArranger;
        [SerializeField] RewardAnimation[] rewardAnimations;

        bool isTriggered = false;
        int working;
        WaitUntil waitUntilTriggered;
        List<RectTransform> activeTransforms = new();
        List<RewardAnimation> activeAnimations = new();

        public WaitUntil WaitUntilComplete { get; private set; }

        event Action onComplete;

        public void SetDestination(int typeId, Transform destination) => rewardAnimations[typeId].SetDestination(destination);

        [Button]
        public void Init()
        {
            isTriggered = false;
            working = 0;
            activeTransforms.Clear();
            activeAnimations.Clear();
            if (waitUntilTriggered == null) waitUntilTriggered = new WaitUntil(() => isTriggered);
            if (WaitUntilComplete == null) WaitUntilComplete = new WaitUntil(() => working == 0);
        }

        [Button(ButtonStyle.FoldoutButton)]
        public void Add(int typeId, int amount, Action onReceiveReward = null)
        {
            if (typeId < 0) return;
            working++;
            RewardAnimation animation = rewardAnimations[typeId];
            activeAnimations.Add(animation);
            activeTransforms.Add(animation.MyRt);
            animation.Init(amount);
            animation.OnReceiveReward(onReceiveReward);
            animation.OnComplete(Complete);

            void Complete()
            {
                working--;
                animation.gameObject.SetActive(false);
            }
        }

        [Button]
        public void Play() => StartCoroutine(PlayCoroutine());
        IEnumerator PlayCoroutine()
        {
            backgroundAnimation.gameObject.SetActive(true);
            backgroundAnimation.Init();
            yield return null;
            backgroundAnimation.Open();
            rewardArranger.Init(activeTransforms);
            yield return null;
            rewardArranger.Play();
            yield return rewardArranger.WaitUntilComplete;
            if (isAutoTrigger) AutoTrigger();
            yield return waitUntilTriggered;
            backgroundAnimation.Close();
            for (int i = 0; i < activeAnimations.Count; i++)
            {
                activeAnimations[i].Play();
            }
            yield return WaitUntilComplete;
            backgroundAnimation.gameObject.SetActive(false);
            onComplete?.Invoke();
            onComplete = null;
        }

        public void OnComplete(Action onComplete) => this.onComplete = onComplete;

        public void Trigger() => isTriggered = true;

        void AutoTrigger() => StartCoroutine(AutoTriggerCoroutine());
        IEnumerator AutoTriggerCoroutine()
        {
            yield return WaitFor.Seconds(autoTriggerDelay);
            Trigger();
        }

        #region UI EVENTS
        public void OnClick()
        {
            Trigger();
        }
        #endregion
    }
}
