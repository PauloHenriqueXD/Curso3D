using UnityEngine;

public class NPCSellTrigger : MonoBehaviour
{
    public GameObject sellCanvas;
    public SellUI sellUI;
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
                OpenSellStore();
        }

        if (Input.GetKeyDown(InputManager.Instance.closeMenu))
            CloseSellStore();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = true;
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

            CloseSellStore();
        }
    }

    public void OpenSellStore()
    {
        sellCanvas.SetActive(true);
        Player.Instance.isPaused = true;
    }

    public void CloseSellStore()
    {
        if (!sellCanvas.activeSelf) return;
        sellCanvas.SetActive(false);
        Player.Instance.isPaused = false;
    }
}