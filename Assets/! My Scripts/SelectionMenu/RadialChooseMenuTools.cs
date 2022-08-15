using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GuruLaghima;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RadialChooseMenu))]
[RequireComponent(typeof(RadialLayoutGroup))]
public class RadialChooseMenuTools : MonoBehaviour
{
  #region Enum Declaration
  public enum Alignment { Right, Left, Top, Bottom, Radial }
  #endregion

  #region Public Fields
  public UnityEvent<Transform, Transform> OnSelectionChangedEvent;
  public UnityEvent<Transform> OnNewTransform;
  #endregion

  #region Exposed Private Fields
  [SerializeField] bool workInInspector = false;
  [Space(10)]
  [SerializeField] private bool IncludeOutlineSelection;
  [SerializeField] private bool IncludeRadialBackground;
  [SerializeField] private bool IncludeZoomOnSelection;
  [SerializeField] private bool IncludeChangeTextOnSelection = true;
  [SerializeField] private Vector3 ZoomSelectedSize;
  [SerializeField] private Vector3 ZoomNormalSize;
  [SerializeField] private float ZoomPositionOffset;
  [SerializeField] private Alignment indicatorAlignment = Alignment.Radial;
  [SerializeField] private RectTransform pointerIndicator;
  [SerializeField] private TextMeshProUGUI textIndicator;
  [SerializeField] private string textWhenMenuIsEmpty;
  [SerializeField] private Sprite backgroundSprite;
  [SerializeField] private Color backgroundColor = Color.white;
  [SerializeField] private float backgroundScaleToParent = 1f;
  [SerializeField] private Vector2 backgroundSize;
  [SerializeField] private bool backgroundToArc;
  [SerializeField] private Vector2 backgroundMinMaxArc = new Vector2(0f, 360f);
  [SerializeField] private float backgroundOffset;
  [SerializeField] private bool backgroundOutline;
  [SerializeField] private Color backgroundOutlineColor = Color.white;
  [SerializeField] private Vector2 backgroundOutlineSize = new Vector2(5f, -5f);
  [SerializeField] private int backgroundSortingOrder = -1;
  [SerializeField] private Color outlineColor = Color.white;
  [SerializeField] private Vector2 outlineSize = new Vector2(5f, -5f);
  #endregion

  #region Private Fields
  private RadialChooseMenu radialChooseMenu;
  private RadialLayoutGroup radialLayoutGroup;
  private Dictionary<Transform, Outline> allOutlines = new Dictionary<Transform, Outline>();
  private Dictionary<Transform, Transform> allBackgrounds = new Dictionary<Transform, Transform>();
  private Dictionary<Transform, Outline> allBackgroundOutlines = new Dictionary<Transform, Outline>();
  #endregion

  #region MonoBehavior Callbacks
  private void Awake()
  {
    radialChooseMenu = GetComponent<RadialChooseMenu>();
    radialLayoutGroup = GetComponent<RadialLayoutGroup>();
  }
  private void OnEnable()
  {
    radialChooseMenu.OnSelectionChanged += OnSelectionChanged;
    radialChooseMenu.OnSelectionDeactivated += OnDeactivated;
    radialLayoutGroup.OnChildrenModified += OnChildrenModified;
  }
  private void OnDisable()
  {
    radialChooseMenu.OnSelectionChanged -= OnSelectionChanged;
    radialChooseMenu.OnSelectionDeactivated -= OnDeactivated;
    radialLayoutGroup.OnChildrenModified -= OnChildrenModified;
  }
  private void Start()
  {
    Init();
  }
#if UNITY_EDITOR
  //im using OnGUI on purpose, since other elements can modify the visuals of a radial menu and this needs to update consistently
  //however, while in runtime, this script uses events to call its methods and runs smoothly
  private void OnValidate()
  {
    if (workInInspector)
    {
      Init();
    }
  }
#endif

  #endregion

  #region public methods
  public void CallFeedbacksOnTransform(Transform lastSelectedObject, Transform previousSelectedObj)
  {
    lastSelectedObject.GetComponent<MMFeedbacks>().PlayFeedbacks();
  }
  #endregion

