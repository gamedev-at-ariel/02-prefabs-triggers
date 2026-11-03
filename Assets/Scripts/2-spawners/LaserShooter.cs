using UnityEngine;

/**
 * This component spawns the given laser-prefab whenever the player clicks a given key.
 * It also registers a score-change action to the laser onHit event.
 */
public class LaserShooter: ClickSpawner {
    [SerializeField]
    [Tooltip("How many points to add to the shooter, if the laser hits its target")]
    int pointsToAdd = 1;

    // A reference to the field that holds the score that has to be updated when the laser hits its target.
    private NumberField scoreField;  

    protected override void Start() {
        base.Start();
        scoreField = GetComponentInChildren<NumberField>();
        if (!scoreField)
            Debug.LogError($"No child of {gameObject.name} has a NumberField component!");
    }

    protected override GameObject spawnObject() {
        GameObject newObject = base.spawnObject();  // base = super
        if (newObject.TryGetComponent<DestroyOnTrigger2D>(out DestroyOnTrigger2D newObjectDestroyer)) {
            newObjectDestroyer.onHit += (() => { scoreField.AddNumber(pointsToAdd); });
        }
        return newObject;
    }
}
