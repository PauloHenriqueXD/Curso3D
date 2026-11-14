using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeSlot : MonoBehaviour
{
    public CategoryType type;
    public TextMeshProUGUI categoryText;
    public TextMeshProUGUI amountText;
    public Sprite icon;
    public Image iconImage;
    public bool allowNone = false;
    private int currentIndex = 0;
    private int totalParts = 0;
    public int CurrentIndex => currentIndex;

    public void Start()
    {
        categoryText.text = type.ToString();
        currentIndex = allowNone ? -1 : 0;
        iconImage.sprite = icon;
        UpdateParts();
    }

    public void Next()
    {
        currentIndex++;
        int minIndex = allowNone ? -1 : 0;

        if (currentIndex >= totalParts)
        {
            currentIndex = minIndex;
        }
        UpdateParts();
    }

    public void Previous()
    {
        currentIndex--;
        int minIndex = allowNone ? -1 : 0;

        if (currentIndex < minIndex)
        {
            currentIndex = totalParts - 1;
        }
        UpdateParts();
    }

    public void ResetToDefault()
    {
        currentIndex = allowNone ? -1 : 0;
        UpdateParts();
    }

    public void Randomize()
    {
        int minIndex = allowNone ? -1 : 0;
        currentIndex = Random.Range(minIndex, totalParts);
        UpdateParts();
    }

    private void UpdateParts()
    {
        foreach (var t in Player.Instance.customizeBody.bodyParts)
        {
            if (t.category == type)
            {
                totalParts = t.parts.Count;

                // executar a lógica de ativar o objeto atual
                for (int i=0; i < t.parts.Count; i++)
                {
                    t.parts[i].SetActive(i == currentIndex);
                }
            }
        }

        UpdateAmountText();
    }

    private void UpdateAmountText()
    {
        if (currentIndex == -1)
            amountText.text = "None";
        else
            amountText.text = (currentIndex + 1).ToString() + " / " + totalParts.ToString();
    }
}
