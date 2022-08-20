using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour {

	enum Direction {
		None,
		Left,
		Right
	}

	private const float normalGravity = 40;
	private const float holdingGravity = 10;
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

	private bool wallKicked = false;
	private float xPhysicsFactor = 0;
	private float gravity = 1f;

	CameraMovement camMovement;

	[SerializeField]
	ParticleSystem dust;

	void Start () {
		camMovement = Camera.main.GetComponent<CameraMovement>();
		newCollider = gameObject.GetComponent<CapsuleCollider>();
		dust = this.GetComponentInChildren<ParticleSystem>();
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
		rb.velocity = new Vector3(movement.x + xPhysicsFactor, yVelocity, rb.velocity.z);
		movement = new Vector3(0f, 0f, 0f);
		updateDrag();
		if (jumped){
			jumped = false;
			jumpCount++;
			rb.AddForce(Vector3.up * jumpForce * (bottomHit?0.8f:1f), ForceMode.Impulse);
			if (!bottomHit) PlayParticleSystem();
		}
	}

	private void handleInputs() {
		camMovement.LookAt(this.transform.position);
		if (Input.GetKey(KeyCode.A)) {
			movement.x -= speed * Time.deltaTime * (wallKicked?0.5f:1);
		}
		if (Input.GetKey(KeyCode.D)) {
			movement.x += speed * Time.deltaTime * (wallKicked?0.5f:1);
		}
		if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpAllowed){
			wallKicked = false;
			jumped = true;
			if (!bottomHit && leftHit) {
				xPhysicsFactor += speed * 8 * Time.deltaTime;
				wallKicked = true;
			}
			else if (!bottomHit && rightHit) {
				xPhysicsFactor -= speed * 8 * Time.deltaTime;
				wallKicked = true;
			}
		}
		if(bottomHit) wallKicked = false;
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

	void PlayParticleSystem()
	{
		dust.Play();
	}
}
