using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraOrbitToggle : MonoBehaviour
{
    public CinemachineCamera virtualCamera;
    private CinemachineInputAxisController inputController;
    private CinemachineOrbitalFollow orbitalFollow;

    // valores iniciais da órbita
    private float initialAxisValueX;
    private float initialAxisValueY;

    void Start()
    {

        inputController = virtualCamera.GetComponent<CinemachineInputAxisController>();
        orbitalFollow = virtualCamera.GetComponent<CinemachineOrbitalFollow>();

        inputController.enabled = false;

        // salva a posição inicial da órbita
        initialAxisValueX = orbitalFollow.HorizontalAxis.Value;
        initialAxisValueY = orbitalFollow.VerticalAxis.Value;       
    }

    void Update()
    {
        // desativa a rotação da câmera se o mouse estiver sobre a UI
        bool mouseOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        // ativa a rotação da câmera apenas se o botão direito ou do meio do mouse estiver pressionado e o mouse não estiver sobre a UI
        bool shouldOrbit = !mouseOverUI && (Input.GetMouseButton(1) || Input.GetMouseButton(2));
        inputController.enabled = shouldOrbit;

        // reseta a posição da câmera para a posição inicial se o botão de resetar for pressionado
        if (Input.GetKeyDown(InputManager.Instance.resetCamera))
        {
            orbitalFollow.HorizontalAxis.Value = initialAxisValueX;
            orbitalFollow.VerticalAxis.Value = initialAxisValueY;
        }
    }
}