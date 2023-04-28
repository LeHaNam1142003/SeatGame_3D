using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using Mono = Pancake.Mono;

public class SeatEmotionUI : MonoBehaviour
{
    [SerializeField] private AnimancerComponent animancerComponent;
    [SerializeField] private AnimationClip initializeAnim;
    private void OnEnable()
    {
        SetAnimation(initializeAnim, (() => gameObject.SetActive(false)));
    }
    void SetAnimation(AnimationClip getAnimationClip, Action getAction)
    {
        StartCoroutine(DoAnimationUI(getAnimationClip, getAction));
    }
    IEnumerator DoAnimationUI(AnimationClip getAnimationClip, Action getAction)
    {
        var playanim = animancerComponent.Play(getAnimationClip);
        yield return playanim;
        getAction?.Invoke();
    }
}
