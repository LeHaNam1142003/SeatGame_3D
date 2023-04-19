using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSwapTool : PopupAnimation
{
    [SerializeField] private AnimationClip moveBoard;
    protected override void AfterShown()
    {
        SetAnimation(moveBoard, null);
        base.AfterShown();
    }
}
