using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : MonoBehaviour {
	
	private bool isRun = false;
	private float endTime;
	private Rigidbody rb;

	void Start() {
		rb = gameObject.AddComponent<Rigidbody>();
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
	}

	void Update() {
		if (!isRun) return;
		rb.velocity = new Vector3(2000 * Time.deltaTime, rb.velocity.y, rb.velocity.z);
		this.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		if(Time.time > endTime) {
			isRun = false;
			this.gameObject.SetActive(false);
		}
	}

	public void run() {
		this.isRun = true;
		endTime = Time.time + 3f;
	}
	
}

