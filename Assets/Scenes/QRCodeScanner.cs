using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using System;
using System.Collections;
using Newtonsoft.Json;

[System.Serializable]
public class JsonContent
{
    public string Location_name;
    public string description;
    public string image;
}

public class QRCodeScanner : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCameraManager;
    [SerializeField] private GameObject infoPanel; // Panel to display the content
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private RawImage contentImage;
    [SerializeField] private Button close;
    [SerializeField] private float panelDistance = 1.5f; // Distance from camera to place panel

    private Texture2D cameraImageTexture;
    private IBarcodeReader barcodeReader = new BarcodeReader();
    private bool isScanning = false;
    private bool isDisplayingContent = false;

    private void OnEnable()
    {
        arCameraManager.frameReceived += OnCameraFrameReceived;
        close.onClick.AddListener(ClosePanel);
    }

    private void OnDisable()
    {
        arCameraManager.frameReceived -= OnCameraFrameReceived;
        close.onClick.AddListener(ClosePanel);
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (!isScanning && !isDisplayingContent)
        {
            StartCoroutine(ScanQRCode());
        }
    }

    private IEnumerator ScanQRCode()
    {
        isScanning = true;

        yield return new WaitForEndOfFrame();

        if (!arCameraManager.TryAcquireLatestCpuImage(out var image))
        {
            isScanning = false;
            yield break;
        }

        var request = image.ConvertAsync(new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),
            outputFormat = TextureFormat.R8,
            transformation = XRCpuImage.Transformation.None
        });

        while (!request.status.IsDone())
            yield return null;

        try
        {
            if (request.status != XRCpuImage.AsyncConversionStatus.Ready)
            {
                Debug.LogError("Failed to convert camera image");
                yield break;
            }

            var data = request.GetData<byte>();
            if (cameraImageTexture == null ||
                cameraImageTexture.width != request.conversionParams.outputDimensions.x ||
                cameraImageTexture.height != request.conversionParams.outputDimensions.y)
            {
                cameraImageTexture = new Texture2D(
                    request.conversionParams.outputDimensions.x,
                    request.conversionParams.outputDimensions.y,
                    request.conversionParams.outputFormat,
                    false);
            }

            cameraImageTexture.LoadRawTextureData(data);
            cameraImageTexture.Apply();

            var result = barcodeReader.Decode(
                cameraImageTexture.GetRawTextureData(),
                cameraImageTexture.width,
                cameraImageTexture.height,
                RGBLuminanceSource.BitmapFormat.Gray8);

            if (result != null)
            {
                Debug.Log($"QR code: {result.Text}");
                resultText.text = "QR code: " + result.Text;
                ProcessQRContent(result.Text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"QR code scan error: {ex.Message}");
        }
        finally
        {
            if (request.status == XRCpuImage.AsyncConversionStatus.Ready)
                request.Dispose();
            if (image.valid)
                image.Dispose();

            isScanning = false;
        }
    }

    private void ProcessQRContent(string file)
    {
        try
        {
            TextAsset json = Resources.Load<TextAsset>("json_files\\" + file);
            Debug.Log(json.text);
            
            JsonContent content = JsonConvert.DeserializeObject<JsonContent>(json.text);
            // Update UI elements
            locationText.text = content.Location_name;
            descriptionText.text = content.description;

            // Load and display the image
            //StartCoroutine(LoadImage(content.image));
            LoadImage(content.image);

            // Position the panel in front of the camera
            PositionPanelInFrontOfCamera();

            isDisplayingContent = true;
            infoPanel.SetActive(true);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to process QR content: {ex.Message}");
        }
    }

    //private IEnumerator LoadImage(string imagePath)
    private void LoadImage(string imagePath)
    {
        Texture2D texture = Resources.Load<Texture2D>("images/" + imagePath);
        if (texture != null)
        {
            contentImage.texture = texture;
        }
        else
        {
            Debug.LogError($"Failed to load local image: {imagePath}");
        }
    }

    private void PositionPanelInFrontOfCamera()
    {
        if (Camera.main != null)
        {
            // Position panel in front of the camera
            infoPanel.transform.position = Camera.main.transform.position + Camera.main.transform.forward * panelDistance;
            
            // Make panel face the camera
            infoPanel.transform.LookAt(Camera.main.transform);
            infoPanel.transform.Rotate(0, 180, 0); // Flip so text isn't mirrored
            infoPanel.transform.localScale = Vector3.one * 0.02f;
        }
    }

    // Call this method to close the panel (attach to a close button)
    public void ClosePanel()
    {
        infoPanel.SetActive(false);
        isDisplayingContent = false;
    }
}