using System.Collections;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ExitAfterDelay(2f));
    }

    IEnumerator ExitAfterDelay(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);

        // Code to execute after the delay (e.g., show a message, save data, etc.)

        // Exit the game (works in standalone builds, not in the Unity Editor)
        Application.Quit();
    }
}