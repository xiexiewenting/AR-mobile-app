
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

public class SceneController_Part1 : MonoBehaviour
{
    /* necessary GameObjects */
    public GameObject _ARSessionOrigin;
    public GameObject _distanceVisualizerPrefab;

    ARRaycastManager m_RaycastManager;

    static List<ARRaycastHit> _s_Hits = new List<ARRaycastHit>();
    public static event Action _onPlacedObject;

    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_cubePrefab;
    public GameObject cubePrefab/// The prefab to instantiate on touch.
    {
        get { return m_cubePrefab; }
        set { m_cubePrefab = value; }
    }
    
    public GameObject _spawnedObject { get; private set; }

    private Camera _mainCamera;
    private GameObject _distVisInstance;
    private LineRenderSettings _lineRendererScript;

    /* tracking the created cubes */
    List<GameObject> _spawnList = new List<GameObject>();

    void Awake()
    {
        _mainCamera = _ARSessionOrigin.GetComponentInChildren <Camera>();
        m_RaycastManager = _ARSessionOrigin.GetComponent<ARRaycastManager>();

        _distVisInstance = Instantiate(_distanceVisualizerPrefab);
        _lineRendererScript = _distVisInstance.GetComponent<LineRenderSettings>();

    }

    void Update() 
    {
        TouchInteraction();
        UpdateTextRotation();
    }

    /* TouchInteraction():
     * checking if there is touch interaction and responding by
     * (1) doing nothing if a UI button has been touched
     * OR
     * (2) creating & adding a new cube 
     */
    void TouchInteraction() 
    {
        if (Input.touchCount <= 0) { return; }

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began) { return; }

        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) { return; }

        if (m_RaycastManager.Raycast(touch.position, _s_Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = _s_Hits[0].pose;
            AddCube(hitPose.position, hitPose.rotation);
        }
    }

    /* AddCube():
     * (1) instantiates a cube and adds it to the list
     * (2) tells DistanceVisualizer to AddCube()
     */
    void AddCube(Vector3 hitPosePos, Quaternion hitPoseRot) // add a cube
    {
        _spawnedObject = Instantiate(m_cubePrefab, hitPosePos, hitPoseRot);
        _spawnList.Add(_spawnedObject);

        _lineRendererScript.AddCube(hitPosePos);

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
        if (_spawnList.Count > 0) { // if there are no cubes, nothing happens
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

      public void LoadMainMenu() {
        SceneManager.LoadScene("LoadScreen");
    }

}
