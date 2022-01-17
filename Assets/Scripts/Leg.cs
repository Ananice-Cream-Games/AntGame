using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Leg: MonoBehaviour
{
	public ThirdPersonMovement controller;

	public Transform[] models;
	public float[] legSizes;

	public int nbIterations;

	public Transform legStart;
	public Transform raycastOrigin;
	public float raycastMaxLength;
	public Transform targetIfCantWalk;

	private void Update()
	{
		// Get target
		Vector3 target;
		bool canWalk = Physics.Raycast(raycastOrigin.position, controller.groundNormal * -1, out RaycastHit hit, raycastMaxLength, controller.layerMask);

		if (canWalk)
		{
			target = hit.point;
		}
		else
		{
			target = targetIfCantWalk.position;
		}

		Debug.DrawLine(legStart.position, target, Color.magenta);

		// Get IK points
		Vector3[] points = new Vector3[legSizes.Length + 1];

		for (int i = 0; i < nbIterations; i++)
		{
			points[0] = legStart.position;

			for (int n = 0; n < points.Length - 1; n++)
			{
				Vector3 direction = (points[n + 1] - points[n]).normalized;
				points[n + 1] = points[n] + direction * legSizes[n];
			}

			points[^1] = target;

			for (int n = points.Length - 1; n > 0; n--)
			{
				Vector3 direction = (points[n - 1] - points[n]).normalized;
				points[n - 1] = points[n] + direction * legSizes[n-1];
			}
		}

		for (int n = 0; n < points.Length - 1; n++)
		{
			Debug.DrawLine(points[n], points[n+1], Color.blue);
		}
	}
}
