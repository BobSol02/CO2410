using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ARSceneManager : MonoBehaviour
{
    public UIDocument uiDocument;
    private Button informationButton, mapButton, homeButton, informationCloseButton, mapCloseButton;
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

        homeButton.clicked += () => { 
            if (SceneManager.GetActiveScene().name != "MainMenu") {
                SceneManager.LoadScene("MainMenu");
            } 
        };

        informationButton.clicked += () => 
        {
            if (mapPopup.style.display == DisplayStyle.Flex) mapPopup.style.display = DisplayStyle.None;
            informationPopup.style.display = DisplayStyle.Flex; 
        };
        informationCloseButton.clicked += () => informationPopup.style.display = DisplayStyle.None;

        mapButton.clicked += () => 
        {
            if (informationPopup.style.display == DisplayStyle.Flex) informationPopup.style.display = DisplayStyle.None;
            mapPopup.style.display = DisplayStyle.Flex; 
        };
        mapCloseButton.clicked += () => mapPopup.style.display = DisplayStyle.None;

        mainImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("test"));
    }
}