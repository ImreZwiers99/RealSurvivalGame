using System.Collections;
using UnityEngine;

public class UiFix : MonoBehaviour
{
    public GameObject menu;
    public Player player;

    private void Start()
    {
        player = GetComponent<Player>();
        StartCoroutine(ResetTime()); 
    }

    private IEnumerator ResetTime()
    {
        player.Paused = true;
        menu.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        player.Paused = false;
        menu.SetActive(false);
    }
}
