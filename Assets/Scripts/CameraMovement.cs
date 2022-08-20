using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	[SerializeField]
	private float posCamSpeed = 15f;
	[SerializeField]
	private float rotCamSpeed = 0.2f;
	[SerializeField]
	private float camForwardFactor = 0.12f;
	[SerializeField]
	private float maxFov= 70;
	[SerializeField]
	private float minFov= 60;
	[SerializeField]
	private float fovSpeed = 3f;

	private Rigidbody playerRb;
	private Vector3 camTarget = new Vector3(0,0,0);
	private Camera cam;

	void Start() {
		cam = gameObject.GetComponent<Camera>();
	}

	void Update() {
		if(playerRb == null) {
			GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
			playerRb = player.GetComponent<Rigidbody>();
			return;
		}
		updateCamRotation();
		updateCamPosition();
		updateCamFov();
	}

	private void updateCamFov() {
		float magnitudeSpeed = playerRb.velocity.magnitude;
		if(magnitudeSpeed == 0) magnitudeSpeed = -0.8f;
		cam.fieldOfView += magnitudeSpeed * Time.deltaTime * fovSpeed;
		if(cam.fieldOfView > maxFov) cam.fieldOfView = maxFov;
		if(cam.fieldOfView < minFov) cam.fieldOfView = minFov;
	}

	private void updateCamRotation () {
		Vector3 direction = camTarget - transform.position;
		Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
		transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotCamSpeed * Time.deltaTime * playerRb.velocity.magnitude);
	}

	private void updateCamPosition() {
		float currentCamSpeed = posCamSpeed * (playerRb.velocity.x == 0?0.5f:1f);
		Vector3 newPos = currentCamSpeed * (camTarget - transform.position) * Time.deltaTime;
		this.transform.position = new Vector3(transform.position.x + newPos.x, transform.position.y + newPos.y, camTarget.z-15f);
	}

	public void LookAt(Vector3 pos) { 
		if(playerRb == null) return;
		camTarget = new Vector3(pos.x + playerRb.velocity.x * camForwardFactor, (pos.y + 2) + (playerRb.velocity.y) * camForwardFactor, pos.z);
	}

}
