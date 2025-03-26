using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ARSceneManager : MonoBehaviour
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
        closeButton.clicked += () => informationPopup.style.display = DisplayStyle.None;

        mainImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("test"));
    }
}