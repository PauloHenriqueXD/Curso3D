using Unity.Cinemachine;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    public static CinemachineController Instance;
    private CinemachineCamera cam;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<CinemachineCamera>(); // 👈 busca o componente
    }

    public void AssignPlayer()
    {
        if (cam == null)
        {
            Debug.LogError("CinemachineCamera não encontrada no GameObject!");
            return;
        }

        if (Player.Instance == null)
        {
            Debug.LogError("Player.Instance é null!");
            return;
        }

        cam.Follow = Player.Instance.transform;
    }
}