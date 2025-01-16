using UnityEngine;
using TMPro;

public class SpeedDisplay : MonoBehaviour {
    public PlayerMovement playerMovement; 
    public TextMeshProUGUI speedText;     

    private void Update() {
        if (playerMovement != null && speedText != null) {
            // Calculate the player's speed (magnitude of velocity)
            float speed = playerMovement.GetComponent<Rigidbody>().linearVelocity.magnitude;

            // Update the TMP text
            speedText.text = $"Speed: {speed:F2} m/s"; 
        }
    }
}
