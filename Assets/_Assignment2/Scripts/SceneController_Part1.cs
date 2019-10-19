
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SceneController_Part1 : MonoBehaviour
{
    // TO DO LIST:
    // (1) SOMETHING VERY BUGGY ABOUT THE INTERACTION OF CUBES & BANNER COUNT !!!!
    // THIS BREAKS THE UNDO BUTTON.
    // (2) NEED TO FIX THE TEXT ROTATION WRT TO THE CAMERA

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


    List<GameObject> _spawnList = new List<GameObject>();

    void Awake()
    {
        _mainCamera = _ARSessionOrigin.GetComponentInChildren <Camera>();

        m_RaycastManager = _ARSessionOrigin.GetComponent<ARRaycastManager>();

        _distVisInstance = Instantiate(_distanceVisualizerPrefab);
        _lineRendererScript = _distVisInstance.GetComponent<LineRenderSettings>();

    }

    void Update() //called after every frame 
    {

        TouchInteraction();
        UpdateTextRotation();
    }


    void TouchInteraction() //when there is touch interaction 
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    //Do nothing 
                }

                else if (m_RaycastManager.Raycast(touch.position, _s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = _s_Hits[0].pose;
                    AddCube(hitPose.position, hitPose.rotation);
                }
            }
        }
    }

    void AddCube(Vector3 hitPosePos, Quaternion hitPoseRot) // add a cube
    {
        _spawnedObject = Instantiate(m_cubePrefab, hitPosePos, hitPoseRot);
        _spawnList.Add(_spawnedObject);

        _lineRendererScript.AddCube(Time.time, hitPosePos);

        if (_onPlacedObject != null)
        {
            _onPlacedObject();
        }
    }

    public void Undo()
    {
        if (_spawnList.Count > 0) { // if there are no cubes, nothing happens
            GameObject removedCube = _spawnList[_spawnList.Count - 1];
            _spawnList.RemoveAt(_spawnList.Count - 1);

            Destroy(removedCube);
            _lineRendererScript.Undo();
        }

    }

    public void Reset()
    {
        while (_spawnList.Count > 0)
        {
            GameObject removedCube = _spawnList[0];
            _spawnList.RemoveAt(0);

            Destroy(removedCube);
        }

        Destroy(_distVisInstance); // destroy & reinstantiate the distance Visualizer 

        _distVisInstance = Instantiate(_distanceVisualizerPrefab);
        _lineRendererScript = _distVisInstance.GetComponent<LineRenderSettings>();
    }


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

}
