using UnityEngine;

public class DrugOreOnTouch : MonoBehaviour
{
    [SerializeField] private float invertDuration = 5f; // how long W/S are flipped

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!(other.gameObject.name == "Player")) return;


        var pc = other.GetComponentInParent<PlayerControllerMain>();
        if (pc != null)
        {
            // Option A: temporary flip
            pc.InvertVerticalFor(invertDuration);

        }

        // CONFUSED SOUND HERE

        Destroy(gameObject);
    }
}
