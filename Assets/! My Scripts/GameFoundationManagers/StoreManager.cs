using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
  #region public fields
  public GameObject storeCanvas;

  #endregion


  #region exposed fields
  [SerializeField] bool showAtStart;
  #endregion


  #region private fields

  #endregion


  #region MonoBehaviour callbacks

  // Start is called before the first frame update
  void Start()
  {
    if (showAtStart)
    {
      SetStoreVisibility(true);
    }
  }

  #endregion


  #region public methods

  public void ToggleStoreVisibility()
  {
    storeCanvas.SetActive(!storeCanvas.activeSelf);
  }
  public void SetStoreVisibility(bool visible)
  {
    storeCanvas.SetActive(visible);
  }

  #endregion

  #region private methods

  #endregion


}
