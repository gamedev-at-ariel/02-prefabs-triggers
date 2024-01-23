using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * This component destroys its object whenever it triggers a 2D collider with the given tag.
 */
public class DestroyAndMoveToLoseScane : MonoBehaviour
{
    [Tooltip("Every object tagged with this tag will trigger the destruction of this object")]
    [SerializeField] string triggeringTag;
    [SerializeField] string sceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == triggeringTag && enabled)
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
