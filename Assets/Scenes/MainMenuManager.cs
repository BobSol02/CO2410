using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif


public class MainMenuManager : MonoBehaviour
{
    public void LoadARScene()
    {
#if UNITY_IOS
        if(!Application.HasUserAuthorization(UserAuthorization.WebCam)){
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }
        Debug.Log("Apple");
#endif
#if UNITY_ANDROID
        if(!Permission.HasUserAuthorizedPermission(Permission.Camera)){
        Debug.Log("Asking Permission");
            Permission.RequestUserPermission(Permission.Camera);
        }
        Debug.Log("Android");
#endif
        SceneManager.LoadScene("ARScene");
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
