using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class SplitScreenAnim
{
  [Range(0, 10)]
  [SerializeField] public float durationOfSlide;
  [SerializeField] public Camera cam;
  [SerializeField] Ease typeOfSlide;

  // Update is called once per frame
  public void AnimateCameraWidth(float widthStart = 0f, float widthEnd = 1f, float xStartPos = 0.5f, float xEndPos = 0f, float yStartPos = 0f, float yEndPos = 0f)
  {
    Rect tempRect = cam.rect;
    tempRect.x = xStartPos;
    tempRect.y = yStartPos;
    tempRect.width = widthStart;
    cam.rect = tempRect;
    Tween wTween = DOTween.To(() => cam.rect.width,
     (x) => { tempRect.width = x; cam.rect = tempRect; },
     widthEnd,
     durationOfSlide);
    Tween xTween = DOTween.To(() => cam.rect.x,
     (x) => { tempRect.x = x; cam.rect = tempRect; },
     xEndPos,
     durationOfSlide);
    Tween yTween = DOTween.To(() => cam.rect.y,
     (x) => { tempRect.y = x; cam.rect = tempRect; },
     yEndPos,
     durationOfSlide);

    wTween.SetEase(typeOfSlide);
    xTween.SetEase(typeOfSlide);
    yTween.SetEase(typeOfSlide);
  }
}
