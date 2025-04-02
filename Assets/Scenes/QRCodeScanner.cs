using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing; //library for qr code scanning
using System;
using System.Collections;
using Newtonsoft.Json;

[System.Serializable]
//clas for defining the structure of the JSON data from QR code
public class JsonContent
{
    public string Location_name;
    public string description;
    public string image;
}

public class QRCodeScanner : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCameraManager;
    [SerializeField] private GameObject infoPanel; //panel to display content after QR code is scanned
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private RawImage contentImage;
    [SerializeField] private Button close;
    [SerializeField] private float panelDistance = 1.5f; //how far the panel is displayed from you in the room

    private Texture2D cameraImageTexture;
    private IBarcodeReader barcodeReader = new BarcodeReader();
    private bool isScanning = false; //boolean variable to prevent multiple simultaneous scans
    private bool isDisplayingContent = false; //boolean variable to track if we are displaying content

    private void OnEnable()
    {
        arCameraManager.frameReceived += OnCameraFrameReceived; //start listening for camera frames
        close.onClick.AddListener(ClosePanel);//for closing the panel
    }

    private void OnDisable()
    {
        arCameraManager.frameReceived -= OnCameraFrameReceived; //stop listening for camera frames
        close.onClick.AddListener(ClosePanel);
    }
    //when the camera captures a new frame, it scans only if we are not already displaying content
    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (!isScanning && !isDisplayingContent)
        {
            StartCoroutine(ScanQRCode());
        }
    }
    //scans camera image for qr codes
    private IEnumerator ScanQRCode()
    {
        isScanning = true;//prevent multiple scans

        yield return new WaitForEndOfFrame();

        if (!arCameraManager.TryAcquireLatestCpuImage(out var image))
        {
            isScanning = false;
            yield break;
        }
        //convert camera image to scanned content
        var request = image.ConvertAsync(new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),
            outputFormat = TextureFormat.R8,
            transformation = XRCpuImage.Transformation.None
        });
        //wait for conversion to finish
        while (!request.status.IsDone())
            yield return null;

        try
        {   //check for succsfull conversion
            if (request.status != XRCpuImage.AsyncConversionStatus.Ready)
            {
                Debug.LogError("Failed to convert camera image");
                yield break;
            }
            //get converted image data
            var data = request.GetData<byte>();
            //create or resize the texture if needed
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
            //load image data into the texture
            cameraImageTexture.LoadRawTextureData(data);
            cameraImageTexture.Apply();
            //decode QR code in the image
            var result = barcodeReader.Decode(
                cameraImageTexture.GetRawTextureData(),
                cameraImageTexture.width,
                cameraImageTexture.height,
                RGBLuminanceSource.BitmapFormat.Gray8);
            //if a QR code is found
            if (result != null)
            {
                Debug.Log($"QR code: {result.Text}");
                ProcessQRContent(result.Text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"QR code scan error: {ex.Message}");
        }
        finally
        {      //clear resources
            if (request.status == XRCpuImage.AsyncConversionStatus.Ready)
                request.Dispose();
            if (image.valid)
                image.Dispose();

            isScanning = false;
        }
    }
    //how we process content from a QR code
    private void ProcessQRContent(string file)
    {
        try
        {
            //load JSON file
            TextAsset json = Resources.Load<TextAsset>("json_files\\" + file);
            Debug.Log(json.text);
            //convert JSON text into JsonContent object
            JsonContent content = JsonConvert.DeserializeObject<JsonContent>(json.text);
            //update UI elements with location information
            locationText.text = content.Location_name;
            descriptionText.text = content.description;
            //display the associated image
            LoadImage(content.image);
            //position panel in front of user
            PositionPanelInFrontOfCamera();

            isDisplayingContent = true;
            infoPanel.SetActive(true);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to process QR content: {ex.Message}");
        }
    }
    //loads and displays image
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
    //positions panel in front of user in the camera
    private void PositionPanelInFrontOfCamera()
    {
        if (Camera.main != null)
        {
            //place it 1.5 m in front of user
            infoPanel.transform.position = Camera.main.transform.position + Camera.main.transform.forward * panelDistance;
            
            //rotate the panel to face the camera
            infoPanel.transform.LookAt(Camera.main.transform);
            infoPanel.transform.Rotate(0, 180, 0);
            infoPanel.transform.localScale = Vector3.one * 0.02f;
        }
    }
    //closing the panel
    public void ClosePanel()
    {
        infoPanel.SetActive(false);
        isDisplayingContent = false;
    }
}