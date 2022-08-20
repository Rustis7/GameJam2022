using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformMovement : MonoBehaviour {

	private const float normalGravity = 40;
	private const float holdingGravity = 10;
	private const float lowSensor = 0.3f;
	private const float longSensor = 0.5f;
	//private const float gravityCap = -10f;

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
	private float jumpOrientation;
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

	// L -> left | R -> right | O -> nothing
	private char jumpDirection = 'O';
	private float gravity = 1f;

	private Camera cam;

	void Start () {
		setupCamera();
		rb = gameObject.AddComponent<Rigidbody>();
		rb.angularDrag = 0;
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
		rb.useGravity = false;
		rb.interpolation = RigidbodyInterpolation.Interpolate;
		newCollider = gameObject.GetComponent<CapsuleCollider>();
		PhysicMaterial playerMat = new PhysicMaterial("playerMat");
		playerMat.dynamicFriction = 0.001f;
		playerMat.staticFriction = 0.001f;
		playerMat.frictionCombine = PhysicMaterialCombine.Multiply;
		playerMat.bounciness = 0;
		playerMat.bounceCombine = PhysicMaterialCombine.Multiply;
		newCollider.sharedMaterial = playerMat;
	}

	void Update () {
		updateSensors();
		updateJumpCount();
		handleInputs();
	}

	void FixedUpdate(){
		//if (bottomHit && !jumped) 
		float yVelocity = rb.velocity.y - gravity;//(rb.velocity.y - gravity < gravityCap? 0: 1);
		rb.velocity = new Vector3(movement.x, yVelocity, rb.velocity.z);
		//else rb.AddForce(movement, ForceMode.Force);
		movement = new Vector3(0f, 0f, 0f);
		updateDrag();
		if (jumped){
			jumped = false;
			jumpCount++;
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			//rb.AddForce(getJumpDirection() * jumpForce, ForceMode.Impulse);
		}
	}

	private void handleInputs() {
		jumpOrientation = 90;
		if (Input.GetKey(KeyCode.A)) {
			jumpOrientation += 45.5f;
			movement.x -= speed * (bottomHit?1:getAirDirectionMomentum('L')) * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D)) {
			jumpOrientation -= 45.5f;
			movement.x += speed * (bottomHit?1:getAirDirectionMomentum('R')) * Time.deltaTime;
		}
		//movement.x -= speed * Mathf.Cos(jumpOrientation * Mathf.Deg2Rad) * Time.deltaTime;
		jumpOrientation *= Mathf.Deg2Rad;
		if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpAllowed){
			if(movement.x > 0) jumpDirection = 'R';
			if(movement.x < 0) jumpDirection = 'L';
			if(movement.x == 0) jumpDirection = 'O';
			jumped = true;
		}
	}

	private float getAirDirectionMomentum(char direction) {
		if(jumpDirection == direction) return 0.7f;
		if(jumpDirection == 0) return 0.5f;
		return 0.3f;
	}

	private updateDrag() {
		rb.drag = 0;
		if (leftHit || rightHit) rb.drag = 10;
	}

	private Vector2 getJumpDirection() {
		rb.drag = 0;
		if (leftHit && !bottomHit){
			jumpOrientation = 45f * Mathf.Deg2Rad;
			rb.drag = 10;
		}else if (rightHit && !bottomHit){
			jumpOrientation = 135f * Mathf.Deg2Rad;
			rb.drag = 10;
		}
		return new Vector2(Mathf.Cos(jumpOrientation), Mathf.Sin(jumpOrientation));
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
		bottomHit = Physics.Raycast(transform.position - new Vector3(0f, newCollider.height/2 - 0.1f, 0f), transform.TransformDirection(Vector3.down), longSensor);
		leftHit = Physics.Raycast(transform.position - new Vector3(newCollider.radius - 0.1f, 0f, 0f), transform.TransformDirection(Vector3.left), sensorLength);
		rightHit = Physics.Raycast(transform.position + new Vector3(newCollider.radius - 0.1f, 0f, 0f), transform.TransformDirection(Vector3.right), sensorLength);
		Debug.DrawRay(transform.position - new Vector3(0f, newCollider.height/2 - 0.1f, 0f), transform.TransformDirection(Vector3.down) * sensorLength, Color.blue);
		Debug.DrawRay(transform.position - new Vector3(newCollider.radius, 0f - 0.1f, 0f), transform.TransformDirection(Vector3.left) * sensorLength, Color.yellow);
		Debug.DrawRay(transform.position + new Vector3(newCollider.radius, 0f - 0.1f, 0f), transform.TransformDirection(Vector3.right) * sensorLength, Color.yellow);
	}

	private void setupCamera() {
		cam = Camera.main;
		cam.transform.SetParent(this.transform);
		cam.transform.localPosition = new Vector3(0, 6, -11);
		cam.transform.localRotation = Quaternion.Euler(11f, 0f, 0f);
	}

}
