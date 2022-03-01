using UnityEngine;
using System.IO;
using System.Collections.Generic;
using GuruLaghima;

namespace Workbench.ProjectDilemma
{
  [System.Serializable]
  public class FinalNote
  {
    [SerializeField] public string sender;
    [SerializeField] public string content;

    public FinalNote(string sender, string content)
    {
      this.sender = sender;
      this.content = content;
    }
    public FinalNote()
    {
      this.sender = "";
      this.content = "";
    }

  }
  public static class FinalNoteCardHandler
  {


    const string fileName = "/finalnotes.data";

    public static List<FinalNote> LoadAllNotes()
    {
      List<FinalNote> allNotes = new List<FinalNote>();
      FinalNote loadedNote = new FinalNote();
      string saveFile = Application.persistentDataPath + fileName;

      // Does the file exist?
      if (File.Exists(saveFile))
      {
        // Read the entire file and save its contents.
        string[] fileContents = File.ReadAllLines(saveFile);

        // Work with JSON
        foreach (string noteAsString in fileContents)
        {
          loadedNote = JsonUtility.FromJson<FinalNote>(noteAsString);
          allNotes.Add(loadedNote);
        }
      }

      return allNotes;
    }

    public static void SaveFinalNote(FinalNote note)
    {
      if (note == null)
      {
        MyDebug.Log("FinalNoteCardHandler", "No note passed");
        return;
      }

      string json = JsonUtility.ToJson(note);

      // Save the full path to the file.
      string saveFile = Application.persistentDataPath + fileName;
      string[] linesToSave = new string[1];
      linesToSave[0] = json;

      File.AppendAllLines(saveFile, linesToSave);

    }
  }
}