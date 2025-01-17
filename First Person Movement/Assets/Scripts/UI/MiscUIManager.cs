using UnityEngine;
using TMPro;

public class MiscUIManager : MonoBehaviour {
    public PlayerMovement playerMovement; 
    public TextMeshProUGUI displayText;   

    private void Update() {
        if (playerMovement != null && displayText != null) {
            float speed = playerMovement.GetComponent<Rigidbody>().linearVelocity.magnitude;

            string movementState = playerMovement.state.ToString();

            displayText.text = $"Speed: {speed:F2} m/s\nState: {movementState}";
        }
    }
}
