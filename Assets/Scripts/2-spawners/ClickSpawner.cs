using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This component spawns the given object whenever the player clicks a given key.
 */
public class ClickSpawner: MonoBehaviour {
    [SerializeField] protected InputAction spawnAction = new InputAction(type: InputActionType.Button);
    [SerializeField] protected GameObject prefabToSpawn;
    [SerializeField] protected Vector3 velocityOfSpawnedObject;
    [SerializeField] float secondsBetweenShoot;



    private bool isWaiting = false;

    void OnEnable()  {
        spawnAction.Enable();
    }

    void OnDisable()  {
        spawnAction.Disable();
    }
    IEnumerator Wait()
    {
        isWaiting = true;
        yield return new WaitForSeconds(secondsBetweenShoot); // Wait for 1 second
        isWaiting = false;
    }

    protected virtual GameObject spawnObject()
    {

        if (!isWaiting)
        {
            Debug.Log("Spawning a new object");

            // Step 1: spawn the new object.
            Vector3 positionOfSpawnedObject = transform.position;  // span at the containing object position.
            Quaternion rotationOfSpawnedObject = Quaternion.identity;  // no rotation.
            GameObject newObject = Instantiate(prefabToSpawn, positionOfSpawnedObject, rotationOfSpawnedObject);

            // Step 2: modify the velocity of the new object.
            Mover newObjectMover = newObject.GetComponent<Mover>();
            if (newObjectMover)
            {
                newObjectMover.SetVelocity(velocityOfSpawnedObject);
            }
            StartCoroutine(Wait());

            return newObject;
        }
        return null;
    }


    private void Update() {
        if (spawnAction.WasPressedThisFrame()) {
            spawnObject();
        }
      

    }
}
