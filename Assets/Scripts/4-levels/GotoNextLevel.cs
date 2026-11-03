using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoNextLevel : MonoBehaviour {
    [SerializeField] string triggeringTag;
    [SerializeField] [Tooltip("Name of scene to move to when triggering the given tag")] string sceneName;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == triggeringTag) {
            // NOTE: the player's position in the new scene is handled by its ResetPositionOnSceneLoad component.
            SceneManager.LoadScene(sceneName);    // Input can either be a serial number or a name; here we use name.
        }
    }
}
