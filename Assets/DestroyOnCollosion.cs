using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnCollosion : MonoBehaviour
{
    [SerializeField] string sceneName;

    [Tooltip("Every object tagged with this tag will trigger the destruction of this object")]
    [SerializeField] string triggeringTag;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && enabled)
        {
            Destroy(this.gameObject);
            Destroy(other.gameObject);
        SceneManager.LoadScene(sceneName);
        Debug.Log("Game Over You lost");
        }
    }

    private void Update()
    {
        /* Just to show the enabled checkbox in Editor */
    }
}
