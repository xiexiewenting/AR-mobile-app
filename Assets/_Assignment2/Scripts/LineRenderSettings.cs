using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderSettings : MonoBehaviour
    // SAME THING AS CURRENT LineRenderSettings EXCEPT WITH THE DEBUG LINES
{
    /* necessary GameObjects */
    public GameObject _textMeshPrefab;

    /* size(A) = size(B) = size(C) = size(D) */
    private LineRenderer _lr; // A
    private List<Vector3> _cubePositions = new List<Vector3>(); // B
    private List<float> _lerpStartTimes = new List<float>(); // C
    private List<bool> _doneLerpingArray = new List<bool>(); // D

    /*size(E) = size(A) - 1, NOT NEEDED FOR INDEX 0 */
    private List<bool> _doneDistTextArray = new List<bool>(); // E
    /*size(F) = size(E.element == true) */
    public List<GameObject> _distTextArray = new List<GameObject>(); // F

    private float _drawSpeed = 1.0f;

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

    void Update()
    {
        UpdateLerp();
        UpdateDistText();
    }

    /* UpdateLerp():
     * checks the line segments that needs lerping 
     */
    private void UpdateLerp()
    {
        int cubeTotal = _cubePositions.Count;
        int i = 0;
        while (i < cubeTotal)
        //i has to be less than cubeTotal
        {
            LerpLineSegment(i);
            i++;
        }
    }

    /* LerpLineSegment():
     * Lerps the line segment for the given cubeIndex 
     */
    private void LerpLineSegment(int cubeIndex)
    {
        if (_lr.positionCount < (cubeIndex + 1)) // if vertex not placed for the cube 
        {
            if (_doneLerpingArray[cubeIndex - 1])//and prev cube is done lerping 
            {
                _lr.positionCount += 1; // (A) increasing vertex count 
                _lr.SetPosition(cubeIndex, _lr.GetPosition(cubeIndex - 1)); // (A) setting lr.vertex
                _lerpStartTimes.Add(Time.time); // (C) setting placement time
            }
            return;
        }

        Vector3 currentPosition = _lr.GetPosition(cubeIndex);
        Vector3 finalEnd = _cubePositions[cubeIndex];

        if (currentPosition == finalEnd) // we are done lerping this cubeIndex 
        {
            _doneLerpingArray[cubeIndex] = true;
            return; 
        }
        //not done lerping yet 
        Vector3 prevPoint = _cubePositions[cubeIndex - 1];

        // calculating the line lerp 
        float distCovered = (Time.time - _lerpStartTimes[cubeIndex]) * _drawSpeed;
        float journeyLength = Vector3.Distance(prevPoint, finalEnd);
        float fractionOfJourney = distCovered / journeyLength;
        Vector3 updatedEnd = Vector3.Lerp(prevPoint, finalEnd, fractionOfJourney);
        _lr.SetPosition(cubeIndex, updatedEnd);
    }

    /* UpdateDistText(): 
     * checks if a DistText needs to be created 
     */
    private void UpdateDistText()
    {
        if (_cubePositions.Count <= 1) { return; }   
         // need at least 2 cubes to make a distanceText
        
        for (int i = 1; i < _cubePositions.Count; i++)
        {
            if (_doneLerpingArray[i] == false) { continue; }
            if (_doneDistTextArray[i-1]) { continue; }
            // (1) the line segment is done lerping
            // (2) the distanceText doesn't exist yet (always 1 less than # of cubes) 

            _doneDistTextArray[i - 1] = true; // (E) updating that distText is created 
            CreateDistText(_cubePositions[i - 1], _cubePositions[i]); // (F) create distText 
        }
    }


    /* CreateDistText():
     * creates the DistText when the segment is done lerping 
     */
    private void CreateDistText(Vector3 position1, Vector3 position2)
    {
        float deltaDistance = Vector3.Distance(position1, position2);
        Vector3 midPoint = (position1 + position2) / 2;

        GameObject textMeshObject = Instantiate(_textMeshPrefab, midPoint, Quaternion.identity);
        _distTextArray.Add(textMeshObject);

        TextMesh distText = textMeshObject.GetComponent<TextMesh>();
        distText.text = Math.Round(deltaDistance, 2).ToString() + "m";
        distText.characterSize = 0.01f;
        distText.color = Color.white;
    }


    /* *****
     * BETTER ADDCUBE METHOD !!!! 
     */
    public void AddCube(Vector3 placementPosition)
    {
        _cubePositions.Add(placementPosition); // (B) position where cube was placed

        //setting vertex of the added cube 
        int cubeIndex = _cubePositions.Count - 1;
        if (cubeIndex == 0)
        {
            _lr.positionCount += 1; // (A) increasing vertex count
            _lr.SetPosition(cubeIndex, _cubePositions[cubeIndex]); // (A) setting lr.vertex
            _lerpStartTimes.Add(Time.time); // (C) time when cube was placed 
            _doneLerpingArray.Add(true); // (D) no need to lerp
            // no need for (E) distText 
            return;
        }
        
        // we can finally set the next position of the next point 
        _doneLerpingArray.Add(false); // (D) we need to lerp 
        _doneDistTextArray.Add(false); // (E) distText not created yet 
    
    }


    /* ********* 
     * BETTER UNDO METHOD !!!!! 
     * Undo():
     * removes last placed element! 
     */
    public void Undo()
    {
        int total_cubes = _cubePositions.Count;
        _cubePositions.RemoveAt(total_cubes - 1); // remove (B)
        _doneLerpingArray.RemoveAt(_doneLerpingArray.Count - 1); // remove (D)

        if (_lr.positionCount != total_cubes) { return; } // if a lr.vertex existed for the cube 
        
        _lr.positionCount -= 1; // remove (A)
        _lerpStartTimes.RemoveAt(total_cubes - 1); // remove (C)

        if (_distTextArray.Count <= 0) { return; }
         // one fewer DistText than cubes (since banner exists for pairs of cubes)

        if (_doneDistTextArray[_doneDistTextArray.Count - 1])
        { // If DistText had been created for the cube, destroy the cube  

            GameObject deletedDistText = _distTextArray[_distTextArray.Count - 1];
            _distTextArray.RemoveAt(_distTextArray.Count - 1); // remove (F)
            Destroy(deletedDistText);
        }
        // regardless, remove the boolean 
        _doneDistTextArray.RemoveAt(_doneDistTextArray.Count - 1); // remove (E) 
    }

    public void DestroyDistText()
    {
        while (_distTextArray.Count > 0)
        {
            GameObject removedDistText = _distTextArray[0];
            _distTextArray.RemoveAt(0);
            Destroy(removedDistText);
         }
    }

}
