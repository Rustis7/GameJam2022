using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    bool wet = false;

    [SerializeField]
    float health = 100f;

    [SerializeField]
    private float multiplier = 1;

    void Update()
    {
        health = Mathf.Clamp(health, 0f, 100f);
        if (wet) health -= Time.deltaTime * 10f * multiplier;
        if (health <= 0) Death();
    }

	private void FixedUpdate()
	{
        int layerMask = 1 << 3;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity, layerMask))
		{
            wet = false;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.yellow);
		}
        else
		{
            wet = true;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 1000, Color.white);
		}
	}

	void Death()
    {
        Debug.Log("Player died");
        //Respawn
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
