using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Cinemachine;
using UnityEngine.UI;

[InitializeOnLoad]
static class HierarchyIcons
{

  // add your components and the associated icons here
  static Dictionary<Type, GUIContent> typeIcons = new Dictionary<Type, GUIContent>() {
         { typeof(Camera), EditorGUIUtility.IconContent( "Camera Icon" ) },
         { typeof(CinemachineVirtualCamera), EditorGUIUtility.IconContent( "Camera Icon" ) },
         { typeof(Rigidbody), EditorGUIUtility.IconContent( "d_Rigidbody Icon" ) },
         { typeof(Collider), EditorGUIUtility.IconContent( "BoxCollider Icon" ) },
         { typeof(AudioSource), EditorGUIUtility.IconContent( "AudioSource Icon" ) },
         { typeof(Animator), EditorGUIUtility.IconContent( "d_AnimationClip Icon" ) },
         { typeof(AudioListener), EditorGUIUtility.IconContent( "AudioListener Icon" ) },
         { typeof(Canvas), EditorGUIUtility.IconContent( "Canvas Icon" ) },
         { typeof(MonoBehaviour), EditorGUIUtility.IconContent( "cs Script Icon" ) },
         { typeof(Image), EditorGUIUtility.IconContent( "d_Image Icon" ) },
         { typeof(ILayoutElement), EditorGUIUtility.IconContent( "d_LayoutElement Icon" ) },
         { typeof(RectTransform), EditorGUIUtility.IconContent( "d_RectTransform Icon" ) },
         { typeof(Transform), EditorGUIUtility.IconContent( "d_Transform Icon" ) },
         // ...
     };

  // cached game object information
  static Dictionary<int, GUIContent> labeledObjects = new Dictionary<int, GUIContent>();
  static HashSet<int> unlabeledObjects = new HashSet<int>();
  static GameObject[] previousSelection = null; // used to update state on deselect

  // set up all callbacks needed
  static HierarchyIcons()
  {
    EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

    // callbacks for when we want to update the object GUI state:
    ObjectFactory.componentWasAdded += c => UpdateObject(c.gameObject.GetInstanceID());
    // there's no componentWasRemoved callback, but we can use selection as a proxy:
    Selection.selectionChanged += OnSelectionChanged;
  }

  static void OnHierarchyGUI(int id, Rect rect)
  {
    if (unlabeledObjects.Contains(id))
      return; // don't draw things with no component of interest

    if (ShouldDrawObject(id, out GUIContent icon))
    {
      // GUI code here
      rect.xMin = rect.xMax - 20; // right-align the icon
      GUI.Label(rect, icon);
    }
  }

  static bool ShouldDrawObject(int id, out GUIContent icon)
  {
    if (labeledObjects.TryGetValue(id, out icon))
      return true;
    // object is unsorted, add it and get icon, if applicable
    return SortObject(id, out icon);
  }

  static bool SortObject(int id, out GUIContent icon)
  {
    GameObject go = EditorUtility.InstanceIDToObject(id) as GameObject;
    if (go != null)
    {
      foreach (KeyValuePair<Type, GUIContent> asd in typeIcons)
      {
        if (go.GetComponent(asd.Key))
        {
          labeledObjects.Add(id, icon = asd.Value);
          return true;
        }
      }
    }

    unlabeledObjects.Add(id);
    icon = default;
    return false;
  }

  static void UpdateObject(int id)
  {
    unlabeledObjects.Remove(id);
    labeledObjects.Remove(id);
    SortObject(id, out _);
  }

  const int MAX_SELECTION_UPDATE_COUNT = 3; // how many objects we want to allow to get updated on select/deselect

  static void OnSelectionChanged()
  {
    TryUpdateObjects(previousSelection); // update on deselect
    TryUpdateObjects(previousSelection = Selection.gameObjects); // update on select
  }

  static void TryUpdateObjects(GameObject[] objects)
  {
    if (objects != null && objects.Length > 0 && objects.Length <= MAX_SELECTION_UPDATE_COUNT)
    { // max of three to prevent performance hitches when selecting many objects
      foreach (GameObject go in objects)
      {
        UpdateObject(go.GetInstanceID());
      }
    }
  }
}