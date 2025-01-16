using UnityEngine;

public class TeleportManager : MonoBehaviour {
    [System.Serializable]
    public class TeleportPoint {
        public string name;
        public Vector3 position;
    }

    public TeleportPoint[] teleportPoints;

    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            TeleportTo("StartPoint");
        } else if (Input.GetKeyDown(KeyCode.F2)) {
            TeleportTo("Slopes");
        } else if (Input.GetKeyDown(KeyCode.F3)) {
            TeleportTo("Tunnels");
        }
    }

    private void TeleportTo(string pointName) {
        foreach (var point in teleportPoints) {
            if (point.name == pointName) {
                transform.position = point.position;
                return;
            }
        }

        Debug.LogWarning($"Teleport point '{pointName}' not found!");
    }
}
