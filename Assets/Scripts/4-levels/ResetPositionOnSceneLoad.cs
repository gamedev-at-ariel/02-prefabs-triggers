using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * This component moves its object back to its starting position whenever a new scene is loaded.
 * Useful for an object that survives between scenes (e.g. with SingletonByName),
 * so that it starts each new level at the same place.
 */
public class ResetPositionOnSceneLoad: MonoBehaviour {
    private Vector3 startPosition;

    void Awake() {
        startPosition = transform.position;   // The position in the first scene in which this object appears.
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        transform.position = startPosition;
    }
}
