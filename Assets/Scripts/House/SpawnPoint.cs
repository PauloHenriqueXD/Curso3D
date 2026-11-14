using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string spawnId; // Unique identifier for the spawn point
    public bool assignCinemachineTarget; // Flag to determine if the Cinemachine camera should be assigned to the player
    public Transform spawnPoint; // Reference to the spawn point transform

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SpawnManager.spawnId == spawnId) // Check if the spawn ID matches the one in the SpawnManager
        {
            Player.Instance.GetComponent<CharacterController>().enabled = false; // Disable CharacterController to avoid unwanted movement during teleportation
            SpawnManager.spawnPosition = spawnPoint.position; // Set the spawn position in the SpawnManager
            Player.Instance.transform.position = SpawnManager.spawnPosition; // Teleport the player to the spawn position
            Player.Instance.GetComponent<CharacterController>().enabled = true; // Re-enable CharacterController after teleportation

            if (assignCinemachineTarget)
            {
                CinemachineController.Instance.AssignPlayer(); // Assign the player as the target for the Cinemachine camera if the flag is set
            }
        }
    }
}
