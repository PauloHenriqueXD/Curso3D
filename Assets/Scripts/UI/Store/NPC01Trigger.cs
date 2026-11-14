using UnityEngine;

public class NPC01Trigger : MonoBehaviour
{
    public GameObject storeCanvas;
    public StoreUI storeUI;
    private bool isPlayerNearby;
    public Transform alertAnchor;
    public GameObject triggerAlert;
    private GameObject _currentAlert;

    private void Update()
    {
        if (isPlayerNearby)
        {
            if (_currentAlert == null)
            {
                _currentAlert = Instantiate(triggerAlert);
                NPCInteractionPrompt prompt = _currentAlert.GetComponent<NPCInteractionPrompt>();
                prompt.target = alertAnchor != null ? alertAnchor : transform;
                prompt.yOffset = 0f;
            }

            if (Input.GetKeyDown(InputManager.Instance.interact))
                OpenStore();
        }

        if (Input.GetKeyDown(InputManager.Instance.closeMenu))
            CloseStore();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Pressione '" + InputManager.Instance.interact + "' para abrir a loja.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            if (_currentAlert != null)
            {
                Destroy(_currentAlert);
                _currentAlert = null;
            }

            CloseStore();
        }
    }

    public void OpenStore()
    {
        storeCanvas.SetActive(true);
        Debug.Log("Player.Instance: " + Player.Instance);
        Player.Instance.isPaused = true;
    }

    public void CloseStore()
    {
        if (!storeCanvas.activeSelf) return; // já tá fechado, ignora
        storeCanvas.SetActive(false);
        Player.Instance.isPaused = false;
        Debug.Log("Store fechada, isPaused: " + Player.Instance.isPaused);
    }
}