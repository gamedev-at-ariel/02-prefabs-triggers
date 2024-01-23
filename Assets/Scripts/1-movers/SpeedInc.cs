using System.Collections;
using UnityEngine;

public class SpeedInc : MonoBehaviour
{
    [SerializeField] NumberField scoreField;
    [SerializeField] float speedIncreaseFactor = 1.5f;
    [SerializeField] float duration = 5f;

    private bool speedIncreased = false;
    private bool flag = false;
    private Coroutine speedIncreaseCoroutine;

    // Reference to the HorizontalMover script
    private HorizontalMover horizontalMover;
    void Start()
    {
        horizontalMover = GetComponent<HorizontalMover>();
        Debug.Log("HorizontalMover: " + horizontalMover);
        if (scoreField == null)
        {
            Debug.LogError("ScoreField is not assigned in the SpeedInc script.");
            scoreField = FindObjectOfType<NumberField>();
        }
    }

    void Update()
    {

        if (scoreField != null && scoreField.GetNumber() % 50 == 0 && scoreField.GetNumber() != 0 && !speedIncreased )
        {
            if (!flag) { StartCoroutine(IncreaseSpeedForDuration()); }
        }
        else if (speedIncreased)
        {
            ResetSpeed();
        }
        if(scoreField.GetNumber() % 50 != 0) { flag = false; }
    }


    IEnumerator IncreaseSpeedForDuration()
    {
       
        Debug.Log("hotizontal--------" + horizontalMover);
        // Increase the speed using the HorizontalMover script
        if (horizontalMover != null && !speedIncreased)
        {
            Debug.Log("hotizontal--------is not null" + horizontalMover);
            flag = true;
            horizontalMover.IncreaseSpeed(speedIncreaseFactor);
            Debug.Log("Speed Increased!");

            yield return new WaitForSeconds(duration);
            speedIncreased = true;
          
        }
      

    }

    void ResetSpeed()
    {
        if (horizontalMover != null && speedIncreased)
        {
            horizontalMover.ResetSpeed();
        }
        speedIncreased = false;
        Debug.Log("Speed Reset!");
    }

    public void SetScoreField(NumberField newScoreField)
    {
        scoreField = newScoreField;
    }
}
