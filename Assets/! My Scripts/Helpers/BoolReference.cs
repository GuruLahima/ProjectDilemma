using UnityEditor;
using UnityEngine;
namespace GuruLaghima
{

  [CreateAssetMenu(fileName = "BoolReference", menuName = "Workbench/ScriptableObjects/BoolReference", order = 1)]
  public class BoolReference : ScriptableObject
  {
    public BoolWrapper boolWrapper;
  }
}