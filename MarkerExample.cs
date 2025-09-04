using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.MagicLeap;
using TMPro;
using static UnityEngine.XR.MagicLeap.MLMarkerTracker;
using MarkerSettings = UnityEngine.XR.MagicLeap.MLMarkerTracker.TrackerSettings;

public class MarkerExample : MonoBehaviour
{
    private MagicLeapInputs mlInputs;
    private MagicLeapInputs.ControllerActions controllerActions;
    private Task _;
    private bool isScan = false;
    private ASCIIEncoding asciiEncoder = new ASCIIEncoding();
    private Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();

    public string StatusText
    {
        set
        {
            text.text = value;
        }
    }
    
    [SerializeField] private GameObject markerObject;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private GameObject axisObject;

    private MLMarkerTracker.TrackerSettings _markerSettings;

    // Represents the different tracker profiles used to optimize marker tracking in difference use cases.
    public MLMarkerTracker.Profile TrackerProfile = MLMarkerTracker.Profile.Custom;
    // A hint to the back-end the max frames per second hat should be analyzed.
    public MLMarkerTracker.FPSHint FPSHint;
    // A hint to the back-end the resolution that should be used.
    public MLMarkerTracker.ResolutionHint ResolutionHint;
    // A hint to the back-end for the cameras that should be used.
    public MLMarkerTracker.CameraHint CameraHint;
    // In order to improve performance, the detectors don't always run on the full
    // frame.Full frame analysis is however necessary to detect new markers that
    // weren't detected before. Use this option to control how often the detector may
    // detect new markers and its impact on tracking performance.
    public MLMarkerTracker.FullAnalysisIntervalHint FullAnalysisIntervalHint;
    // This option provides control over corner refinement methods and a way to
    // balance detection rate, speed and pose accuracy. Always available and
    // applicable for Aruco and April tags.
    public MLMarkerTracker.CornerRefineMethod CornerRefineMethod;
    // Run refinement step that uses marker edges to generate even more accurate
    // corners, but slow down tracking rate overall by consuming more compute.
    // Aruco/April tags only.
    public bool UseEdgeRefinement;

    //Enable scanning on start?
    private bool _enableMarkerScanning = true;

    // The marker types that are enabled for this scanner. Enable markers by
    // combining any number of <c> MarkerType </c> flags using '|' (bitwise 'or').
    public MLMarkerTracker.MarkerType MarkerTypes = MLMarkerTracker.MarkerType.Aruco_April | MLMarkerTracker.MarkerType.QR;
    // QR Code marker size to use (in meters).
    public float QRCodeSize = 0.18f;

    // Aruco dictionary to use.
    public MLMarkerTracker.ArucoDictionaryName ArucoDicitonary = MLMarkerTracker.ArucoDictionaryName.DICT_5X5_100;

    // Aruco marker size to use (in meters).
    public float ArucoMarkerSize = 0.18f;

    // Start is called before the first frame update
    void Start()
    {
        mlInputs = new MagicLeapInputs();
        mlInputs.Enable();
        controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);
        controllerActions.Menu.performed += HandleOnBumper;
        MLMarkerTracker.OnMLMarkerTrackerResultsFound += OnTrackerResultsFound;
        text.text = "Start();";
    }


    private void OnDestroy()
    {
        MLMarkerTracker.OnMLMarkerTrackerResultsFound -= OnTrackerResultsFound;
    }

    private void HandleOnBumper(InputAction.CallbackContext obj)
    {
        isScan = !isScan;
        if (isScan)
        {
            MarkerTypes = (int)MarkerTypes == -1 ? MLMarkerTracker.MarkerType.All : MarkerTypes;
            // If we are using a custom profile, create the profile before creating the tracker settings
            if (TrackerProfile == MLMarkerTracker.Profile.Custom)
            {
                MLMarkerTracker.TrackerSettings.CustomProfile customProfile = MLMarkerTracker.TrackerSettings.CustomProfile.Create(FPSHint, ResolutionHint, CameraHint, FullAnalysisIntervalHint, CornerRefineMethod, UseEdgeRefinement);
                _markerSettings = MLMarkerTracker.TrackerSettings.Create(
                    _enableMarkerScanning, MarkerTypes, QRCodeSize, ArucoDicitonary, ArucoMarkerSize, TrackerProfile, customProfile);
            }
            else
            {
                _markerSettings = MLMarkerTracker.TrackerSettings.Create(
                    _enableMarkerScanning, MarkerTypes, QRCodeSize, ArucoDicitonary, ArucoMarkerSize, TrackerProfile);
            }
            _ = MLMarkerTracker.SetSettingsAsync(_markerSettings);
            _ = MLMarkerTracker.StartScanningAsync();

            text.text = "HandleOnBumper\nisScan = true";
        }
        else
        {
            _ = MLMarkerTracker.StopScanningAsync();
            text.text = "HandleOnBumper\nisScan = false";
        }
    }

    private Vector3[] _positions;
    
    private void OnTrackerResultsFound(MLMarkerTracker.MarkerData data)
    {
        var id = "dummy";
        if (MLMarkerTracker.MarkerType.QR == data.Type)
        {
            id = asciiEncoder.GetString(data.BinaryData.Data, 0, data.BinaryData.Data.Length);
        }
        if (markers.ContainsKey(id))
        {
            markers[id].transform.position = data.Pose.position;
            markers[id].transform.rotation = data.Pose.rotation;
            markers[id].transform.localScale = Vector3.one * QRCodeSize;
        }
        else
        {
            GameObject marker = Instantiate(markerObject, data.Pose.position, data.Pose.rotation);
            marker.transform.localScale = Vector3.one * QRCodeSize;
            markers.Add(id, marker);
            marker.transform.GetComponentInChildren<TextMeshPro>().text = id;
        }
        if (markers.Count >= 3)
        {
            StartCoroutine(Draw());
        }
    }
     IEnumerator Draw()
    {
        // 最初の3つを取得
        _positions = markers
            .Take(3)
            .Select(pair =>
                pair.Value.transform.position
                - pair.Value.transform.right * QRCodeSize / 2
                + pair.Value.transform.up * QRCodeSize / 2
                )
            .ToArray();

        // QRコード1からQRコード2へのベクトル
        Vector3 qrCode1To2 = _positions[1] - _positions[0];
        // QRコード1からQRコード3へのベクトル
        Vector3 qrCode1To3 = _positions[2] - _positions[0];
        // 平面の法線ベクトルを計算
        Vector3 normal = Vector3.Cross(qrCode1To2, qrCode1To3).normalized;
        // 法線ベクトルを常に上向きに向ける回転を計算
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, -normal);
        axisObject.transform.rotation = rotation;

        // メッシュの位置をQRコードAの中心点に設定
        axisObject.transform.position = _positions[0];

        // メッシュのZ軸をQR1からQR2の方向に揃える回転
        Vector3 localForward = qrCode1To2.normalized;
        Quaternion zRotation = Quaternion.FromToRotation(axisObject.transform.forward, localForward);
        axisObject.transform.rotation = zRotation * axisObject.transform.rotation;

        // 法線ベクトルのY成分をチェックしてメッシュの向きを調整
        if (normal.y > 0)
        {
            // 配置が時計回りの場合
            axisObject.transform.Rotate(Vector3.forward, 180.0f);
        }
        RMMRDA.instance.SetPositionAndRotationLast(axisObject.transform.position, axisObject.transform.rotation);
        // オブジェクトをアクティブにする
        axisObject.SetActive(true);
        yield return null;
    }

}
