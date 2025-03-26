using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SettingsManager : MonoBehaviour
{
    public UIDocument uiDocument;
    private Button informationButton, homeButton, settingsButton, closeButton;
    private VisualElement appLogo;
    private VisualElement informationPopup;
    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        informationButton = root.Q<Button>("informationButton");
        homeButton = root.Q<Button>("homeButton");
        settingsButton = root.Q<Button>("settingsButton");
        appLogo = root.Q<VisualElement>("appLogo");
        informationPopup = root.Q<VisualElement>("informationPopup");
        closeButton = root.Q<Button>("closeButton");

        informationButton.clicked += () => { informationPopup.style.display = DisplayStyle.Flex; };
        homeButton.clicked += () => {
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                SceneManager.LoadScene("MainMenu");
            }
        };
        settingsButton.clicked += () => {
            if (SceneManager.GetActiveScene().name != "Settings")
            {
                SceneManager.LoadScene("Settings");
            }
        };
        closeButton.clicked += () => informationPopup.style.display = DisplayStyle.None;
    }
}
