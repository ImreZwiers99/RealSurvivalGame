using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Countdown : MonoBehaviour
{
    public int countdownTime = 10;
    public GameObject Fire, Campfire, CampfireLight;

    void Start()
    {
        StartCoroutine(CountdownCoroutine());
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CampfireLight.SetActive(true);
            Fire.SetActive(true);
            Campfire.AddComponent<Rigidbody>();
            Rigidbody rb = Campfire.GetComponent<Rigidbody>();
            rb.freezeRotation = false;
        }
        if (countdownTime <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("WinScreen");
        }

    }

    IEnumerator CountdownCoroutine()
    {
        while (countdownTime > 0)
        {
            Debug.Log(countdownTime);
            yield return new WaitForSeconds(1);
            countdownTime--;
        }

        Debug.Log("Time's Up!");
    }
}
