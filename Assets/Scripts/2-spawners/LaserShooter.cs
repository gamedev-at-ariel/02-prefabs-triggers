using System.Collections;
using UnityEngine;

/**
 * This component spawns the given laser-prefab whenever the player clicks a given key.
 * It also updates the "scoreText" field of the new laser.
 */
public class LaserShooter : ClickSpawner
{
    [SerializeField] NumberField scoreField;
    [SerializeField] float cooldownTime = 1f;

    private bool canShoot = true;

    protected override GameObject spawnObject()
    {
        if (canShoot)
        {
            GameObject newObject = base.spawnObject();  // base = super

            // Modify the text field of the new object.
            ScoreAdder newObjectScoreAdder = newObject.GetComponent<ScoreAdder>();

            if (newObjectScoreAdder)
                newObjectScoreAdder.SetScoreField(scoreField);
            StartCoroutine(StartCooldown());

            return newObject;
        }
        return null;
    }
    IEnumerator StartCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
    }
}
