using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    float health = 100f;
    [SerializeField]
    float multiplier = 1f;
    [SerializeField]
    float healStrength = 50f;
    [SerializeField]
    float fadeMultiplier = 1f;

    float wetness = 0f;
    float alpha = 0f;
    bool wet = false;
    bool e = false;
    bool dead = false;

    GameObject[] healItems;
    GameObject menu;
    Image deathOverlay;
    Transform checkpoint;

	private void Awake()
	{
		healItems = GameObject.FindGameObjectsWithTag("Heal");
        deathOverlay = GameObject.Find("DeathOverlay").GetComponent<Image>();
        menu = GameObject.Find("Menu");
        menu.SetActive(false);
    }

	void Update()
    {
        health = Mathf.Clamp(health, 0f, 100f);
        wetness = Mathf.Clamp(wetness, 0f, 100f);
        if (wet) health -= Time.deltaTime * 10f * multiplier;
        if (health <= 0) Death();
        e = Input.GetKey(KeyCode.E) ? true : false;
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
        }
        else
        {
            wet = true;
            wetness += Time.deltaTime * 30f;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 1000, Color.white);
        }
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.CompareTag("Checkpoint") || other.gameObject.CompareTag("Heal"))
		{
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
		}
    }

	private void OnTriggerStay(Collider other)
	{
        if (other.gameObject.CompareTag("Checkpoint") && e == true)
        {
            checkpoint = other.gameObject.transform;
            Heal(100);
        }
        else if (other.gameObject.CompareTag("Heal") && e == true)
        {
            other.transform.GetChild(0).gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            Heal(healStrength);
        }
    }
	private void OnTriggerExit(Collider other)
	{
        if (other.gameObject.CompareTag("Checkpoint") || other.gameObject.CompareTag("Heal")) other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
	}

	void Death()
    {
        //Play death sound
        //Play death animation
        dead = true;
        if (alpha >= 1)
        {
            Respawn();
            ReloadHealthItems();
            //Restore player model and animations
            dead = false;
        }
        //Resume gameplay
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
}
