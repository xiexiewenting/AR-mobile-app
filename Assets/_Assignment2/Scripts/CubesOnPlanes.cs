
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CubesOnPlanes : MonoBehaviour
{
    public GameObject _ARSessionOrigin;
    public GameObject _distanceVisualizerPrefab;
    ARRaycastManager _m_RaycastManager;

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

    private GameObject _dvInstance;
    private LineRenderSettings _lrs;
    private LineRenderer _distanceVisualizer;


    List<GameObject> _spawnList = new List<GameObject>();

    void Awake()
    {
        _m_RaycastManager = _ARSessionOrigin.GetComponent<ARRaycastManager>();

        _dvInstance = Instantiate(_distanceVisualizerPrefab);
        _distanceVisualizer = _dvInstance.GetComponent<LineRenderer>();
        _lrs = _dvInstance.GetComponent<LineRenderSettings>();

    }

    void Update()
    {
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (_m_RaycastManager.Raycast(touch.position, _s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = _s_Hits[0].pose;

                    _spawnedObject = Instantiate(m_cubePrefab, hitPose.position, hitPose.rotation);
                    _spawnList.Add(_spawnedObject);

                    _lrs.AddCube(Time.time, hitPose.position);


                    if (_onPlacedObject != null)
                    {
                        _onPlacedObject();
                    }

                }
            }
        }


    }




}
