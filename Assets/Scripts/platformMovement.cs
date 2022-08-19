using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformMovement : MonoBehaviour {

	private const float normalGravity = 40;
	private const float holdingGravity = 10;
	private const float lowSensor = 0.3f;
	private const float longSensor = 0.5f;

	private int maxJumpAllowed = 2;
	private int jumpCount = 1;
	private float speed = 0.04f;
	private float jumpForce = 0.004f;
	private Rigidbody2D rb;
	private BoxCollider2D newCollider;
	private float jumpOrientaion;
	private RaycastHit2D bottomHit;
	private RaycastHit2D leftHit;
	private RaycastHit2D rightHit;
	private float sensorLength = 0f;
	private Vector2 movement;
	private Boolean jumped = false;


	void Start () {
		rb = GetComponent<Rigidbody2D>();
		rb.angularDrag = 0;
		rb.drag = 20;
		speed = rb.mass * 400;
		jumpForce = rb.mass * 150;
		newCollider = GetComponent<BoxCollider2D>();
		PhysicsMaterial2D playerMat = new PhysicsMaterial2D("playerMat");
		playerMat.friction = 0;
		newCollider.sharedMaterial = playerMat;
	}
	
	void Update () {
		updateSensors();
		updateJumpCount();
		updatePhysicReaction();
		handleInputs();
	}

	void FixedUpdate(){
		rb.AddForce(movement, ForceMode2D.Force);
		if (jumped){
			jumped = false;
			jumpCount++;
			rb.AddForce(getJumpDirection() * jumpForce, ForceMode2D.Impulse);
		}
	}

	private void updatePhysicReaction(){
		if (bottomHit.collider == null && rb.velocity.y < 0 && (leftHit.collider != null || rightHit.collider != null)){
			rb.gravityScale = holdingGravity;
		}
		else {
			rb.gravityScale = normalGravity;
		}
	}

	private void handleInputs() {
		movement = new Vector3(0f, 0f, 0f);
		jumpOrientaion = 90;
		if (Input.GetKey(KeyCode.A)){
			jumpOrientaion += 45;
			movement.x -= speed;
		}
		if (Input.GetKey(KeyCode.D)){
			jumpOrientaion -= 45;
			movement.x += speed;
		}
		jumpOrientaion *= Mathf.Deg2Rad;
		if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpAllowed){
			jumped = true;
		}
	}

	private Vector2 getJumpDirection() {
		if (leftHit.collider != null && bottomHit.collider == null){
			jumpOrientaion = 45f * Mathf.Deg2Rad;
		}else if (rightHit.collider != null && bottomHit.collider == null){
			jumpOrientaion = 135f * Mathf.Deg2Rad;
		}
		return new Vector2(Mathf.Cos(jumpOrientaion), Mathf.Sin(jumpOrientaion));
	}

	private float getJumpOrientation() {
		if (leftHit.collider != null){
			return 45 * Mathf.Deg2Rad;
		}else if (rightHit.collider != null){
			return 135 * Mathf.Deg2Rad;
		}else if (bottomHit.collider != null){
			return 90 * Mathf.Deg2Rad;
		}
		return 0;
	}

	private void updateJumpCount() {
		if (leftHit.collider != null || rightHit.collider != null || bottomHit.collider != null){
			jumpCount = 1;
		}
	}

	private void updateSensors() {
		sensorLength = (bottomHit.collider != null) ? lowSensor : longSensor;
		bottomHit = Physics2D.Raycast(transform.position - new Vector3(0f, newCollider.size.y / 2, 0f), transform.TransformDirection(Vector3.down), sensorLength);
		leftHit = Physics2D.Raycast(transform.position - new Vector3(newCollider.size.x / 2, 0f, 0f), transform.TransformDirection(Vector3.left), sensorLength);
		rightHit = Physics2D.Raycast(transform.position + new Vector3(newCollider.size.x / 2, 0f, 0f), transform.TransformDirection(Vector3.right), sensorLength);
		Debug.DrawRay(transform.position - new Vector3(0f, newCollider.size.y / 2, 0f), transform.TransformDirection(Vector3.down) * sensorLength, Color.blue);
		Debug.DrawRay(transform.position - new Vector3(newCollider.size.x / 2, 0f, 0f), transform.TransformDirection(Vector3.left) * sensorLength, Color.yellow);
		Debug.DrawRay(transform.position + new Vector3(newCollider.size.x / 2, 0f, 0f), transform.TransformDirection(Vector3.right) * sensorLength, Color.yellow);
	}

}
