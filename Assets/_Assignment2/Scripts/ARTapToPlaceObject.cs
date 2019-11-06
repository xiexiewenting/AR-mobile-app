using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject _ARSessionOrigin, _gamePieceTypeText, _instructionsText;

    [SerializeField]
    [Tooltip("Instantiates Mew on a plane at the touch location.")]
    GameObject m_MewGamePiece;

    [SerializeField]
    [Tooltip("Instantiates cereal bowl on a plane at the touch location.")]
    GameObject m_CerealGamePiece;

    [SerializeField]
    [Tooltip("Instantiates raccoon on a plane at the touch location.")]
    GameObject m_RaccoonGamePiece;

    [SerializeField]
    [Tooltip("Instantiates ramen on a plane at the touch location.")]
    GameObject m_RamenGamePiece;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject gamePiecePrefab_mew
    {
        get { return m_MewGamePiece; }
        set { m_MewGamePiece = value; }
    }
    public GameObject gamePiecePrefab_cereal
    {
        get { return m_CerealGamePiece; }
        set { m_CerealGamePiece = value; }
    }

    public GameObject gamePiecePrefab_raccoon
    {
        get { return m_RaccoonGamePiece; }
        set { m_RaccoonGamePiece = value; }
    }

        public GameObject gamePiecePrefab_ramen
    {
        get { return m_RamenGamePiece; }
        set { m_RamenGamePiece = value; }
    }
    public GameObject _gamePiece { get; private set; }
    GameObject _gpTemplate;
    string _gpTypeString;
    ARRaycastManager m_RaycastManager;
    Vector3 _destination;
    float _movementSmooth = 1.0f;
    static List<ARRaycastHit> _s_Hits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        m_RaycastManager = _ARSessionOrigin.GetComponent<ARRaycastManager>();
        _gpTemplate = m_CerealGamePiece;
        _gpTypeString = "Cereal Bowl";
        _gamePieceTypeText.GetComponent<Text>().text = _gpTypeString;
        Destroy(_instructionsText, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        TouchInteraction();
        MoveTowardsTap();
        _gamePieceTypeText.GetComponent<Text>().text = _gpTypeString;
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
            _gamePiece = Instantiate(_gpTemplate, hitPose.position, hitPose.rotation);
            Debug.Log("Instantiation happened "+Time.time);

        }
    }

    void MoveTowardsTap() {
        
        if (_gamePiece == null) { Debug.Log("the piece does not exist "+Time.time); return; }
        Transform gpTrans = _gamePiece.transform;
        float step = _movementSmooth * Time.deltaTime;
        //Debug.Log("piece position "+gpTrans.position+" and destination is "+_destination);
        gpTrans.LookAt(_destination);
        gpTrans.position = Vector3.MoveTowards(gpTrans.position, _destination, step);

    }

    public void TurnIntoCereal() {
        Debug.Log("change into cereal");
        if (_gpTemplate == m_CerealGamePiece) { return; }
        _gpTemplate = m_CerealGamePiece;
        _gpTypeString = "cereal bowl";
        if (_gamePiece == null) { return; }
        TransformGamePiece();
    }

    public void TurnIntoMew() {
        Debug.Log("change into mew");
        if (_gpTemplate == m_MewGamePiece) { return; }
        _gpTemplate = m_MewGamePiece;
        _gpTypeString = "Mew";
        if (_gamePiece == null) { return; }
        TransformGamePiece();
    }   
    public void TurnIntoRaccoon() {
        Debug.Log("change into raccoon");
        if (_gpTemplate == m_RaccoonGamePiece) { return; }
        _gpTemplate = m_RaccoonGamePiece;
        _gpTypeString = "raccoon";
        if (_gamePiece == null) { return; }
        TransformGamePiece();
    } 

    public void TurnIntoRamen() {
        Debug.Log("change into ramen");
        if (_gpTemplate == m_RamenGamePiece) { return; }
        _gpTemplate = m_RamenGamePiece;
        _gpTypeString = "ramen";
        if (_gamePiece == null) { return; }
        TransformGamePiece();
    } 

    void TransformGamePiece() {
        Vector3 currentPos = _gamePiece.transform.position;
        Quaternion currentRot = _gamePiece.transform.rotation;
        Debug.Log("Pre: Current position is "+currentPos+", and current rotation is "+currentRot);
        Destroy(_gamePiece);
        Debug.Log("Post: Current position is "+currentPos+", and current rotation is "+currentRot);
        _gamePiece = Instantiate(_gpTemplate, currentPos, currentRot);
    }
}
