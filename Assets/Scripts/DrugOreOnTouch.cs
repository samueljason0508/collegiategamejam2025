using UnityEngine;

public class DrugOreOnTouch : MonoBehaviour
{
    [SerializeField] private float invertDuration = 5f; // how long W/S are flipped

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!(other.gameObject.name == "Player")) return;


        var pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            // Option A: temporary flip
            pc.InvertVerticalFor(invertDuration);

        }

        Destroy(gameObject);
    }
}
