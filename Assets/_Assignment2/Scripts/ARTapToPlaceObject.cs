using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject _ARSessionOrigin;
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_gamePiecePrefab;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject gamePiecePrefab
    {
        get { return m_gamePiecePrefab; }
        set { m_gamePiecePrefab = value; }
    }
    public GameObject _gamePiece { get; private set; }
    ARRaycastManager m_RaycastManager;
    Vector3 _destination;
    float _movementSmooth = 1.0f;
    static List<ARRaycastHit> _s_Hits = new List<ARRaycastHit>();
    //Camera _mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        //_mainCamera = _ARSessionOrigin.GetComponentInChildren <Camera>();
        m_RaycastManager = _ARSessionOrigin.GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        TouchInteraction();
        MoveTowardsTap();
    }

    void TouchInteraction() {
        if (Input.touchCount <= 0) { return; }

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began) { return; }

        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) { return; }

        if (m_RaycastManager.Raycast(touch.position, _s_Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = _s_Hits[0].pose;
            _destination = hitPose.position;
            Debug.Log("We hit a plane "+Time.time );
            
            if (_gamePiece != null) {return; }

            Debug.Log("Going to instantiate "+Time.time);
            _gamePiece = Instantiate(m_gamePiecePrefab, hitPose.position, hitPose.rotation);
            Debug.Log("Instantiation happened "+Time.time);

        }
    }

    void MoveTowardsTap() {
        
        if (_gamePiece == null) { Debug.Log("the piece does not exist "+Time.time); return; }
        Transform gpTrans = _gamePiece.transform;
        float step = _movementSmooth * Time.deltaTime;
        Debug.Log("piece position "+gpTrans.position+" and destination is "+_destination);
        gpTrans.LookAt(_destination);
        gpTrans.position = Vector3.MoveTowards(gpTrans.position, _destination, step);

    }
}