  #region Subscribed Methods
  void OnSelectionChanged(Transform oldSelection, Transform newSelection)
  {
    //Call events on the options themselves
    OnSelectionChangedEvent?.Invoke(newSelection, oldSelection);
    OnNewTransform?.Invoke(newSelection);

    if (allOutlines.Count > 0)
    {
      if (oldSelection)
        allOutlines[oldSelection].enabled = false;
      if (newSelection)
        allOutlines[newSelection].enabled = true;
    }
    if (allBackgroundOutlines.Count > 0)
    {
      if (oldSelection)
        allBackgroundOutlines[oldSelection].enabled = false;
      if (newSelection)
      {
        allBackgroundOutlines[newSelection].enabled = true;

      }
    }
    if (textIndicator && IncludeChangeTextOnSelection)
    {
      SetTextIndicator(newSelection);
    }
    if (pointerIndicator)
    {
      SetPointerIndicator(newSelection);
    }
    if (IncludeZoomOnSelection)
    {
      if (oldSelection)
      {
        float distance;
        if (newSelection)
        {
          distance = Vector2.Distance(newSelection.position, transform.position);
        }
        else
        {
          MyDebug.Log("impossible scenario");
          distance = ZoomPositionOffset;
        }
        var dir = (oldSelection.position - transform.position).normalized;
        oldSelection.position = transform.position + dir * distance;
        oldSelection.localScale = ZoomNormalSize;

      }
      if (newSelection)
      {
        var dir = (newSelection.position - transform.position).normalized;
        newSelection.position = transform.position + dir * ZoomPositionOffset;
        newSelection.localScale = ZoomSelectedSize;

      }
    }
  }
  void OnDeactivated()
  {
    if (pointerIndicator)
    {
      pointerIndicator.gameObject.SetActive(false);
    }
    // if (textIndicator)
    // {
    //   textIndicator.gameObject.SetActive(false);
    // }
  }
  void OnChildrenModified()
  {
    Init();
  }
  #endregion


  private void AddOutlines()
  {
    allOutlines.Clear();
    for (int i = 0; i < transform.childCount; i++)
    {
      Outline outline = transform.GetChild(i).GetComponent<Outline>();
      if (outline)
      {
        outline.effectColor = outlineColor;
        outline.effectDistance = outlineSize;
      }
      else
      {
        outline = transform.GetChild(i).gameObject.AddComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.effectDistance = outlineSize;
      }
      outline.enabled = false;
      allOutlines.Add(transform.GetChild(i), outline);
    }
  }
  private void AddBackground()
  {
    allBackgrounds.Clear();
    for (int i = 0; i < transform.childCount; i++)
    {
      var loc = transform.GetChild(i).GetComponentsInChildren<Image>();
      Image img = null;
      if (loc.Length > 1)
        img = loc[1];
      if (img)
      {
        img.sprite = backgroundSprite;
        img.color = backgroundColor;
        var canvas = img.GetComponent<Canvas>();
        if (canvas)
        {
          canvas.overrideSorting = true;
          canvas.sortingOrder = backgroundSortingOrder;
        }
        else
        {
          canvas = img.gameObject.AddComponent<Canvas>();
          canvas.overrideSorting = true;
          canvas.sortingOrder = backgroundSortingOrder;
        }
      }
      else
      {
        var obj = new GameObject("background");
        obj.transform.SetParent(transform.GetChild(i));
        img = obj.AddComponent<Image>();
        var canvas = obj.AddComponent<Canvas>();
        img.sprite = backgroundSprite;
        img.color = backgroundColor;
        canvas.overrideSorting = true;
        canvas.sortingOrder = backgroundSortingOrder;
      }

      var rt = img.GetComponent<RectTransform>();
      if (!rt)
      {
        rt = img.gameObject.AddComponent<RectTransform>();
      }
      var rtParent = (RectTransform)transform.GetChild(i);
      rt.anchoredPosition = Vector2.zero;
      rt.sizeDelta = backgroundSize + backgroundSize * (rtParent.sizeDelta / backgroundSize * backgroundScaleToParent);
      allBackgrounds.Add(rtParent.transform, img.transform);
    }

    if (backgroundOutline)
    {
      allBackgroundOutlines.Clear();
      foreach (KeyValuePair<Transform, Transform> bg in allBackgrounds)
      {
        var outline = bg.Value.GetComponent<Outline>();
        if (outline)
        {
          outline.effectColor = backgroundOutlineColor;
          outline.effectDistance = backgroundOutlineSize;
        }
        else
        {
          outline = bg.Value.gameObject.AddComponent<Outline>();
          outline.effectColor = backgroundOutlineColor;
          outline.effectDistance = backgroundOutlineSize;
        }
        outline.enabled = false;
        allBackgroundOutlines.Add(bg.Key, outline);
      }
    }

    if (backgroundToArc)
    {
      int index = 0;
      foreach (KeyValuePair<Transform, Transform> bg in allBackgrounds)
      {
        GenerateArcFill(bg.Value.GetComponent<Image>(), index);
        index++;
      }
    }

  }

