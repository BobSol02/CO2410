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
    private Button button1F, buttonG, buttonB;  // Floor selection buttons
    private VisualElement mainImage, appLogo, informationPopup, mapPopup, mapImage;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        // UI Elements
        mainImage = root.Q<VisualElement>("mainImage");
        appLogo = root.Q<VisualElement>("appLogo");
        informationPopup = root.Q<VisualElement>("informationPopup");
        mapPopup = root.Q<VisualElement>("mapPopup");
        mapImage = root.Q<VisualElement>("mapImage"); // Map image element

        // Buttons
        informationButton = root.Q<Button>("informationButton");
        mapButton = root.Q<Button>("mapButton");
        homeButton = root.Q<Button>("homeButton");
        informationCloseButton = root.Q<Button>("informationCloseButton");
        mapCloseButton = root.Q<Button>("mapCloseButton");
        startButton = root.Q<Button>("startButton");

        // Floor buttons
        button1F = root.Q<Button>("1F");
        buttonG = root.Q<Button>("G");
        buttonB = root.Q<Button>("B");

        // Home button logic
        homeButton.clicked += () => {
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                SceneManager.LoadScene("MainMenu");
            }
        };

        // Information popup logic
        informationButton.clicked += () =>
        {
            if (mapPopup.style.display == DisplayStyle.Flex) mapPopup.style.display = DisplayStyle.None;
            informationPopup.style.display = DisplayStyle.Flex;
        };
        informationCloseButton.clicked += () => { informationPopup.style.display = DisplayStyle.None; };

        // Map popup logic
        mapButton.clicked += () =>
        {
            if (informationPopup.style.display == DisplayStyle.Flex) informationPopup.style.display = DisplayStyle.None;
            mapPopup.style.display = DisplayStyle.Flex;
        };
        mapCloseButton.clicked += () => { mapPopup.style.display = DisplayStyle.None; };

        // Start exploring button
        startButton.clicked += () => LoadARScene();

        //Set initial main image background
        

        //Floor button logic
        button1F.clicked += () => UpdateMapImage("map_1F");
        buttonG.clicked += () => UpdateMapImage("map_G");
        buttonB.clicked += () => UpdateMapImage("map_B");
    }

    void UpdateMapImage(string imageName)
    {
        Texture2D texture = Resources.Load<Texture2D>($"images/{imageName}");
        if (texture != null)
        {
            mapImage.style.backgroundImage = new StyleBackground(texture);
        }
        else
        {
            Debug.LogError($"Image not found: {imageName}");
        }
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
