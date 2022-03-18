using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using NaughtyAttributes;
#endif
using GuruLaghima;

namespace Workbench.ProjectDilemma
{
  public class DeathSequence : MonoBehaviour
  {

    // public DeathSequence_UI_Item ui_for_deathBook;
    public string labelText;
    public float duration;
    public Sprite backgroundSprite;
    [SerializeField] Camera deathCam;

    // // Start is called before the first frame update
    void Start()
    {
      Invoke("SelfDestruct", duration);
    }

    void SelfDestruct()
    {
      Destroy(this.gameObject);
    }

    // // Update is called once per frame
    // void Update()
    // {

    // }
  }
}