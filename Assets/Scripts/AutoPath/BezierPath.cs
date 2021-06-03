using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BezierPath
{
	[HideInInspector]
	public List<Vector3> pathPoints;
	private int segments;
	public int pointCount = 20;
	public GameObject[] controlPoint = new GameObject[4];

	public BezierPath()
	{
		pathPoints = new List<Vector3>();
	}

	/// <summary>
	/// Clears path points
	/// </summary>
	public void DeletePath()
	{
		pathPoints.Clear();
	}

	/// <summary>
	/// returns point along line using Bezier 4 point calculation
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	/// <param name="p3"></param>
	/// <param name="t"></param>
	/// <returns></returns>
	Vector3 BezierPathCalculation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		float tt = t * t;
		float ttt = t * tt;
		float u = 1.0f - t;
		float uu = u * u;
		float uuu = u * uu;

		Vector3 B = new Vector3();
		B = uuu * p0;
		B += 3.0f * uu * t * p1;
		B += 3.0f * u * tt * p2;
		B += ttt * p3;

		return B;
	}


	/// <summary>
	/// Populates path poins using location for 4 control points
	/// </summary>
	/// <param name="controlPoints"></param>
	public void CreateCurve(List<Vector3> controlPoints)
	{
		segments = controlPoints.Count / 3;

		for (int s = 0; s < controlPoints.Count - 3; s += 3)
		{
			Vector3 p0 = controlPoints[s];
			Vector3 p1 = controlPoints[s + 1];
			Vector3 p2 = controlPoints[s + 2];
			Vector3 p3 = controlPoints[s + 3];

			if (s == 0)
			{
				pathPoints.Add(BezierPathCalculation(p0, p1, p2, p3, 0.0f));
			}

			for (int p = 0; p < (pointCount / segments); p++)
			{
				float t = (1.0f / (pointCount / segments)) * p;
				Vector3 point = new Vector3();
				point = BezierPathCalculation(p0, p1, p2, p3, t);
				pathPoints.Add(point);
			}
		}
	}

	/// <summary>
	/// Converts Control Points to Vector3 to Create a path
	/// </summary>
	public void UpdatePath()
	{
		List<Vector3> c = new List<Vector3>();
		for (int i = 0; i < controlPoint.Length; i++)
		{
			if (controlPoint[i] != null)
			{
				Vector3 point = controlPoint[i].transform.position;
				c.Add(point);
			}
		}
		DeletePath();
		CreateCurve(c);
	}
}
