
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

public class SceneController_Part1 : MonoBehaviour
{
    /* necessary GameObjects */
    public GameObject _ARSessionOrigin, _distanceVisualizerPrefab, _cubeText;
    
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_cubePrefab;
    public GameObject cubePrefab/// The prefab to instantiate on touch.
    {
        get { return m_cubePrefab; }
        set { m_cubePrefab = value; }
    }
    
    public GameObject _spawnedObject { get; private set; }
    private ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> _s_Hits = new List<ARRaycastHit>();
    public static event Action _onPlacedObject;
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
        _cubeText.GetComponent<Text>().text = "Cubes: 0";
    }

    void Update() 
    {
        TouchInteraction();
        UpdateTextRotation();
        _cubeText.GetComponent<Text>().text = "Cubes: "+_spawnList.Count;
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

    /* BUTTON METHOD 
     * AddCube(): 
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

    /* BUTTON METHOD
     * Undo():
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

    /* BUTTON METHOD
     * Reset():
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

    /* BUTTON METHOD
     * LoadMainMenu():
     * loads the main menu scene
     */
    public void LoadMainMenu() {
        SceneManager.LoadScene("LoadScreen");
    }

}
