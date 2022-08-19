using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformMovement : MonoBehaviour {

	private const float normalGravity = 40;
	private const float holdingGravity = 10;
	private const float lowSensor = 0.3f;
	private const float longSensor = 0.5f;

	[SerializeField]
	private int maxJumpAllowed = 2;
	[SerializeField]
	private int jumpCount = 1;
	[SerializeField]
	private float speed = 4.0f;
	[SerializeField]
	private float jumpForce = 4.0f;
	[SerializeField]
	private Rigidbody rb;
	[SerializeField]
	private CapsuleCollider newCollider;
	[SerializeField]
	private float jumpOrientaion;
	[SerializeField]
	private bool bottomHit;
	[SerializeField]
	private bool leftHit;
	[SerializeField]
	private bool rightHit;
	[SerializeField]
	private float sensorLength = 0f;
	[SerializeField]
	private Vector2 movement;
	[SerializeField]
	private Boolean jumped = false;

	private Camera cam;

	void Start () {
		setupCamera();
		rb = gameObject.AddComponent<Rigidbody>();
		rb.angularDrag = 0;
		//rb.drag = 20;
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
		newCollider = gameObject.GetComponent<CapsuleCollider>();
		PhysicMaterial playerMat = new PhysicMaterial("playerMat");
		playerMat.dynamicFriction = 0.1f;
		playerMat.staticFriction = 0.1f;
		playerMat.frictionCombine = PhysicMaterialCombine.Multiply;
		newCollider.sharedMaterial = playerMat;
	}

	void Update () {
		updateSensors();
		updateJumpCount();
		//updatePhysicReaction();
		handleInputs();
	}

	void FixedUpdate(){
		//rb.AddForce(movement, ForceMode.Force);
		rb.velocity = new Vector3(movement.x, rb.velocity.y, rb.velocity.z);
		movement = new Vector3(0f, 0f, 0f);
		if (jumped){
			jumped = false;
			jumpCount++;
			rb.AddForce(getJumpDirection() * jumpForce, ForceMode.Impulse);
		}
	}

	private void updatePhysicReaction(){
		/*
		if (bottomHit.collider == null && rb.velocity.y < 0 && (leftHit.collider != null || rightHit.collider != null)){
			rb.gravityScale = holdingGravity;
		}
		else {
			rb.gravityScale = normalGravity;
		}
		*/
	}

	private void handleInputs() {
		jumpOrientaion = 90;
		if (Input.GetKey(KeyCode.A)){
			//jumpOrientaion += 45.5f;
			movement.x -= speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D)){
			//jumpOrientaion -= 45.5f;
			movement.x += speed * Time.deltaTime;
		}
		//jumpOrientaion *= Mathf.Deg2Rad;
		if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpAllowed){
			jumped = true;
		}
	}

	private Vector2 getJumpDirection() {
		if (leftHit && !bottomHit){
			jumpOrientaion = 45f * Mathf.Deg2Rad;
		}else if (rightHit && !bottomHit){
			jumpOrientaion = 135f * Mathf.Deg2Rad;
		}
		return new Vector2(Mathf.Cos(jumpOrientaion), Mathf.Sin(jumpOrientaion));
	}

	private float getJumpOrientation() {
		if (leftHit){
			return 45 * Mathf.Deg2Rad;
		}else if (rightHit){
			return 135 * Mathf.Deg2Rad;
		}else if (bottomHit){
			return 90 * Mathf.Deg2Rad;
		}
		return 0;
	}

	private void updateJumpCount() {
		if (leftHit || rightHit || bottomHit){
			jumpCount = 1;
		}
	}

	private void updateSensors() {
		sensorLength = (bottomHit) ? lowSensor : longSensor;
		bottomHit = Physics.Raycast(transform.position - new Vector3(0f, newCollider.height/2 , 0f), transform.TransformDirection(Vector3.down), sensorLength);
		leftHit = Physics.Raycast(transform.position - new Vector3(newCollider.radius, 0f, 0f), transform.TransformDirection(Vector3.left), sensorLength);
		rightHit = Physics.Raycast(transform.position + new Vector3(newCollider.radius, 0f, 0f), transform.TransformDirection(Vector3.right), sensorLength);
		Debug.DrawRay(transform.position - new Vector3(0f, newCollider.height/2, 0f), transform.TransformDirection(Vector3.down) * sensorLength, Color.blue);
		Debug.DrawRay(transform.position - new Vector3(newCollider.radius, 0f, 0f), transform.TransformDirection(Vector3.left) * sensorLength, Color.yellow);
		Debug.DrawRay(transform.position + new Vector3(newCollider.radius, 0f, 0f), transform.TransformDirection(Vector3.right) * sensorLength, Color.yellow);
	}

	private void setupCamera() {
		cam = Camera.main;
		cam.transform.SetParent(this.transform);
		cam.transform.localPosition = new Vector3(0, 8, -11);
		cam.transform.localRotation = Quaternion.Euler(11f, 0f, 0f);
	}

}
