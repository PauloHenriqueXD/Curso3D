using UnityEngine;
using UnityEngine.EventSystems;

public class CustomizeRotation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonRight;
    public float rotationSpeed;
    private bool isHolding;
    private float direction;

    public void Update()
    {
        if (isHolding)
        {
            if(buttonRight)
            {
                direction = -1f;
            }
            else
            {
                direction = 1f;
            }
            Player.Instance.transform.Rotate(0f, direction * rotationSpeed * Time.deltaTime, 0f);
        }
    }

    // É chamado ao clicar em um elemento de UI em que esteja com esse script
    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
    }

    // É chamado ao soltar o clique em um elemento de UI em que esteja com esse script
    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
    }
}
