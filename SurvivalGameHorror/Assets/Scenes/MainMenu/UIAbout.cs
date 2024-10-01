using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAbout : MonoBehaviour
{
    public GameObject Start, Options, Quit, Title, aboutText, aboutButtonBack;

    public void AboutPressed()
    {
        Start.SetActive(false);
        Options.SetActive(false);
        Quit.SetActive(false);
        Title.SetActive(false);
        aboutText.SetActive(true);
        aboutButtonBack.SetActive(true);
    }

    public void AboutBackPressed()
    {
        Start.SetActive(true);
        Options.SetActive(true);
        Quit.SetActive(true);
        Title.SetActive(true);
        aboutText.SetActive(false);
        aboutButtonBack.SetActive(false);
    }
}
