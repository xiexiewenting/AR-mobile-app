using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRS_v1 : MonoBehaviour
{
    // TO DO LIST:
    // (1) SOMETHING VERY BUGGY ABOUT THE INTERACTION OF CUBES & BANNER COUNT !!!!
    // THIS BREAKS THE UNDO BUTTON.
    // (2) NEED TO FIX THE TEXT ROTATION WRT TO THE CAMERA

    public GameObject _textMeshPrefab;

    private LineRenderer _lr;
    private List<Vector3> _cubePositions = new List<Vector3>();
    private List<float> _cubePlacementTimes = new List<float>();

    private List<bool> _doneLerpingArray = new List<bool>();

    public List<GameObject> _distTextArray = new List<GameObject>();
    private List<bool> _doneDistTextArray = new List<bool>();


    private float _drawSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.material = new Material(Shader.Find("Sprites/Default")) { color = Color.blue };
        _lr.startWidth = 0.01f;
        _lr.endWidth = 0.01f;
        //_lr.startWidth = 0.2f; // for use in the Text scene 

        _lr.useWorldSpace = true;
        _lr.positionCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        int cubeTotal = _lr.positionCount;
        int i = 0;
        while ((i < cubeTotal) && (_lr.GetPosition(i) == _cubePositions[i]))
        // (1) i has to be less than cubeTotal
        // (2) if the vertex's current position is not the same as its final position
        //      we don't want to draw it 
        // want to stop when we hit a line segment where the end has not been drawn yet 
        {
            i++;
            if (i < cubeTotal) { LerpLineSegment(i); }
        }

        UpdateDistText();
    }

    public void AddCube(float placementTime, Vector3 placementPosition)
    {
        _cubePlacementTimes.Add(placementTime); // time when cube was placed 
        _cubePositions.Add(placementPosition); // position where cube was placed
        _lr.positionCount = _lr.positionCount + 1; // increasing vertex count

        //setting vertex of the added cube 
        int cubeIndex = _cubePositions.Count - 1;
        if (cubeIndex == 0)
        {
            _lr.SetPosition(cubeIndex, _cubePositions[cubeIndex]);
            _doneLerpingArray.Add(true);
        }
        else // we can finally set the next position of the next point 
        {
            _lr.SetPosition(cubeIndex, _lr.GetPosition(cubeIndex - 1));
            _doneLerpingArray.Add(false);
            _doneDistTextArray.Add(false); // 
        }

    }


    private void LerpLineSegment(int cubeIndex)
    {
        Vector3 currentPosition = _lr.GetPosition(cubeIndex);
        Vector3 finalEnd = _cubePositions[cubeIndex];

        if (currentPosition != finalEnd)
        {
            Vector3 prevPoint = _cubePositions[cubeIndex - 1];

            float distCovered = (Time.time - _cubePlacementTimes[cubeIndex]) * _drawSpeed;
            float journeyLength = Vector3.Distance(prevPoint, finalEnd);
            float fractionOfJourney = distCovered / journeyLength;

            Vector3 updatedEnd = Vector3.Lerp(prevPoint, finalEnd, fractionOfJourney);

            _lr.SetPosition(cubeIndex, updatedEnd);
        }
        else
        {
            _doneLerpingArray[cubeIndex] = true;
        }

    }


    private void UpdateDistText()
    {
        if (_cubePositions.Count > 1)
        {
            for (int i = 1; i < _cubePositions.Count; i++)
            {
                if (_doneLerpingArray[i] && (_doneDistTextArray[i - 1] == false))
                {
                    _doneDistTextArray[i - 1] = true;
                    CreateDistText(_cubePositions[i - 1], _cubePositions[i]);
                }
            }
        }
    }

    private void CreateDistText(Vector3 position1, Vector3 position2)
    {
        float deltaDistance = Vector3.Distance(position1, position2);
        Vector3 midPoint = (position1 + position2) / 2;

        GameObject textMeshObject = Instantiate(_textMeshPrefab, midPoint, Quaternion.identity);
        _distTextArray.Add(textMeshObject);
        TextMesh distText = textMeshObject.GetComponent<TextMesh>();

        distText.text = deltaDistance.ToString();
        distText.characterSize = 0.01f;
        distText.color = Color.black;
    }



    // SOMETHING VERY BUGGY ABOUT THE INTERACTION OF CUBES & BANNER COUNT !!!! 

    public void Undo() //removes last placed element! 
    {
        Debug.Log("There are " + _cubePositions.Count + " cubes and " + _doneDistTextArray.Count +
            " banners. Existence: " + _doneDistTextArray[_doneDistTextArray.Count - 1]);
        _cubePositions.RemoveAt(_cubePositions.Count - 1);
        _cubePlacementTimes.RemoveAt(_cubePlacementTimes.Count - 1);

        _doneLerpingArray.RemoveAt(_doneLerpingArray.Count - 1);

        if ((_distTextArray.Count > 0) && (_doneDistTextArray[_doneDistTextArray.Count - 1]))
        {
            //one fewer DistText than number of cubes (since banner exists for pairs of cubes)
            //check that the DistText had been created for the cube 
            GameObject deletedDistText = _distTextArray[_distTextArray.Count - 1];
            _distTextArray.RemoveAt(_distTextArray.Count - 1);

            Destroy(deletedDistText);
        }
        _doneDistTextArray.RemoveAt(_doneDistTextArray.Count - 1);
        _lr.positionCount = _lr.positionCount - 1;


    }

}
