using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using NaughtyAttributes;
#endif
using UnityEngine;
using GuruLaghima;



namespace Workbench.ProjectDilemma

{
  [CreateAssetMenu(fileName = "ScenariosManager", menuName = "Workbench/ScriptableObjects/ScenariosManager", order = 1)]
  public class ScenariosManager : SingletonScriptableObject<ScenariosManager>
  {

    public List<string> approvedScenariosNames;



#if UNITY_EDITOR
    [ReadOnly]
    [SerializeField] string approvedScenesPath;

    [ReorderableList]
    [SerializeField] List<SceneAsset> playableScenarios;

    public List<SceneAsset> PlayableScenarios
    {
      get { return playableScenarios; }
      set
      {
        playableScenarios = value;
        approvedScenariosNames.Clear();
        foreach (SceneAsset scene in playableScenarios)
        {
          approvedScenariosNames.Add(scene.name);
        }
      }
    }

    [Button("1. Choose Folder With Approved Scenarios")]
    void ChooseFolderWithApprovedScenarios()
    {
      string path = EditorUtility.OpenFolderPanel("Choose approved scenes folder", "", "");
      if (path != "" && path != null)
      {
        approvedScenesPath = path.Substring(path.IndexOf("Assets"));
      }
      MyDebug.Log(approvedScenesPath);
    }


    [Button("2. Load approved scenarios")]
    void LoadApprovedScenarios()
    {

      if (AssetDatabase.IsValidFolder(approvedScenesPath))
      {

        // fetch the GUIDs of all the approved scenes
        string[] approvedFolders = new string[] { approvedScenesPath };
        string[] scenesGUIDs = AssetDatabase.FindAssets("t:scene", approvedFolders);
        MyDebug.Log("Found " + scenesGUIDs.Length.ToString() + " approved scenarios");

        if (scenesGUIDs.Length > 0)
        {
          playableScenarios.Clear();
          approvedScenariosNames.Clear();
        }

        foreach (string sceneGUID in scenesGUIDs)
        {
          SceneAsset t = (SceneAsset)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(sceneGUID));
          MyDebug.Log(t.name);
          playableScenarios.Add(t);
          approvedScenariosNames.Add(t.name);
        }

      }
    }

    [Button("3. Add Scenarios To Build")]
    void AddScenariosToBuild()
    {
      // Find valid Scene paths and make a list of EditorBuildSettingsScene
      List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
      foreach (var sceneAsset in playableScenarios)
      {
        string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
        if (!string.IsNullOrEmpty(scenePath) && !editorBuildSettingsScenes.Exists((obj) => { return obj.guid.ToString() == AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sceneAsset)); }))
          editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
      }

      // Set the Build Settings window Scene list
      EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
    }


    [MenuItem("CONTEXT/ScenariosManager/Reset (Custom)")]
    static void CustomReset(MenuCommand command)
    {
      ScenariosManager thisObj = (ScenariosManager)command.context;
      thisObj.Reset();
    }
#endif

  }
}