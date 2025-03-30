using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class MainMenuManager : MonoBehaviour
{
    public UIDocument uiDocument;
    private Button informationButton, mapButton, homeButton, informationCloseButton, mapCloseButton, startButton;
    private VisualElement mainImage, appLogo;
    private VisualElement informationPopup, mapPopup;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        mainImage = root.Q<VisualElement>("mainImage");
        appLogo = root.Q<VisualElement>("appLogo");
        informationPopup = root.Q<VisualElement>("informationPopup");
        mapPopup = root.Q<VisualElement>("mapPopup");
        informationButton = root.Q<Button>("informationButton");
        mapButton = root.Q<Button>("mapButton");
        homeButton = root.Q<Button>("homeButton");
        informationCloseButton = root.Q<Button>("informationCloseButton");
        mapCloseButton = root.Q<Button>("mapCloseButton");
        startButton = root.Q<Button>("startButton");

        homeButton.clicked += () => {
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                SceneManager.LoadScene("MainMenu");
            }
        };

        informationButton.clicked += () =>
        {
            if (mapPopup.style.display == DisplayStyle.Flex) mapPopup.style.display = DisplayStyle.None;
            informationPopup.style.display = DisplayStyle.Flex;
        };
        informationCloseButton.clicked += () => { informationPopup.style.display = DisplayStyle.None; };

        mapButton.clicked += () =>
        {
            if (informationPopup.style.display == DisplayStyle.Flex) informationPopup.style.display = DisplayStyle.None;
            mapPopup.style.display = DisplayStyle.Flex;
        };
        mapCloseButton.clicked += () => { mapPopup.style.display = DisplayStyle.None; };

        startButton.clicked += () => LoadARScene();
        
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

