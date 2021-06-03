using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPathScript : MonoBehaviour
{




	int current = 1;
	int currentSegment = 0;
	public float speed = 50;
	public List<BezierPath> pathSegments;
	public bool isFollowingPath = false;


	//calls UpdatePath for all segments
	private void UpdatePath()
	{
		for (int i = 0; i < pathSegments.Count; i++)
		{
			pathSegments[i].UpdatePath();

		}
	}

	// Use this for initialization
	void Start()
	{
		UpdatePath();
	}

	// Update is called once per frame 
	void Update()
	{

		if (isFollowingPath)
		{
			float distanceToMove = speed * Time.deltaTime;
			float distanceToNextPoint = Vector3.Distance(transform.position, pathSegments[currentSegment].pathPoints[current]);


			while (distanceToNextPoint < distanceToMove)
            {
				distanceToMove -= distanceToNextPoint;

				transform.position = pathSegments[currentSegment].pathPoints[current];
				ChangeTrgetPointToNextPoint();

				distanceToNextPoint = Vector3.Distance(transform.position, pathSegments[currentSegment].pathPoints[current]);
			}

			if (Vector3.Distance(transform.position, pathSegments[currentSegment].pathPoints[current]) >= 0.1)
			{
				transform.position = Vector3.MoveTowards(transform.position, pathSegments[currentSegment].pathPoints[current], speed * Time.deltaTime);

			}
			else
			{
				ChangeTrgetPointToNextPoint();

			}
		}
	}

	//cheeks if at end of path segment and changes the currentSegment and current as needed
	private void ChangeTrgetPointToNextPoint()
    {
		if (current == pathSegments[currentSegment].pointCount - 1)
		{
			currentSegment = (currentSegment + 1) % pathSegments.Count;
			current = 1;
			return;

		}
		current = (current + 1) % pathSegments[currentSegment].pointCount;

	}

	public void SetToStartOfPath()
	{
		transform.position = pathSegments[0].pathPoints[0];
		current = 1;
		currentSegment = 0;
	}

	//Draw path the scene view
	void OnDrawGizmos()
	{
		UpdatePath();
		for (int section = 0; section < pathSegments.Count; section++)
		{
			for (int i = 1; i < (pathSegments[section].pointCount); i++)
			{
				Vector3 startv = pathSegments[section].pathPoints[i - 1];
				Vector3 endv = pathSegments[section].pathPoints[i];
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(startv, endv);
			}
		}
	}

}