using UnityEngine;

public class GameManagement : MonoBehaviour
{

    public Transform initialSpawnPosition; // Reference to the initial spawn position transform
    public static GameManagement Instance;
    public Transform respawnPoint;
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D combatCursor;
    // hotspot = ponto de clique do cursor
    private readonly Vector2 hotspot = Vector2.zero;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if(Player.Instance != null)
        {
            Player.Instance.playerHUD.ActiveCanvas(true); // Activate the player's HUD canvas at the start of the game
            // o CharacterController sobrescreve transform.position se estiver ativo — desliga durante o teleporte
            var cc = Player.Instance.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            Player.Instance.transform.position = initialSpawnPosition.position; // Teleport the player to the initial spawn position at the start of the game
            if (cc != null) cc.enabled = true;
            Player.Instance.isPaused = false; // Unpause the game when the player is spawned
            CinemachineController.Instance.AssignPlayer(); // Assign the player as the target for the Cinemachine camera if the flag is set
        }

        // Set the default cursor at the start of the game
        SetDefaultCursor();
    }

    public void AddCoin()
    {
        Player.Instance.coinScore++; // Increment the coin score when a coin is collected
        Player.Instance.playerHUD.UpdateCoinText(); // Update the coin score display on the player's HUD
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }

    public void SetCombatCursor()
    {
        Cursor.SetCursor(combatCursor, hotspot, CursorMode.Auto);
    }
}
