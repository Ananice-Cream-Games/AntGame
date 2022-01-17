using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MobileMovement : MonoBehaviour
{
	[HideInInspector]
	public Vector2 playerMovement;
	[HideInInspector]
	public Vector2 cameraMovement;

	public TextMeshProUGUI debugText;

	public float cameraSensitivity;

	int movementTouch;
	int cameraTouch;

	private void Start()
	{
		Input.simulateMouseWithTouches = false;
		Input.multiTouchEnabled = true;

		movementTouch = -1;
		cameraTouch = -1;
	}

	private void Update()
	{
		float screenHalf = Screen.width / 2;

		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);

			if (touch.phase == TouchPhase.Began)
			{
				if (touch.position.x < screenHalf && movementTouch == -1) // Movement
				{
					movementTouch = touch.fingerId;
				}
				else if (touch.position.x > screenHalf && cameraTouch == -1) // Look
				{
					cameraTouch = touch.fingerId;
				}
			}
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				if (touch.fingerId == movementTouch) // Movement
				{
					movementTouch = -1;
					playerMovement = Vector2.zero;
				}
				else if (touch.fingerId == cameraTouch) // Look
				{
					cameraTouch = -1; 
					cameraMovement = Vector2.zero;
				}
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				if (touch.fingerId == movementTouch) // Movement
				{
					playerMovement = touch.deltaPosition;
				}
				else if (touch.fingerId == cameraTouch) // Look
				{
					cameraMovement = touch.deltaPosition * cameraSensitivity * Time.deltaTime;
				}
			}
			else if (touch.phase == TouchPhase.Stationary)
			{
				if (touch.fingerId == cameraTouch) // Look
				{
					cameraMovement = Vector2.zero;
				}
			}
		}

		debugText.text = $"movementTouch: {movementTouch}\ncameraTouch: {cameraTouch}\nscreenHalf: {screenHalf}\nplayerMovement: {playerMovement}\ncameraMovement: {cameraMovement}";
	}
}
