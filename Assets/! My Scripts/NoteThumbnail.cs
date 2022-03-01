using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NoteThumbnail : MonoBehaviour
{

  [SerializeField] TextMeshProUGUI senderNicknameField;

  [HideInInspector] public NoteModal fullNoteModal;
  [HideInInspector] public string senderNickname;
  [HideInInspector] public string messageContent;
  // Start is called before the first frame update
  void Start()
  {
    senderNicknameField.text = senderNickname;
  }

  public void ShowModal()
  {
    // show message modal
    fullNoteModal.senderNicknameField.text = senderNickname;
    fullNoteModal.messageContent.text = messageContent;
    fullNoteModal.gameObject.SetActive(true);
  }
}
