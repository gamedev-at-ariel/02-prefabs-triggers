using System;
using System.Collections;
using UnityEngine;

/**
 * This component spawns the given laser-prefab whenever the player clicks a given key.
 * It also updates the "scoreText" field of the new laser.
 */
public class LaserShooter: ClickSpawner {
    [SerializeField] NumberField scoreField;

 

    protected override GameObject spawnObject() {


        GameObject newObject = base.spawnObject();  // base = super

        // Modify the text field of the new object.
        if(newObject != null )
        {
            ScoreAdder newObjectScoreAdder = newObject.GetComponent<ScoreAdder>();
            if (newObjectScoreAdder)
                newObjectScoreAdder.SetScoreField(scoreField);


            return newObject;
        }
        return null;
      
    }

}
