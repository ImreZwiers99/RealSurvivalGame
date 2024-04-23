using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathAnimScript : MonoBehaviour
{
    public GameObject redUI;
    public GameObject respawnUI;
    public Animator animator;

    public void RedUIActive()
    {
        redUI.SetActive(true);
    }

    public void RespawnUIActive()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        respawnUI.SetActive(true);
    }

    public void RespawnButton()
    {
        SceneManager.LoadScene("MainMap");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
