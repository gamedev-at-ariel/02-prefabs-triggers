using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class ShieldThePlayer : MonoBehaviour {
    [Tooltip("The number of seconds that the shield remains active")] [SerializeField] float duration;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Shield triggered by player");
            var destroyComponent = other.GetComponent<DestroyOnTrigger2D>();
            if (destroyComponent) {
                ShieldTemporarily(destroyComponent);
                Destroy(this.gameObject);  // Destroy the shield itself - to prevent double-use
            }
        } else {
            Debug.Log("Shield triggered by " + other.name);
        }
    }

    private async void ShieldTemporarily(DestroyOnTrigger2D destroyComponent) {
        destroyComponent.enabled = false;
        for (float t = duration; t > 0; t--) {
            Debug.Log("Shield: " + t + " seconds remaining!");
            await Awaitable.WaitForSecondsAsync(1);
        }
        Debug.Log("Shield gone!");
        destroyComponent.enabled = true;
    }
}
