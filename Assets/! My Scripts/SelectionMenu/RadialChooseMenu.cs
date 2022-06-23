using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadialChooseMenu : MonoBehaviour
{
  #region Public Fields
  public RadialMenuData radialMenuData;
  /// <summary>
  /// Triggered whenever the selection changes i.e new element has been selected from the list
  /// <para>First transform is the old selection</para>
  /// <para>Second transform is the new selection</para>
  /// </summary>
  public Action<Transform, Transform> OnSelectionChanged;
  public Action OnSelectionDeactivated;
  public Dictionary<Vector2, RectTransform> SnapPoints = new Dictionary<Vector2, RectTransform>();
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
    set
    {
      if (_lastSelected != value || value == null)
      {
        if (_lastSelected)
        {
          var smc = _lastSelected.GetComponent<SelectionMenuContainer>();
          if (smc)
          {
            smc.OnDeselected?.Invoke();
          }
        }
        if (value)
        {
          var smc = value.GetComponent<SelectionMenuContainer>();
          if (smc)
          {
            smc.OnSelected?.Invoke();
          }
        }
        OnSelectionChanged?.Invoke(_lastSelected, value);
      }
      _lastSelected = value;
    }
  }
  #endregion

  #region Exposed Private Fields
  [SerializeField][InputAxis] string HorizontalAxis;
  [SerializeField][InputAxis] string VerticalAxis;
  [SerializeField] bool useMouseMovement;
  [SerializeField] GameObject raycastBlocker;
  #endregion

  #region Private Fields
  private List<Vector2> _objectSnapPoints = new List<Vector2>();
  private List<RectTransform> _objectPositions = new List<RectTransform>();
  private Transform _lastSelected;

  private Vector2 FakeCursorPosition
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
  #endregion

  #region MonoBehavior Callbacks
  private void Start()
  {
    GenerateSnapPoints();
    PopulateRadialMenuFromData();
  }

  private void OnTransformChildrenChanged()
  {
    GenerateSnapPoints();
  }
  #endregion

  #region Public Methods
  public void Activate()
  {
    if (transform.childCount > SnapPoints.Count)
    {
      LastSelectedObject = null;
      GenerateSnapPoints();
      return;
    }
    Vector2 cursorDirection = Vector3.Normalize(Vector2.zero - FakeCursorPosition);
    float closestDot = -1f;
    Transform _closestObject = null;
    foreach (KeyValuePair<Vector2, RectTransform> pair in SnapPoints)
    {
      var _dot = Vector3.Dot(cursorDirection, pair.Key);
      if (_dot > closestDot)
      {
        closestDot = _dot;
        _closestObject = pair.Value;
      }
    }
    LastSelectedObject = _closestObject;
    raycastBlocker.SetActive(true);
  }
  public void Deactivate()
  {
    raycastBlocker.SetActive(false);
    OnSelectionDeactivated?.Invoke();

  }

  public void RegenerateSnapPoints()
  {
    Debug.Log("RegeneratingSnapPoints");
    GenerateSnapPoints();
  }

  public void PopulateRadialMenuFromData()
  {
    if (radialMenuData)
      for (int i = 0; i < transform.childCount; i++)
      {
        var tr = transform.GetChild(i).GetComponent<SelectionMenuContainer>();
        if (tr != null)
        {
          if (radialMenuData.orderedItems.Count > i && radialMenuData.orderedItems[i])
          {
            tr.container = radialMenuData.orderedItems[i];
            tr.image.sprite = radialMenuData.orderedItems[i].ico;
          }
          else
          {
            tr.container = null;
            tr.image.sprite = tr.defaultIcon;
          }
        }
      }
  }

  #endregion

  #region Private Methods
  private void GenerateSnapPoints()
  {
    _objectPositions = new List<RectTransform>();
    for (int i = 0; i < transform.childCount; i++)
    {
      var tr = transform.GetChild(i).gameObject.transform;
      _objectPositions.Add((RectTransform)tr);
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

  #endregion
}
