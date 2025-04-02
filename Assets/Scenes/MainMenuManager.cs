using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class MainMenuManager : MonoBehaviour
{
    public UIDocument uiDocument;
    //buttons
    private Button informationButton, mapButton, homeButton, informationCloseButton, mapCloseButton, startButton;
    private Button button1F, buttonG, buttonB;
    //pop-ups, map image
    private VisualElement mainImage, appLogo, informationPopup, mapPopup, mapImage;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        //Initialise Ui elements
        mainImage = root.Q<VisualElement>("mainImage");
        appLogo = root.Q<VisualElement>("appLogo");
        informationPopup = root.Q<VisualElement>("informationPopup");
        mapPopup = root.Q<VisualElement>("mapPopup");
        mapImage = root.Q<VisualElement>("mapImage");

        // Initialise buttons
        informationButton = root.Q<Button>("informationButton");
        mapButton = root.Q<Button>("mapButton");
        homeButton = root.Q<Button>("homeButton");
        informationCloseButton = root.Q<Button>("informationCloseButton");
        mapCloseButton = root.Q<Button>("mapCloseButton");
        startButton = root.Q<Button>("startButton");

        // initialise map ppo up buttons
        button1F = root.Q<Button>("1F");
        buttonG = root.Q<Button>("G");
        buttonB = root.Q<Button>("B");

        //when pressing home button move to main menu
        homeButton.clicked += () => {
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                SceneManager.LoadScene("MainMenu");
            }
        };

        //when pressing information button display pop-up
        informationButton.clicked += () =>
        {
            if (mapPopup.style.display == DisplayStyle.Flex) mapPopup.style.display = DisplayStyle.None;
            informationPopup.style.display = DisplayStyle.Flex;
        };
        informationCloseButton.clicked += () => { informationPopup.style.display = DisplayStyle.None; };

        //when pressing map button display pop-up 
        mapButton.clicked += () =>
        {
            if (informationPopup.style.display == DisplayStyle.Flex) informationPopup.style.display = DisplayStyle.None;
            mapPopup.style.display = DisplayStyle.Flex;
        };
        mapCloseButton.clicked += () => { mapPopup.style.display = DisplayStyle.None; };

        // when pressing start exploring button move to AR screen with camera
        startButton.clicked += () => LoadARScene();

        //when a specific button for a floor is pressed in map display image of that floor
        button1F.clicked += () => UpdateMapImage("map_1F");
        buttonG.clicked += () => UpdateMapImage("map_G");
        buttonB.clicked += () => UpdateMapImage("map_B");
    }
    //function for hiding the previous image and updaing with the new selected one
    void UpdateMapImage(string imageName)
    {
        Texture2D texture = Resources.Load<Texture2D>($"images/{imageName}");
        if (texture != null)
        {
            var imageElement = mapImage as VisualElement;
            imageElement.style.backgroundImage = new StyleBackground(texture);
            imageElement.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;  //used to make the image stay within the pop-up         

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
