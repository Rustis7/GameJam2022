using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    bool wet = false;
    [SerializeField]
    float wetness = 0;
    [SerializeField]
    float health = 100f;
    [SerializeField]
    private float multiplier = 1;
    [SerializeField]
    private Transform checkpoint;
    [SerializeField]
    private bool e = false;
    [SerializeField]
    private GameObject[] healItems;
    [SerializeField]
    Image deathOverlay;

	private void Awake()
	{
		healItems = GameObject.FindGameObjectsWithTag("Heal");
        deathOverlay = GameObject.Find("DeathOverlay").GetComponent<Image>();
    }

	void Update()
    {
        health = Mathf.Clamp(health, 0f, 100f);
        wetness = Mathf.Clamp(wetness, 0f, 100f);
        if (wet) health -= Time.deltaTime * 10f * multiplier;
        if (health <= 0) Death();

        if (Input.GetKey(KeyCode.E)) e = true;
        else e = false;
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
        Debug.Log("Enter");
        if (other.gameObject.CompareTag("Checkpoint") || other.gameObject.CompareTag("Heal"))
		{
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
		}
    }

	private void OnTriggerStay(Collider other)
	{
        if (other.gameObject.CompareTag("Checkpoint") && e == true) checkpoint = other.gameObject.transform;
        else if (other.gameObject.CompareTag("Heal") && e == true)
        {
            other.transform.GetChild(0).gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            health += 50;
        }
    }
	private void OnTriggerExit(Collider other)
	{
        other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
	}

	void Death()
    {
        Debug.Log("Player died");
        //Play death sound
        //Play death animation
        //Do a blackout
        deathOverlay.color = new Color(0, 0, 0, 255);
        Respawn();
        ReloadHealthItems();
        //Fade in
        //deathOverlay.color = new Color(0, 0, 0, 0);
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

    public void Heal(int value)
    {
        health += value;
    }

    public void SetMultiplier(float value)
	{
        multiplier = value;
	}
}
