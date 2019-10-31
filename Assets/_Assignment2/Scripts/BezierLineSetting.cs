using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierLineSetting : MonoBehaviour
{
    private LineRenderer _lr; // A
    private int _vertexCount = 50;
    private float _velocityScale = 1.2f;
    // Start is called before the first frame update
    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
        _lr.startWidth = 0.02f;
        _lr.endWidth = 0.02f;

        _lr.useWorldSpace = true;
        _lr.positionCount = _vertexCount;
    }

    // Update is called once per frame
    void Update()
    {}

    public void BezierLineUpdate(Vector3 startPosition, Vector3 cubeVelocity, Vector3 cubePosition)
    {
        // set points for Bezier curve
		Vector3 p0 = startPosition;
        Vector3 midPoint = (cubePosition + startPosition)/2.0f;
		Vector3 p1 = midPoint + cubeVelocity*_velocityScale;
		Vector3 p2 = cubePosition;

		for(int i = 0; i < _vertexCount; i++)
		{
			float t = i / (_vertexCount - 1.0f);
			Vector3 newPosition = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
			_lr.SetPosition(i, newPosition);
		}
	
    }

}
