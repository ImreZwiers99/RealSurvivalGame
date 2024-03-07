using SaveIsEasy;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveIsEasyMenu : MonoBehaviour
{
    public LoadPart PrefabToLoad;
    public List<GameObject> PointsToLoadPrefabs;

    private int actualPage, totalPages;
    private List<LoadPart> allLoadParts = new List<LoadPart>();
    private List<SceneFile> allSceneFiles;

    private void Start()
    {
        RefreshSceneFiles();
        totalPages = Mathf.CeilToInt((float)allSceneFiles.Count / PointsToLoadPrefabs.Count);

        foreach (GameObject point in PointsToLoadPrefabs)
        {
            GameObject go = Instantiate(PrefabToLoad.gameObject, point.transform.position, Quaternion.identity);
            go.transform.SetParent(point.transform);
            allLoadParts.Add(go.GetComponent<LoadPart>());
        }

        UpdatePage();
    }
    private void Update()
    {
        UpdatePage();
        RefreshSceneFiles();
    }
    public void RefreshSceneFiles()
    {
        allSceneFiles = new List<SceneFile>(SaveIsEasyAPI.ListOfValidSaves());
    }

    public void UpdatePage()
    {
        List<SceneFile> copySceneFiles = new List<SceneFile>(allSceneFiles);
        copySceneFiles.RemoveRange(0, actualPage * PointsToLoadPrefabs.Count);

        foreach (LoadPart item in allLoadParts)
        {
            if (copySceneFiles.Count == 0)
            {
                item.gameObject.SetActive(false);
                continue;
            }

            item.gameObject.SetActive(true);

            SceneFile select = copySceneFiles[0];
            copySceneFiles.Remove(select);
            item.SetInfo(select);
        }
    }

    public void Next()
    {
        if ((actualPage + 1) * PointsToLoadPrefabs.Count >= allSceneFiles.Count)
            return;

        actualPage++;
        UpdatePage();
    }

    public void Back()
    {
        if (actualPage <= 0)
            return;

        actualPage--;
        UpdatePage();
    }

    public void OpenLevel(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }
}
