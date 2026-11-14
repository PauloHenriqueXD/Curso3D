using UnityEngine;

public class LimitCameraMinimap : MonoBehaviour
{
    [SerializeField] private float heightOffset = 55f;

    private void LateUpdate()
    {
        if (Player.Instance != null)
        {
            Vector3 playerPos = Player.Instance.transform.position;
            transform.position = new Vector3(playerPos.x, playerPos.y + heightOffset, playerPos.z);
        }
    }
}