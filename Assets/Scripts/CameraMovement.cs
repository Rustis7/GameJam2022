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
	private Rigidbody playerRb;
	private Vector3 camTarget = new Vector3(0,0,0);

	public void LookAt(Vector3 pos) { 
		camTarget = new Vector3(pos.x + playerRb.velocity.x * camForwardFactor, (pos.y + 2) + (playerRb.velocity.y) * camForwardFactor, pos.z);
	}

	void Update() {
		if(playerRb == null) {
			GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
			playerRb = player.GetComponent<Rigidbody>();
			return;
		}
		updateCamRotation();
		updateCamPosition();
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

}
