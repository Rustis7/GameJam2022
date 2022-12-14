using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour {

	private const float lowSensor = 0.3f;
	private const float longSensor = 0.5f;
	private const float airFriction = 0.9f;

	[SerializeField]
	ParticleSystem dust;
	[SerializeField]
	private AudioClip runningSound;
	[SerializeField]
	private AudioClip jumpSound;
	[SerializeField]
	private AudioClip landingSound;
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
	private float jumpCoolDown = 0.2f;
	private float nextJumpTime = 0;
	[SerializeField]
	private float wallKickCoolDown = 0.3f;
	private float nextWallKickTime = 0;

	private float xPhysicsFactor = 0;
	private float gravity = 1f;
	private bool landed = true;

	private CameraMovement camMovement;
	private AudioSource audioSrc;
	private AudioSource effectAudioSrc;
	private Player player;
	private Animator anim;
	private float animationRotationFactor = 0;

	void Start () {
		anim = gameObject.GetComponentInChildren<Animator>();
		player = gameObject.GetComponent<Player>();
		camMovement = Camera.main.GetComponent<CameraMovement>();
		newCollider = gameObject.GetComponent<CapsuleCollider>();
		dust = this.GetComponentInChildren<ParticleSystem>();
		setupMaterial();
		setupRigidbody();
		audioSrc = gameObject.AddComponent<AudioSource>();
		effectAudioSrc = gameObject.AddComponent<AudioSource>();
		effectAudioSrc.loop = false;
		audioSrc.loop = true;
		audioSrc.clip = runningSound;
		playGetStolen();
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
		updateRunningSound();
		updateModelOrientation();
	}

	void updateModelOrientation() {
		anim.SetBool("falling", !bottomHit && rb.velocity.y < -1);
		anim.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 1 && bottomHit);
		if(rb.velocity.x > 1) animationRotationFactor = 1f;
		else if(rb.velocity.x < -1) animationRotationFactor = -1f;
		anim.gameObject.transform.rotation = Quaternion.Euler(0, 90f*animationRotationFactor, 0);
	}

	void FixedUpdate(){
		updateMovement();
		movement = new Vector3(0f, 0f, 0f);
		updateDrag();
	}

	private void updateMovement() {
		float yVelocity = rb.velocity.y - gravity;
		xPhysicsFactor *= airFriction;
		if (jumped) {
			jumped = false;
			jumpCount++;
			yVelocity += jumpForce * (bottomHit?1f:0.5f);
			if (!bottomHit) {
				PlayParticleSystem();
				anim.SetTrigger("doubleJump");
			} else anim.SetTrigger("jump");
		}
		rb.velocity = new Vector3(movement.x + xPhysicsFactor, yVelocity, rb.velocity.z);
	}

	private void handleInputs() {
		camMovement.LookAt(this.transform.position);
		updateStrafe();
		updateJump();
		if(bottomHit) {
			xPhysicsFactor = 0;
			if(rb.velocity.y < 0f && !landed) {
				playSound(landingSound);
				landed = true;
			}
		}else landed = false;
	}

	private void updateStrafe() {
		if(player.isDead()) return;
		if(nextWallKickTime > Time.time) return;
		if (Input.GetKey(KeyCode.A)) {
			movement.x -= calculateCurrentSpeed() * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D)) {
			movement.x += calculateCurrentSpeed() * Time.deltaTime;
		}
	}

	private void updateJump() {
		if (!Input.GetKeyDown(KeyCode.Space) || jumpCount >= maxJumpAllowed || Time.time < nextJumpTime || player.isDead()) return;
		playSound(jumpSound);
		rb.drag = 0;
		nextJumpTime = Time.time + jumpCoolDown;
		jumped = true;
		if(bottomHit) return;
		if (leftHit) {
			xPhysicsFactor += calculateCurrentSpeed() * 10 * Time.deltaTime;
			nextWallKickTime = Time.time + wallKickCoolDown;
			return;
		}
		if (rightHit) {
			xPhysicsFactor -= calculateCurrentSpeed() * 10 * Time.deltaTime;
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

	private void PlayParticleSystem() {
		dust.Play();
	}

	private void updateRunningSound () {
		if(audioSrc.isPlaying && (rb.velocity.magnitude == 0 || !bottomHit)) audioSrc.Pause();
		if(!audioSrc.isPlaying && rb.velocity.magnitude > 0 && bottomHit) audioSrc.Play();
	}

	private void playSound(AudioClip clip) {
		if(clip == null) return;
		float maxPitch = 1.25f;
		float minPitch = 0.75f;
		effectAudioSrc.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
		effectAudioSrc.PlayOneShot(clip, UnityEngine.Random.Range(minPitch, maxPitch));
	}

	private float calculateCurrentSpeed() {
		return this.speed - this.speed * (player.getWetness()/100)/4;
	}

	private void playGetStolen() {
		player.skip = true;
		GameObject thief = GameObject.FindGameObjectsWithTag("thief")[0];
		thief.transform.rotation = Quaternion.Euler(0, 0, 0);
		GameObject umbrella = GameObject.FindGameObjectsWithTag("umbrella")[0];
		GameObject hand = GameObject.FindGameObjectsWithTag("hand")[0];
		umbrella.transform.SetParent(hand.transform);
		umbrella.transform.localPosition = new Vector3(0,0,0);
		umbrella.transform.localRotation = Quaternion.Euler(0,0,0);
		anim.SetBool("hasParapluie", true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(!other.gameObject.CompareTag("thief")) return;
		player.skip = false;
		other.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
		anim.SetBool("hasParapluie", false);
		GameObject umbrella = GameObject.FindGameObjectsWithTag("umbrella")[0];
		GameObject hand = GameObject.FindGameObjectsWithTag("thiefHand")[0];
		Animator otherAnim = other.gameObject.GetComponentInChildren<Animator>();
		otherAnim.SetBool("hasParapluie", true);
		otherAnim.SetBool("isRunning", true);
		umbrella.transform.SetParent(hand.transform);
		umbrella.transform.localPosition = new Vector3(0,0,0);
		umbrella.transform.localRotation = Quaternion.Euler(0,0,0);
		other.gameObject.GetComponent<Thief>().run();
	}

}
