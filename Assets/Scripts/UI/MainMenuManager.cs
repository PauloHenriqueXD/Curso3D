using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("customization");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
