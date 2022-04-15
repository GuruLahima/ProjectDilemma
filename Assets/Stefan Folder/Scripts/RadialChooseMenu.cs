using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialChooseMenu : MonoBehaviour
{
  public RectTransform fakeCursor;

  Vector2 fakeCursorPosition
  {
    get
    {
      var mousePos = Input.mousePosition;
      mousePos.x -= Screen.width / 2;
      mousePos.y -= Screen.height / 2;
      return mousePos.normalized;
    }
  }

  RectTransform snapPoint
  {
    get
    {
      Vector2 closestPoint = Vector2.zero;
      RectTransform rt = null;
      foreach (KeyValuePair<Vector2, RectTransform> pt in SnapPoints)
      {
        if (rt)
        {
          if (Mathf.Abs(fakeCursorPosition.x - pt.Key.x) + Mathf.Abs(fakeCursorPosition.y - pt.Key.y) < 
            Mathf.Abs(fakeCursorPosition.x - closestPoint.x) + Mathf.Abs(fakeCursorPosition.y - closestPoint.y))
          {
            closestPoint = pt.Key;
            rt = pt.Value;
          }
          else
          {
            // it remains the same
          }
        }
        else
        {
          closestPoint = pt.Key;
          rt = pt.Value;
        }
      }
      return rt;
    }
  }

  List<Vector2> _objectSnapPoints = new List<Vector2>();
  [SerializeField] List<RectTransform> _objectPositions = new List<RectTransform>();
  public Dictionary<Vector2, RectTransform> SnapPoints = new Dictionary<Vector2, RectTransform>();

  private void Start()
  {
    _lastPosition = fakeCursorPosition;
    GenerateSnapPoints();
  }

  // ! TEMPORARY FOR TESTING !
  private void Update()
  {
    if (Input.GetKey(KeyCode.T))
    {
      Activate();
      Debug.Log("Cursor at: " + fakeCursorPosition);
    }
    if (Input.GetKeyUp(KeyCode.T))
    {
      Deactivate();
    }
  }

  private void GenerateSnapPoints()
  {
    _objectSnapPoints = new List<Vector2>();
    float maxDistanceX = 0; float maxDistanceY = 0;
    float minDistanceX = 0; float minDistanceY = 0;
    foreach (RectTransform rt in _objectPositions)
    {
      if (rt.anchoredPosition.x > maxDistanceX) maxDistanceX = rt.anchoredPosition.x;
      if (rt.anchoredPosition.x < minDistanceX) minDistanceX = rt.anchoredPosition.x;

      if (rt.anchoredPosition.y > maxDistanceY) maxDistanceY = rt.anchoredPosition.y;
      if (rt.anchoredPosition.y < minDistanceY) minDistanceY = rt.anchoredPosition.y;
    }

    foreach (RectTransform rt in _objectPositions)
    {
      _objectSnapPoints.Add(new Vector2((rt.anchoredPosition.x - minDistanceX) / (maxDistanceX - minDistanceX),
        (rt.anchoredPosition.y - minDistanceY) / (maxDistanceY - minDistanceY)));
    }

      int n = (_objectPositions.Count > _objectSnapPoints.Count) ? _objectPositions.Count : _objectSnapPoints.Count;
    for (int i = 0; i < n; i++)
    {
      SnapPoints.Add(_objectSnapPoints[i], _objectPositions[i]);
    }
  }

  Vector2 _lastPosition;
  
  [SerializeField] float positionDistanceChange = 0.1f;
  public void Activate()
  {
    if (SnapPoints.Count <= 0)
    {
      GenerateSnapPoints();
      return;
    }
    if (Vector2.Distance(_lastPosition, fakeCursorPosition) > positionDistanceChange)
    {
      _lastPosition = fakeCursorPosition;
      fakeCursor.anchoredPosition = snapPoint.anchoredPosition;
    }
    fakeCursor.gameObject.SetActive(true);


  }

  public void Deactivate()
  {
    fakeCursor.gameObject.SetActive(false);
  }



}
