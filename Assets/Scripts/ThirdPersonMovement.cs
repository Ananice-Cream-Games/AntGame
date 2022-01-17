using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public Transform playerCamera;
    public Rigidbody rb;
    public SphereCollider myCollider;
    public MobileMovement mobileMovement;

    public Vector3 groundNormal;

    public float walkSpeed;
    public float mouseSensitivity;
    public float groundDistance;
    public float fallRaycastRadius;
    public float maxGroundDistanceToFall;
    public float jumpForce;

    public LayerMask layerMask;

    Vector3 move;
    float xRotation = 0;
    List<ContactPoint> contactPoints = new List<ContactPoint>();
    bool isOnGround;

	private void Start()
	{
        Cursor.lockState = CursorLockMode.Locked;
	}

    // Update is called once per frame
    private void Update()
    {
        RotatePlayer();
        MovePlayer();
    }

	private void FixedUpdate()
	{
        if (contactPoints.Count > 0)
        {
            OnHitGround();

            // Calc new normal (sum of vectors), ignore if gameObejct has tag IgnoreNormal
            Vector3 newNormal = contactPoints.Aggregate(Vector3.zero, 
                (Vector3 sum, ContactPoint next) => {
                    if (!next.otherCollider.gameObject.tag.Contains("IgnoreNormal"))
                        return sum + next.normal;
                    else
                        return sum + groundNormal;
                });

            // If only IgnoreNormal objects, don't change
            if (newNormal == Vector3.zero)
                newNormal = groundNormal;

            newNormal = newNormal.normalized;

            // Calc pos of the ground
            Vector3 groundPos = contactPoints.Aggregate(Vector3.zero, (Vector3 sum, ContactPoint next) => {
                return sum + next.point;
            });
            groundPos /= contactPoints.Count;

            Debug.DrawRay(groundPos, newNormal * 2, Color.red);

            // Rotate body
            gameObject.transform.Rotate(Quaternion.FromToRotation(gameObject.transform.up, newNormal).eulerAngles, Space.World);

            // Move body
            if (!contactPoints.All(cp => cp.otherCollider.gameObject.tag.Contains("IgnoreNormal")))
            {
                gameObject.transform.position = groundPos + newNormal * groundDistance;
            }

            // Apply normal
            groundNormal = newNormal;
        }
        else // not touching ground!
		{
            bool res = Physics.SphereCast(gameObject.transform.position, fallRaycastRadius, groundNormal * -1, out RaycastHit hit, maxGroundDistanceToFall, layerMask);
            
            // Replace on ground
            if (res)
			{
                Vector3 newPosition;

                // If must prserve normal
                if (hit.collider.gameObject.tag.Contains("IgnoreNormal"))
				{
                    Debug.Log("Fall on ignoreNormal!");

                    newPosition = hit.point; // Hit point
                    Debug.DrawRay(newPosition, hit.normal * groundDistance, Color.white, 2, false);
                    newPosition += hit.normal * groundDistance; // Center of collider
                    Debug.DrawRay(newPosition, - groundNormal * groundDistance, Color.white, 2, false);
                    newPosition += -groundNormal * groundDistance; // Bottom of the collider
                }
                else
				{
                    // Replace to the first point touched
                    newPosition = hit.point;
				}

                gameObject.transform.position = newPosition + groundNormal * groundDistance;
                OnHitGround();
            }
            else // Fall
			{
                OnLeaveGround();
			}
		}

        contactPoints.Clear();

        // apply move
        gameObject.transform.position += move * Time.fixedDeltaTime;

        // check jump
        CheckJump();
    }

	private void OnCollisionStay(Collision collision)
	{
        contactPoints.AddRange(collision.contacts);
    }

    private void RotatePlayer()
	{
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (mobileMovement != null)
        {
            mouseX += mobileMovement.cameraMovement.x;
            mouseY += mobileMovement.cameraMovement.y;
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        gameObject.transform.Rotate(groundNormal, mouseX, Space.World);
    }

    private void MovePlayer()
	{
		if (isOnGround)
		{
			float x = 0, z = 0;

            if (mobileMovement != null)
			{
                if (mobileMovement.playerMovement.x > 0)
                    x = 1;
                else if (mobileMovement.playerMovement.x < 0)
                    x = -1;

                if (mobileMovement.playerMovement.y > 0)
                    z = 1;
                else if (mobileMovement.playerMovement.y < 0)
                    z = -1;
            }

			if (Input.GetKey(KeyCode.Z))
				z = 1;
			if (Input.GetKey(KeyCode.Q))
				x = -1;
			if (Input.GetKey(KeyCode.S))
				z = -1;
			if (Input.GetKey(KeyCode.D))
				x = 1;

			move = gameObject.transform.right * x + gameObject.transform.forward * z;
			move *= walkSpeed; 
		}
    }

    /// <summary>
    /// Check jump for keyboard
    /// </summary>
    private void CheckJump()
	{
        if (isOnGround && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public void Jump()
	{
        if (isOnGround)
		{
            rb.AddForce(groundNormal * jumpForce);
            gameObject.transform.position += groundNormal * (maxGroundDistanceToFall * 1.5f);
            OnLeaveGround();
        }
	}

    private void OnHitGround()
	{
        isOnGround = true;
        rb.drag = 100000;
	}

    private void OnLeaveGround()
	{
        isOnGround = false;
        rb.drag = 0;
    }
}
