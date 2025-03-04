using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadARScene()
    {
        SceneManager.LoadScene("ARScene");
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
