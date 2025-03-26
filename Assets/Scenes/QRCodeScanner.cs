using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using ZXing;
using System.Collections;
using System;
using PimDeWitte.UnityMainThreadDispatcher;

public class QRCodeScanner : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCameraManager;
    [SerializeField] private TextMeshProUGUI resultText;

    private Texture2D cameraImageTexture;
    private IBarcodeReader barcodeReader = new BarcodeReader();
    private bool isScanning = false;

    private void OnEnable()
    {

        arCameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        arCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (!isScanning)
        {
            StartCoroutine(ScanQRCode());
        }
    }

    private IEnumerator ScanQRCode()
    {
        isScanning = true;

        // Wait for the end of frame to ensure all rendering is complete
        yield return new WaitForEndOfFrame();

        // Stage 1: Try to acquire the image (no yielding here)
        if (!arCameraManager.TryAcquireLatestCpuImage(out var image))
        {
            isScanning = false;
            yield break;
        }

        // Stage 2: Convert the image (async operation)
        var request = image.ConvertAsync(new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),
            outputFormat = TextureFormat.R8,
            transformation = XRCpuImage.Transformation.None
        });

        // Wait for conversion to complete (outside try-catch)
        while (!request.status.IsDone())
            yield return null;

        // Stage 3: Process the result (in try-catch)
        try
        {
            if (request.status != XRCpuImage.AsyncConversionStatus.Ready)
            {
                Debug.LogError("Failed to convert camera image");
                yield break;
            }

            // Create or resize texture if needed
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

            // Decode the QR code
            var result = barcodeReader.Decode(
                cameraImageTexture.GetRawTextureData(),
                cameraImageTexture.width,
                cameraImageTexture.height,
                RGBLuminanceSource.BitmapFormat.Gray8);

            if (result != null)
            {
                // UnityMainThreadDispatcher.Instance().Enqueue(() =>
                //{
                //    resultText.text = result.Text;
                //});
                Debug.Log($"QR code: {result.Text}");
                resultText.text = "QR code: " + result.Text;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"QR code scan error: {ex.Message}");
        }
        finally
        {
            // Clean up resources
            if (request.status == XRCpuImage.AsyncConversionStatus.Ready)
                request.Dispose();
            if (image.valid)
                image.Dispose();

            isScanning = false;
        }
    }
}