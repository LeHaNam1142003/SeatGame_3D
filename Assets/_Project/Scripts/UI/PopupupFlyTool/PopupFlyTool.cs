using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupFlyTool : PopupAnimation
{
    [SerializeField] private AnimationClip moveBoard;
    protected override void AfterShown()
    {
        SetAnimation(moveBoard, null);
        base.AfterShown();
    }
    public void Cancel()
    {
        Level.Instance.EndDoFlyTool();
        Hide();
    }
}
