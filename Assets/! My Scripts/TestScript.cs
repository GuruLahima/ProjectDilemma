using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TestScript : MonoBehaviour
{
  [SerializeField] SplitScreenAnim anim;
  [SerializeField] float startWidth;
  [SerializeField] float endWidth;
  [SerializeField] float startXPos;
  [SerializeField] float startYPos;
  [SerializeField] float endXPos;
  [SerializeField] float endYPos;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  public void AnimateCameraWidthSideways()
  {
    anim.AnimateCameraWidth(startWidth, endWidth, startXPos, endXPos, startYPos, endYPos);
  }
}
