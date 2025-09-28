using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointsAdderOnTouch : MonoBehaviour
{
    public int pointsToAdd = 10;
    public string prefix;
    public TextMeshProUGUI scoreText; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!(other.gameObject.name == "Player")) return;

        if (scoreText != null)
        {
            string currentText = scoreText.text;
            int currentScore = 0;
            
            // Extract number after the prefix
            if (currentText.StartsWith(prefix))
            {
                string numberPart = currentText.Substring(prefix.Length);
                int.TryParse(numberPart, out currentScore);
            }
            
            // Add points and update text
            int newScore = currentScore + pointsToAdd;
            scoreText.text = prefix + newScore;
        }

        // COIN COLLECTION SOUND HERE

        Destroy(gameObject);
    }
}