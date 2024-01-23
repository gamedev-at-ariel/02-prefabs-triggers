using UnityEngine;

/*
 * This component increases a given "score" field whenever it is triggered.
 */

public class ScoreAdder : MonoBehaviour
{
    [Tooltip("Every object tagged with this tag will trigger adding score to the score field.")]
    [SerializeField] string triggeringTag;
    [SerializeField] NumberField scoreField;
    public int pointsToAdd = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == triggeringTag && scoreField != null)
        {


            if ((scoreField.GetNumber() % 50) == 0 && scoreField.GetNumber() != 0)
            {
                scoreField.AddNumber(pointsToAdd * 10);

            }
            else
            {
                scoreField.AddNumber(pointsToAdd);
            }
        }
    }

    public void SetScoreField(NumberField newTextField)
    {
        this.scoreField = newTextField;
    }

}