  private void GenerateArcFill(Image img, int index)
  {
    float arcLength = 360 / allBackgrounds.Count;
    var dir = (transform.position - img.transform.position).normalized;
    img.transform.rotation = Quaternion.Euler(img.transform.eulerAngles.x, img.transform.eulerAngles.y, arcLength * index + arcLength / 2);
    img.fillOrigin = (int)Image.Origin360.Right;
    var rt = img.GetComponent<RectTransform>();
    var rtParent = img.transform.parent.GetComponent<RectTransform>();
    //rt.anchoredPosition = (rtParent.anchoredPosition + (Vector2)dir * backgroundOffset) * (1 / rt.sizeDelta.x);
    rt.anchoredPosition = dir * backgroundOffset;
    //img.GetComponent<RectTransform>().anchoredPosition=/* RectTransformUtility.PixelAdjustPoint(img.transform.parent.position, img.transform, img.GetComponent<Canvas>()) +*/ (backgroundOffset * dir) + dir * (backgroundOffset / allBackgrounds.Count);
    //img.transform.position = RectTransformUtility.PixelAdjustPoint(img.transform.parent.position, img.transform, img.GetComponent<Canvas>());
    img.type = Image.Type.Filled;
    img.fillMethod = Image.FillMethod.Radial360;
    img.fillAmount = Mathf.Clamp(arcLength / 360, backgroundMinMaxArc.x / 360, backgroundMinMaxArc.y / 360);
  }

  private void SetPointerIndicator(Transform newSelection)
  {
    if (newSelection != null)
    {
      RectTransform rt = (RectTransform)newSelection;
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
      pointerIndicator.position = _pos;
      var dir = (rt.position - pointerIndicator.position).normalized;
      float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      pointerIndicator.rotation = Quaternion.Euler(pointerIndicator.eulerAngles.x, pointerIndicator.eulerAngles.y, angle);
      pointerIndicator.gameObject.SetActive(true);
    }
  }
  private void SetTextIndicator(Transform newSelection)
  {
    string desc;
    if (newSelection)
    {
      var container = newSelection.GetComponent<SelectionMenuContainer>();
      if (container)
      {
        if (container.container != null)
        {
          if (container.container is ItemData @ItemData)
          {
            desc = @ItemData.Name;
          }
          else
          {
            desc = container.container.name;
          }
        }
        else
        {
          desc = newSelection.name;
        }
      }
      else
      {
        desc = newSelection.name;
      }
    }
    else
    {
      desc = textWhenMenuIsEmpty;
    }
    textIndicator.text = desc;
    textIndicator.gameObject.SetActive(true);
  }
  private void Init()
  {
    if (!radialChooseMenu) radialChooseMenu = GetComponent<RadialChooseMenu>();
    if (!radialLayoutGroup) radialLayoutGroup = GetComponent<RadialLayoutGroup>();

    if (IncludeOutlineSelection)
    {
      AddOutlines();
      if (radialChooseMenu.LastSelectedObject)
      {
        if (allOutlines.ContainsKey(radialChooseMenu.LastSelectedObject))
        {
          allOutlines[radialChooseMenu.LastSelectedObject].enabled = true;
        }
      }
    }
    if (IncludeRadialBackground)
    {
      AddBackground();
    }
    if (backgroundOutline)
    {
      if (radialChooseMenu.LastSelectedObject)
      {
        if (allBackgroundOutlines.ContainsKey(radialChooseMenu.LastSelectedObject))
        {
          allBackgroundOutlines[radialChooseMenu.LastSelectedObject].enabled = true;
        }
      }
    }
  }

}
