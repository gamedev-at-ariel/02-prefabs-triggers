using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/**
 * This component instantiates a given prefab at random time intervals and random bias from its object position.
 */
public class TimedSpawnerRandom: MonoBehaviour {
    [SerializeField] Mover prefabToSpawn;
    [SerializeField] Vector3 velocityOfSpawnedObject;
    [Tooltip("Minimum time between consecutive spawns, in seconds")] [SerializeField] float minTimeBetweenSpawns = 0.2f;
    [Tooltip("Maximum time between consecutive spawns, in seconds")] [SerializeField] float maxTimeBetweenSpawns = 1.0f;
    [Tooltip("Maximum distance in X between spawner and spawned objects, in meters")] [SerializeField] float maxXDistance = 1.5f;
    [SerializeField] Transform parentOfAllInstances;

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
                float timeBetweenSpawnsInSeconds = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
                await Awaitable.WaitForSecondsAsync(timeBetweenSpawnsInSeconds, cancellationToken);       // co-routines
                Vector3 positionOfSpawnedObject = new Vector3(
                    transform.position.x + Random.Range(-maxXDistance, +maxXDistance),
                    transform.position.y,
                    transform.position.z);
                GameObject newObject = Instantiate(prefabToSpawn.gameObject, positionOfSpawnedObject, Quaternion.identity);
                newObject.GetComponent<Mover>().SetVelocity(velocityOfSpawnedObject);
                newObject.transform.parent = parentOfAllInstances;
            }
        } catch (System.OperationCanceledException) {
            // The wait was cancelled in OnDisable - stop spawning.
        }
    }
}
