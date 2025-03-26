using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class MainMenuManager : MonoBehaviour
{
    public UIDocument uiDocument;
    private Button informationButton, homeButton, settingsButton, startButton, closeButton;
    private VisualElement mainImage, appLogo;
    private VisualElement informationPopup;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        informationButton = root.Q<Button>("informationButton");
        homeButton = root.Q<Button>("homeButton");
        startButton = root.Q<Button>("startButton");
        mainImage = root.Q<VisualElement>("mainImage");
        appLogo = root.Q<VisualElement>("appLogo");
        informationPopup = root.Q<VisualElement>("informationPopup");
        closeButton = root.Q<Button>("closeButton");

        informationButton.clicked += () => { informationPopup.style.display = DisplayStyle.Flex; };
        homeButton.clicked += () => { 
            if (SceneManager.GetActiveScene().name != "MainMenu") {
                SceneManager.LoadScene("MainMenu");
            } 
        };
        startButton.clicked += () => LoadARScene();
        closeButton.clicked += () => informationPopup.style.display = DisplayStyle.None;

        mainImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("test"));
    }

    public void LoadARScene()
    {
#if UNITY_IOS
        if(!Application.HasUserAuthorization(UserAuthorization.WebCam)){
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }
        Debug.Log("Apple");
#endif
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.Log("Asking Permission");
            Permission.RequestUserPermission(Permission.Camera);
        }
        Debug.Log("Android");
#endif
        SceneManager.LoadScene("ARScene");
    }

}

