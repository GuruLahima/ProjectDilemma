using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class SplitScreenAnim
{
  [SerializeField] public Camera cam;

  // Update is called once per frame
  public void AnimateCameraWidth(float widthStart = 0f, float widthEnd = 1f, float xStartPos = 0.5f, float xEndPos = 0f, float yStartPos = 0f, float yEndPos = 0f, Ease slideType = Ease.Unset, float duration = 2f)
  {
    Rect tempRect = cam.rect;
    tempRect.x = xStartPos;
    tempRect.y = yStartPos;
    tempRect.width = widthStart;
    cam.rect = tempRect;
    Tween wTween = DOTween.To(() => cam.rect.width,
     (x) => { tempRect.width = x; cam.rect = tempRect; },
     widthEnd,
     duration);
    Tween xTween = DOTween.To(() => cam.rect.x,
     (x) => { tempRect.x = x; cam.rect = tempRect; },
     xEndPos,
     duration);
    Tween yTween = DOTween.To(() => cam.rect.y,
     (x) => { tempRect.y = x; cam.rect = tempRect; },
     yEndPos,
     duration);

    wTween.SetEase(slideType);
    xTween.SetEase(slideType);
    yTween.SetEase(slideType);
  }
}
