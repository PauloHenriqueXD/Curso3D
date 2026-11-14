using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomizeManager : MonoBehaviour
{
    private void Start()
    {
        Player.Instance.playerHUD.ActiveCanvas(false);
    }

    public void Confirm()
    {
        CustomizeSlot[] allSlots = FindObjectsByType<CustomizeSlot>(FindObjectsSortMode.None);
        foreach (var slot in allSlots)
        {
            if (slot.type == CategoryType.Gender)
            {
                Player.Instance.baseBodyIndex = slot.CurrentIndex;
                break;
            }
        }

        SceneManager.LoadScene("GamePlay");
    }
}

public enum CategoryType
{
    Gender,
    Beard,
    hair,
    body,
    hat,
    eye,
    mustache,
    mouth,
    Accessory
}
