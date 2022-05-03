using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialChooseMenu : MonoBehaviour
{
  [SerializeField][InputAxis] string HorizontalAxis;
  [SerializeField][InputAxis] string VerticalAxis;
  [SerializeField] bool useMouseMovement;

  public enum Alignment { Right, Left, Top, Bottom, Radial }
  public Alignment indicatorAlignment;
  public RectTransform indicator;
  public Transform LastSelectedObject
  {
    get
    {
      if (_objectPositions.Count < 1 || !_lastSelected)
      {
        return null;
      }
      else
      {
        return _lastSelected;
      }
    }
  }
  private Transform _lastSelected;
  Vector2 FakeCursorPosition
  {
    get
    {
      if (useMouseMovement)
      {
        var mousePos = Input.mousePosition;
        mousePos.x -= Screen.width / 2;
        mousePos.y -= Screen.height / 2;
        return mousePos.normalized;
      }
      else
      {
        return new Vector2(Input.GetAxis(HorizontalAxis), Input.GetAxis(VerticalAxis));
      }
    }
  }
  List<Vector2> _objectSnapPoints = new List<Vector2>();
  List<RectTransform> _objectPositions = new List<RectTransform>();
  public Dictionary<Vector2, RectTransform> SnapPoints = new Dictionary<Vector2, RectTransform>();

  private void Start()
  {
    GenerateSnapPoints();
  }

  private void OnTransformChildrenChanged()
  {
    GenerateSnapPoints();
  }


  private void GenerateSnapPoints()
  {
    _objectPositions = new List<RectTransform>();
    for (int i = 0; i < transform.childCount; i++)
    {
      if (!transform.GetChild(i).GetComponent<SelectionMenuContainer>())
      {
        var smi = transform.GetChild(i).gameObject.AddComponent<SelectionMenuContainer>();
        _objectPositions.Add((RectTransform)smi.transform);
      }
      else
      {
        var smi = transform.GetChild(i).GetComponent<SelectionMenuContainer>();
        _objectPositions.Add((RectTransform)smi.transform);
      }
    }
    _objectSnapPoints = new List<Vector2>();
    foreach (RectTransform rt in _objectPositions)
    {
      _objectSnapPoints.Add(Vector3.Normalize(Vector2.zero - rt.anchoredPosition));
    }
    SnapPoints.Clear();
    for (int i = 0; i < _objectPositions.Count; i++)
    {
      if (!SnapPoints.ContainsKey(_objectSnapPoints[i]))
      {
        SnapPoints.Add(_objectSnapPoints[i], _objectPositions[i]);
      }
    }
  }

  public void Activate()
  {
    if (transform.childCount > SnapPoints.Count)
    {
      GenerateSnapPoints();
      return;
    }

    Vector2 cursorDirection = Vector3.Normalize(Vector2.zero - FakeCursorPosition);
    float closestDot = -1f;
    foreach (KeyValuePair<Vector2, RectTransform> pair in SnapPoints)
    {
      var _dot = Vector3.Dot(cursorDirection, pair.Key);
      if (_dot > closestDot)
      {
        closestDot = _dot;
        _lastSelected = pair.Value;
      }
    }
    SetIndicator();
  }

  private void SetIndicator()
  {
    if (_lastSelected != null)
    {
      RectTransform rt = (RectTransform)_lastSelected;
      Vector2 _pos = rt.position;
      switch (indicatorAlignment)
      {
        case Alignment.Right:
          _pos.x += rt.sizeDelta.x / 2;
          break;
        case Alignment.Left:
          _pos.x -= rt.sizeDelta.x / 2;
          break;
        case Alignment.Top:
          _pos.y += rt.sizeDelta.y / 2;
          break;
        case Alignment.Bottom:
          _pos.y -= rt.sizeDelta.y / 2;
          break;
        case Alignment.Radial:
          var direction = (Vector2.zero - rt.anchoredPosition).normalized;
          var diagonal = Mathf.Sqrt(rt.sizeDelta.x * rt.sizeDelta.x + rt.sizeDelta.y * rt.sizeDelta.y);
          _pos += direction * diagonal / 2;
          break;
      }

      indicator.position = _pos;
      var dir = (rt.position - indicator.position).normalized;
      float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      indicator.rotation = Quaternion.Euler(indicator.eulerAngles.x, indicator.eulerAngles.y, angle);
      indicator.gameObject.SetActive(true);
    }
  }

  public void Deactivate()
  {

    indicator.gameObject.SetActive(false);
  }
}
