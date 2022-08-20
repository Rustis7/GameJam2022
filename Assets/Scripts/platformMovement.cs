using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour {

	private const float lowSensor = 0.3f;
	private const float longSensor = 0.5f;
	private const float airFriction = 0.9f;

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
	[SerializeField]
	private float jumpCoolDown = 0.1f;
	private float nextJumpTime = 0;
	[SerializeField]
	private float wallKickCoolDown = 0.3f;
	private float nextWallKickTime = 0;

	private float xPhysicsFactor = 0;
	private float gravity = 1f;

	CameraMovement camMovement;

	void Start () {
		camMovement = Camera.main.GetComponent<CameraMovement>();
		newCollider = gameObject.GetComponent<CapsuleCollider>();
		setupMaterial();
		setupRigidbody();
	}

	private void setupRigidbody() {
		rb = gameObject.AddComponent<Rigidbody>();
		rb.angularDrag = 0;
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
		rb.useGravity = false;
		rb.interpolation = RigidbodyInterpolation.Interpolate;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}

	private void setupMaterial() {
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
		float yVelocity = rb.velocity.y - gravity;
		xPhysicsFactor *= airFriction;
		if (jumped){
			jumped = false;
			jumpCount++;
			yVelocity += jumpForce * (bottomHit?1f:0.5f);
		}
		rb.velocity = new Vector3(movement.x + xPhysicsFactor, yVelocity, rb.velocity.z);
		movement = new Vector3(0f, 0f, 0f);
		updateDrag();
	}

	private void handleInputs() {
		camMovement.LookAt(this.transform.position);
		updateStrafe();
		updateJump();
		if(bottomHit) xPhysicsFactor = 0;
	}

	private void updateStrafe() {
		if(nextWallKickTime > Time.time) return;
		if (Input.GetKey(KeyCode.A)) {
			movement.x -= speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D)) {
			movement.x += speed * Time.deltaTime;
		}
	}

	private void updateJump() {
		if (!Input.GetKeyDown(KeyCode.Space) || jumpCount >= maxJumpAllowed || Time.time < nextJumpTime) return;
		rb.drag = 0;
		nextJumpTime = Time.time + jumpCoolDown;
		jumped = true;
		if(bottomHit) return;
		if (leftHit) {
			xPhysicsFactor += speed * 20 * Time.deltaTime;
			nextWallKickTime = Time.time + wallKickCoolDown;
			return;
		}
		if (rightHit) {
			xPhysicsFactor -= speed * 20 * Time.deltaTime;
			nextWallKickTime = Time.time + wallKickCoolDown;
		}
	}

	private void updateDrag() {
		rb.drag = 0;
		if (rb.velocity.y < 0 && (leftHit || rightHit)) rb.drag = 10;
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

}
