using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuruLaghima;

namespace Workbench.ProjectDilemma
{

  public class FinalNotesManager : MonoBehaviour
  {
    [Range(0, 100)]
    [SerializeField] int smallPileMaxAmount;
    [SerializeField] GameObject smallPile;
    [Range(0, 100)]
    [SerializeField] int mediumPileMaxAmount;
    [SerializeField] GameObject mediumPile;
    [Range(0, 100)]
    [SerializeField] int largePileMaxAmount;
    [SerializeField] GameObject largePile;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject notesListElement;
    [SerializeField] GameObject noteThumbnailPrefab;
    [SerializeField] NoteModal fullNoteModal;
    // Start is called before the first frame update
    void Start()
    {
      // load received final notes
      List<FinalNote> allNotes = FinalNoteCardHandler.LoadAllNotes();
      foreach (FinalNote note in allNotes)
      {
        MyDebug.Log(note.sender, note.content);
        NoteThumbnail tmpThumb = Instantiate(noteThumbnailPrefab, notesListElement.transform, false).GetComponent<NoteThumbnail>();
        tmpThumb.senderNickname = note.sender;
        tmpThumb.messageContent = note.content;
        tmpThumb.fullNoteModal = fullNoteModal;
      }

      if (allNotes.Count > 0)
      {
        // choose size of pile of notes depending on count of notes 
        if (((float)allNotes.Count / (float)smallPileMaxAmount) <= 1f)
          smallPile.SetActive(true);
        else if (((float)allNotes.Count / (float)mediumPileMaxAmount) <= 1f)
          mediumPile.SetActive(true);
        else if (((float)allNotes.Count / (float)largePileMaxAmount) <= 1f)
          largePile.SetActive(true);
      }
    }

    public void ShowNotes()
    {
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.Confined;
      canvas.SetActive(true);
    }

    public void HideNotes()
    {
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
      canvas.SetActive(false);
    }

  }



}