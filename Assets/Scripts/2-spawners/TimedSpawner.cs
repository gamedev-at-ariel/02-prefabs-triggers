using System.Threading;
using UnityEngine;

/**
 * This component spawns the given object at fixed time-intervals at its object position.
 */
public class TimedSpawner: MonoBehaviour {
    [SerializeField] Mover prefabToSpawn;
    [SerializeField] Vector3 velocityOfSpawnedObject;
    [SerializeField] float secondsBetweenSpawns = 1f;

    CancellationTokenSource cancellationTokenSource;

    void OnEnable() {
        cancellationTokenSource = new CancellationTokenSource();
        SpawnRoutine(cancellationTokenSource.Token);
    }
    
    void OnDisable() {
        // Called when the component is disabled, the GameObject is deactivated, or it is destroyed (e.g. on scene change).
        cancellationTokenSource.Cancel();
    }

    async void SpawnRoutine(CancellationToken cancellationToken) {
        try {
            while (true) {
                GameObject newObject = Instantiate(prefabToSpawn.gameObject, transform.position, Quaternion.identity);
                newObject.GetComponent<Mover>()?.SetVelocity(velocityOfSpawnedObject);
                await Awaitable.WaitForSecondsAsync(secondsBetweenSpawns, cancellationToken);
                // See here for more options: https://docs.unity3d.com/6000.0/Documentation/Manual/async-awaitable-introduction.html
            }
        } catch (System.OperationCanceledException) {
            // The wait was cancelled in OnDisable - stop spawning.
        }
    }
}
