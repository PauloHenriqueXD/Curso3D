using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public string targetSpawnId;
    public string houseSceneName;
    private bool isPlayerNearby;

    private void Update()
    {
        if(isPlayerNearby && Input.GetKeyDown(InputManager.Instance.interact))
        {
            // Lógica para abrir a porta
            Debug.Log("Porta aberta!");
            LoadHouseScene();
            isPlayerNearby = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Pressione '" + InputManager.Instance.interact + "' para abrir a porta.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Você saiu do alcance da porta.");
        }
    }

    void LoadHouseScene()
    {
        // Lógica para carregar a cena da casa
        Debug.Log("Carregando cena da casa: " + houseSceneName);
        SpawnManager.spawnId = targetSpawnId; // Define o spawnId para a cena da casa
        SceneManager.LoadScene(houseSceneName);
    }
}
