using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SelectionMenu : MonoBehaviour
{
  public GameObject LastSelectedObject
  {
    get
    {
      if (lastChildIndex > 0 & lastChildIndex < childrenList.Count)
      {
        return childrenList[lastChildIndex].gameObject;
      }
      else
        return null;
    }
  }

  private int lastChildIndex
  { get
    {
      return _index;
    }
    set
    {
      _index = value;
      if (value < 0)
      {
        indicator.gameObject.SetActive(false);
      }
      else
      {
        indicator.gameObject.SetActive(true);
      }
    }
  }
  private int _index = -1;
  private List<RectTransform> childrenList;
  private void Start()
  {
    Init();
  }
  private void OnTransformChildrenChanged()
  {
    Init();
  }

  void Init()
  {
    childrenList = new List<RectTransform>();
    for (int i = 0; i < transform.childCount; i++)
    {
      if (!transform.GetChild(i).GetComponent<SelectionMenuItem>())
      {
        var smi = transform.GetChild(i).gameObject.AddComponent<SelectionMenuItem>();
        smi.Set(this);
        childrenList.Add((RectTransform)smi.transform);
      }
      else
      {
        var smi = transform.GetChild(i).GetComponent<SelectionMenuItem>();
        smi.Set(this);
        childrenList.Add((RectTransform)smi.transform);
      }
    }
  }
  public enum Alignment { Right, Left, Top, Bottom, Radial }
  public Alignment indicatorAlignment;
  public RectTransform indicator;
  

  public void OnTriggerEnterChild(int Index)
  {
    lastChildIndex = Index;
    var rt = childrenList[Index];
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
        _pos += direction * diagonal/2;
        break;
    }
    indicator.position = _pos;
    var dir = (rt.position - indicator.position).normalized;
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    indicator.rotation = Quaternion.Euler(indicator.eulerAngles.x, indicator.eulerAngles.y, angle);
  }
  public void OnTriggerExitChild(int Index)
  {
    if (lastChildIndex == Index)
    {
      lastChildIndex = -1;
    }
  }
}
