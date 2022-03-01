using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NoteModal : MonoBehaviour
{

  [SerializeField] public TextMeshProUGUI senderNicknameField;
  [SerializeField] public TextMeshProUGUI messageContent;

  public void CloseModal()
  {
    this.gameObject.SetActive(false);
  }
}
