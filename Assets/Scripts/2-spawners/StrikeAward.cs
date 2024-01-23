using UnityEngine;
using System.Collections;

public class StrikeAward : MonoBehaviour
{
    private int strikeCount = 0;
    private int pointsPerStrike = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Handle enemy hit
            Destroy(other.gameObject);

            // Increment strike count
            strikeCount++;

            // Check for award trigger
            if (strikeCount == 10)
            {
                StartCoroutine(ActivateAward());
                strikeCount = 0; // Reset the counter after triggering the award
            }

            // Increment score based on the current points per strike
            int score = CalculateScore();
            // Update or display the score as needed
        }
    }

    int CalculateScore()
    {
        return pointsPerStrike;
    }

    IEnumerator ActivateAward()
    {
        // Activate award conditions
        int originalPointsPerStrike = pointsPerStrike; // Save the original points per strike
        pointsPerStrike = 5; // Set the new points per strike

        yield return new WaitForSeconds(5f); // Wait for 5 seconds

        // Deactivate award conditions
        pointsPerStrike = originalPointsPerStrike; // Restore the original points per strike
    }
}
