using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    Player player;
    private AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
       audio = GetComponent<AudioSource>();
    }

   public void HoverAudio()
    {
        audio.Play();
    }

    public void Resume()
    {
        player.Paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }
    public void Options()
    {
        Debug.Log("No Options ;)");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
