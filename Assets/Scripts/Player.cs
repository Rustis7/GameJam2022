using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	[SerializeField]
	private float health = 100f;
	[SerializeField]
	private float multiplier = 1f;
	[SerializeField]
	private float healStrength = 50f;
	[SerializeField]
	private float fadeMultiplier = 1f;
	[SerializeField]
	private AudioClip candySound;
	[SerializeField]
	private AudioClip deathSound;
	[SerializeField]
	private AudioClip checkpointSound;

	private float wetness = 0f;
	private float alpha = 0f;
	private bool wet = false;
	private bool e = false;
	private bool dead = false;

	private GameObject[] healItems;
	private GameObject menu;
	private Image deathOverlay;
	private Transform checkpoint;
	private AudioSource audioSrc;
	private Animator anim;

	private void Awake()
	{
		anim = gameObject.GetComponentInChildren<Animator>();
		healItems = GameObject.FindGameObjectsWithTag("Heal");
		deathOverlay = GameObject.Find("DeathOverlay").GetComponent<Image>();
		menu = GameObject.Find("Menu");
		menu.SetActive(false);
		audioSrc = gameObject.AddComponent<AudioSource>();
		audioSrc.loop = false;
	}

	void Update()
	{
		health = Mathf.Clamp(health, 0f, 100f);
		wetness = Mathf.Clamp(wetness, 0f, 100f);
		if (wet) health -= Time.deltaTime * 10f * multiplier;
		if (health <= 0) Death();
		if (Input.GetKeyDown(KeyCode.E)) e = true;
		alpha = Mathf.Clamp01(alpha);
		alpha += Time.deltaTime * fadeMultiplier * (dead ? 1f : -1f);
		deathOverlay.color = new Color(0, 0, 0, alpha);
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			menu.SetActive(!menu.activeInHierarchy);
		}
	}

	private void FixedUpdate()
	{
		Rain();
	}

	void Rain()
	{
		int layerMask = 1 << 3;
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity, layerMask))
		{
			wet = false;
			wetness -= Time.deltaTime * 30f;
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.yellow);
			return;
		}
		wet = true;
		wetness += Time.deltaTime * 30f;
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 1000, Color.white);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Checkpoint") || other.gameObject.CompareTag("Heal"))
		{
			other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
		}
		if (other.gameObject.CompareTag("Trap")) health = 0;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Checkpoint") && e == true) {
			checkpoint = other.gameObject.transform;
			Heal(100);
			playSound(checkpointSound);
		}
		else if (other.gameObject.CompareTag("Heal") && e == true)
		{
			other.transform.GetChild(0).gameObject.SetActive(false);
			other.gameObject.SetActive(false);
			Heal(healStrength);
			playSound(candySound);
		}
		if(e) e=!e;
	}

	private void OnTriggerExit(Collider other)
	{
        if (other.gameObject.CompareTag("Checkpoint") || other.gameObject.CompareTag("Heal")) other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
	}

	void Death()
	{
		if(!dead) {
			playSound(deathSound);
			anim.SetTrigger("die");
		}
		dead = true;
		if (alpha >= 1)
		{
			Respawn();
			ReloadHealthItems();
			//Restore player model and animations
			dead = false;
		}
	}

	public void Respawn()
	{
		if(checkpoint != null) transform.position = checkpoint.position;
		health = 100;
	}

	public void ReloadHealthItems()
	{
		foreach (GameObject obj in healItems)
		{
			obj.SetActive(true);
		}
	}

	public void Heal(float value)
	{
		health += value;
	}

	public void SetMultiplier(float value)
	{
		multiplier = value;
	}

	private void playSound(AudioClip clip) {
		float maxPitch = 1.25f;
		float minPitch = 0.75f;
		if(clip == null) return;
		audioSrc.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
		audioSrc.PlayOneShot(clip);
	}

	public bool isDead() {
		return dead;
	}

}
