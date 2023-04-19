using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using DG.Tweening;
using UnityEngine;

public class PopupAnimation : Popup
{
    [SerializeField] private AnimancerComponent animancerComponent;
    public void SetAnimation(AnimationClip getAnimationClip, Action getAction)
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
