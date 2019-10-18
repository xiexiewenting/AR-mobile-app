using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LineRenderSettings : MonoBehaviour
{
    private LineRenderer _lr;
    private List<float> _cubePlacementTimes = new List<float>();
    private List<Vector3> _cubePositions = new List<Vector3>();
    private List<bool> _finishedDrawing = new List<bool>();


    private float _drawSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.startWidth = 0.02f;
        _lr.endWidth = 0.02f;
        _lr.startColor = Color.blue;
        _lr.endColor = Color.blue;
        _lr.useWorldSpace = true;
        _lr.positionCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        int cubeTotal = _lr.positionCount;//_cubePositions.Count;
        int index = 0;
        while ((index < cubeTotal) && 
            (_lr.GetPosition(index) == _cubePositions[index]))
            // want to stop when we hit a line segment where the end has not been drawn yet 
        {
            index++;
            if (index < cubeTotal)
            {
                lerpLine(index);
                //Debug.Log("index: " + index + ", cubetotal: " + cubeTotal);
            }
        }

        _lr.startWidth = 0.02f;
        _lr.endWidth = 0.02f;

    }

    public void addCube(float placementTime, Vector3 placementPosition)
    {
        _cubePlacementTimes.Add(placementTime); // time when cube was placed 
        _cubePositions.Add(placementPosition); // position where cube was placed


        //setting vertex of the added cube 
        int cubeIndex = _cubePositions.Count - 1;
        _lr.positionCount = _cubePositions.Count; // increasing vertex count
        if (cubeIndex == 0)
        {
            _lr.SetPosition(cubeIndex, _cubePositions[cubeIndex]);
        }
        else // we can finally set the next position of the next point 
        {
            _lr.SetPosition(cubeIndex, _lr.GetPosition(cubeIndex - 1));
        }

        Debug.Log("Total points in LR: " + (cubeIndex + 1));
    }

    private void lerpLine(int cubeIndex)
    {
        Vector3 currentPosition = _lr.GetPosition(cubeIndex);
        Vector3 finalEnd = _cubePositions[cubeIndex];

        if (currentPosition != finalEnd)
        {
            Vector3 prevPoint = _cubePositions[cubeIndex - 1] ;

            float distCovered = (Time.time - _cubePlacementTimes[cubeIndex]) * _drawSpeed;
            float journeyLength = Vector3.Distance(prevPoint, finalEnd);
            float fractionOfJourney = distCovered / journeyLength;

            Vector3 updatedEnd = Vector3.Lerp(prevPoint, finalEnd, fractionOfJourney);

            _lr.SetPosition(cubeIndex, updatedEnd);
        }

    }














    //------------------------------------------------DEBUGGING METHOD 
    //public void addPoints(List<Vector3> vertices)
    //{
    //    Debug.Log("-------------------------------");
    //    Debug.Log("!!!!!! cubeIndex: " + vertices.Count);

    //    _lr.positionCount = vertices.Count;
    //    _lr.SetPositions(vertices.ToArray());

    //    Vector3[] vertexPositions = new Vector3[vertices.Count];
    //    _lr.GetPositions(vertexPositions);
    //    for (int i = 0; i < (vertices.Count); i++)
    //    {
    //        Debug.Log("!!!!!! " + vertices.Count + ": Index at " + i + " currently is " + vertexPositions[i]);
    //    }
    //    Debug.Log("-------------------------------");

       
    //}
} 
