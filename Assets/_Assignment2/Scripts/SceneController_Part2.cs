using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

public class SceneController_Part2 : MonoBehaviour
{
    /* necessary GameObjects */
    public GameObject _ARSessionOrigin, _distanceVisualizerPrefab, _bezierVisualizerPrefab, _shadowPrefab, _cubeText;
    public Slider _distanceSlider;

    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_cubePrefab;
    public GameObject cubePrefab/// The prefab to instantiate on touch.
    {
        get { return m_cubePrefab; }
        set { m_cubePrefab = value; }
    }

    public GameObject _spawnedObject { get; private set; }
    public GameObject _bezierCube { get; private set; } // the  cube that is being pulled around 
    ARRaycastManager m_RaycastManager;

    static List<ARRaycastHit> _s_Hits = new List<ARRaycastHit>();
    public static event Action _onPlacedObject;
    private Camera _mainCamera;
    private GameObject _distVisInstance, _bezierInstance, _shadow;
    private BezierLineSetting _bezierScript;
    private LineRenderSettings _lineRendererScript;

    

    float _distanceFromCamera = 1.0f;
    Vector3 _cubeVelocity = new Vector3(0.1f, 0.1f, 0.1f);
    Vector3 _camVector;
    float _smoothTime = 0.4F;

    /* tracking the created cubes */
    List<GameObject> _spawnList = new List<GameObject>();
    // Start is called before the first frame update

    void Start()
    {
        _mainCamera = _ARSessionOrigin.GetComponentInChildren<Camera>();
        m_RaycastManager = _ARSessionOrigin.GetComponent<ARRaycastManager>();

        _distVisInstance = Instantiate(_distanceVisualizerPrefab);
        _lineRendererScript = _distVisInstance.GetComponent<LineRenderSettings>();

        _bezierInstance = Instantiate(_bezierVisualizerPrefab);
        _bezierScript = _bezierInstance.GetComponent<BezierLineSetting>();

        _bezierCube = Instantiate(m_cubePrefab, Vector3.zero, Quaternion.identity);
        _shadow = Instantiate(_shadowPrefab);
        _cubeText.GetComponent<Text>().text = "Cubes: 0";
    }

    // Update is called once per frame
    void Update()
    {
        BezierCubeUpdate();
        BezierLineUpdate();
        UpdateTextRotation();
        DetectShadow();
        _cubeText.GetComponent<Text>().text = "Cubes: "+_spawnList.Count;
    }

    /* BezierLineUpdate():
     * updates the line as a bezier quadratic curve
     */
    void BezierLineUpdate()
    {
        //Vector3 camDown = -_mainCamera.transform.up;
        _camVector = (_mainCamera.transform.forward - _mainCamera.transform.up)/2.0f;
        Vector3 cubePosition = _bezierCube.transform.position;
        _bezierScript.BezierLineUpdate(_camVector, _cubeVelocity, cubePosition);

    }

    /* BezierCubeUpdate():
     * updates the cube position in relation to camera
     */
    void BezierCubeUpdate()
    {
        Vector3 currentPosition = _bezierCube.transform.position;
        Vector3 camPosition = _mainCamera.transform.position;
        Vector3 offset = _mainCamera.transform.forward * _distanceFromCamera;// + _mainCamera.transform.up * ;
        Vector3 resultingPosition = camPosition + offset;
  
        _bezierCube.transform.position = Vector3.SmoothDamp(currentPosition, 
            resultingPosition, ref _cubeVelocity, _smoothTime);
        //_bezierCube.transform.LookAt(_bezierScript.CalculateP1(_camVector, _cubeVelocity, currentPosition));
        _bezierCube.transform.LookAt(_bezierCube.transform.position);
    }

    /* DetectShadow():
     * detects whether beziercube is above a plane, if so make shadow appear 
     */
    void DetectShadow() {
        Ray shadowRay = new Ray(_bezierCube.transform.position, _bezierCube.transform.up * -1.0f);
        if (m_RaycastManager.Raycast(shadowRay, _s_Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose shadowPose = _s_Hits[0].pose;
            _shadow.SetActive(true);
            _shadow.transform.position = shadowPose.position;
            return;
        }
        _shadow.SetActive(false);
    }


    /* UpdateTextRotation():
     * updates all the rotations of the DistText banners according to the current camera angle 
    */
    void UpdateTextRotation()
    {
        int textTotal = _lineRendererScript._distTextArray.Count;
        for (int i = 0; i < textTotal; i++)
        {
            Transform distTextTransform = _lineRendererScript._distTextArray[i].transform;
            Quaternion camRot = _mainCamera.transform.rotation;

            distTextTransform.LookAt(distTextTransform.position + camRot * Vector3.forward, camRot * Vector3.up);
        }
    }

    public void PlaceCube() // add a cube
    {
        Debug.Log("place cube was called, there are " + _spawnList.Count+ " cubes in the scene");
        _spawnedObject = Instantiate(m_cubePrefab, _bezierCube.transform.position, _bezierCube.transform.rotation);
        _spawnList.Add(_spawnedObject);

        _lineRendererScript.AddCube(_bezierCube.transform.position);

        if (_onPlacedObject != null)
        {
            _onPlacedObject();
        }
    }


    /* Undo():
     * if there are still cubes in the scene 
     * (1) removes the last-created cube from the list and destroys it
     * (2) tells DistanceVisualizer to Undo()
     */
    public void Undo()
    {
        Debug.Log("Undo was called");
        if (_spawnList.Count > 0)
        { // if there are no cubes, nothing happens
            GameObject removedCube = _spawnList[_spawnList.Count - 1];
            _spawnList.RemoveAt(_spawnList.Count - 1);

            Destroy(removedCube);
            _lineRendererScript.Undo();
        }
    }


    /* Reset():
     * (1) removes and destroys all cubes in the scene
     * (2) destroys DistanceVisualizer
     * (3) reinstantiates DistanceVisualizer 
     */
    public void Reset()
    {
        Debug.Log("Reset was called");
        while (_spawnList.Count > 0)
        {
            GameObject removedCube = _spawnList[0];
            _spawnList.RemoveAt(0);

            Destroy(removedCube);
        }
        _lineRendererScript.DestroyDistText();
        Destroy(_distVisInstance); // destroy & reinstantiate the distance Visualizer 

        _distVisInstance = Instantiate(_distanceVisualizerPrefab);
        _lineRendererScript = _distVisInstance.GetComponent<LineRenderSettings>();
    }


    public void ChangeDistance() {
        _distanceFromCamera = _distanceSlider.value;
        Debug.Log("Distance is changed and now "+_distanceFromCamera);
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene("LoadScreen");
    }
}
