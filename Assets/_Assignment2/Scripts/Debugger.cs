using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;

public class Debugger : MonoBehaviour
{

    [SerializeField]
    GameObject m_cubePrefab;



    public GameObject cubePrefab/// The prefab to instantiate on touch.
    {
        get { return m_cubePrefab; }
        set { m_cubePrefab = value; }
    }


    public GameObject spawnedObject { get; private set; }
    public GameObject _distanceVisualizerPrefab;
    //private GameObject _distanceVisualizer;

    List<Vector3> _cubePositions = new List<Vector3>();


    private GameObject _dvInstance;
    private LineRenderSettings _lrs;
    LineRenderer l;

    int totalClicks;

    // Start is called before the first frame update
    void Awake()
    {
        _dvInstance = Instantiate(_distanceVisualizerPrefab);
        l = _dvInstance.GetComponent<LineRenderer>();
        _lrs = _dvInstance.GetComponent<LineRenderSettings>();


    }

    // Update is called once per frame
    void Update()
    {
        WasMouseClicked();

    }

    void WasMouseClicked()
    {
        if (Input.GetMouseButtonUp(0))
        {
            totalClicks++;

            //Vector3 point = Input.mousePosition;
            Vector3 newPosition = calculatePosition();
            //spawnedObject = Instantiate(m_cubePrefab, newPosition, Quaternion.identity);
            _lrs.addCube(Time.time, newPosition);
            Debug.Log(totalClicks + ": " + newPosition);

            _cubePositions.Add(newPosition);



        }
    }

    Vector3 calculatePosition()
    {
        return (new Vector3(0.0f + totalClicks * 2.0f, 0.0f , 0.0f ));
    }


}
