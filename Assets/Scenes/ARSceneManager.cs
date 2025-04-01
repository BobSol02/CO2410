using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ARSceneManager : MonoBehaviour
{
    public UIDocument uiDocument;
    private Button informationButton, mapButton, homeButton, informationCloseButton, mapCloseButton,button1F, buttonG, buttonB;
    private VisualElement mainImage, appLogo, mapImage;
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

        mapImage = root.Q<VisualElement>("mapImage"); // Map image element
        button1F = root.Q<Button>("1F");
        buttonG = root.Q<Button>("G");
        buttonB = root.Q<Button>("B");

        //Floor button logic
        button1F.clicked += () => UpdateMapImage("map_1F");
        buttonG.clicked += () => UpdateMapImage("map_G");
        buttonB.clicked += () => UpdateMapImage("map_B");


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
        informationCloseButton.clicked += () => { informationPopup.style.display = DisplayStyle.None; };

        mapButton.clicked += () => 
        {
            if (informationPopup.style.display == DisplayStyle.Flex) informationPopup.style.display = DisplayStyle.None;
            mapPopup.style.display = DisplayStyle.Flex; 
        };
        mapCloseButton.clicked += () => { mapPopup.style.display = DisplayStyle.None; } ;

        SetRaycastIgnoring(root);

    }




    private void SetRaycastIgnoring(VisualElement element)
    {
        // Check if the element is a button
        if (!(element is Button))
        {
            element.pickingMode = PickingMode.Ignore;
        }

        // Recursively apply to all children
        foreach (var child in element.Children())
        {
            SetRaycastIgnoring(child);
        }
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
}