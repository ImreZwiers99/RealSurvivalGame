using UnityEngine;
using System.IO;
using SaveIsEasy;
using TMPro;
using UnityEngine.SceneManagement;

public class ExampleControlUI : MonoBehaviour
{
    public TMP_InputField fileNameInput;
    private Scene selectedScene;

    private void Start()
    {
        selectedScene = SceneManager.GetSceneAt(0);
        UpdateUI();
    }

    private void UpdateUI()
    {
        fileNameInput.text = SaveIsEasyAPI.GetSceneFileNameByScene(selectedScene);
    }

    public void SelectScene(Scene scene)
    {
        selectedScene = scene;
        UpdateUI();
    }

    public void SaveGame()
    {
        SetFileName(); // Set the file name before saving
        SaveIsEasyAPI.SaveAll(selectedScene);
    }

    public void DeleteGame()
    {
        string fileName = fileNameInput.text + ".game";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("File deleted: " + fileName);
        }
        else
        {
            Debug.LogWarning("File not found: " + fileName);
        }
    }


    public void LoadGame()
    {
        SaveIsEasyAPI.LoadAll(selectedScene);
    }

    public void SetFileName()
    {
        string fileName = fileNameInput.text;
        if (!string.IsNullOrEmpty(fileName))
        {
            SaveIsEasyAPI.SetSceneFileNameByScene(fileName, selectedScene);
        }
    }
}
