using TMPro; // Add this line to include TextMeshPro
using SaveIsEasy;
using UnityEngine;
using UnityEngine.UI;

public class LoadPart : MonoBehaviour
{

    public TMP_Text Title, Line3; // Changed from Text to TMP_Text
    SceneFile sceneFile;

    public void SetInfo(SceneFile sceneFile)
    {
        this.sceneFile = sceneFile;

        ResetTexts();

        Title.text = sceneFile.Name;

        if (sceneFile.HasSaveIsEasyStatistics)
        {
            Line3.text = sceneFile.StatisticsCreationDateAsDateTime.ToString("d/M/yyyy");
        }
    }

    private void ResetTexts()
    {
        Title.text = "";
        Line3.text = "";
    }

    public void OnClick()
    {
        sceneFile.LoadSceneAndGame(SceneFile.LoadSceneBy.ScenePath, true, false);
    }
}
