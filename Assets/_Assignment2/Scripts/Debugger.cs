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


    public GameObject _spawnedObject { get; private set; }
    public GameObject _distanceVisualizerPrefab;


    List<Vector3> _cubePositions = new List<Vector3>();
    List<GameObject> _spawnList = new List<GameObject>();


    private GameObject _dvInstance;
    private LineRenderSettings _lrs;
    private LineRenderer _l;


    int totalClicks;

    // Start is called before the first frame update
    void Awake()
    {
        _dvInstance = Instantiate(_distanceVisualizerPrefab);
        _lrs = _dvInstance.GetComponent<LineRenderSettings>();
        _l = _dvInstance.GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        _l.startWidth = 0.1f;
        WasMouseClicked();
        IsUndoNeeded();

    }

    void WasMouseClicked()
    {
        if (Input.GetMouseButtonUp(0))
        {
            totalClicks++;
            //Vector3 point = Input.mousePosition;
            AddCube();

        }
    }

    void IsUndoNeeded()
    {
        if (Input.GetKeyDown("space")) {
            Undo();
        }
    }

    Vector3 CalculatePosition()
    {
        return (new Vector3(0.0f + totalClicks * 2.0f, 0.0f , 0.0f ));
    }


    /* AddCube():
 * (1) instantiates a cube and adds it to the list
 * (2) tells DistanceVisualizer to AddCube()
 */
    void AddCube() // add a cube
    {
        Vector3 newPosition = CalculatePosition();
        _spawnedObject = Instantiate(m_cubePrefab, newPosition, Quaternion.identity);
        _spawnList.Add(_spawnedObject);

        _lrs.AddCube(newPosition);
        Debug.Log(totalClicks + ": " + newPosition);

        //if (_onPlacedObject != null)
        //{
        //    _onPlacedObject();
        //}
    }

    /* Undo():
     * if there are still cubes in the scene 
     * (1) removes the last-created cube from the list and destroys it
     * (2) tells DistanceVisualizer to Undo()
     */
    public void Undo()
    {
        if (_spawnList.Count > 0)
        { // if there are no cubes, nothing happens
            GameObject removedCube = _spawnList[_spawnList.Count - 1];
            _spawnList.RemoveAt(_spawnList.Count - 1);

            Destroy(removedCube);
            _lrs.Undo();
        }

    }

}
