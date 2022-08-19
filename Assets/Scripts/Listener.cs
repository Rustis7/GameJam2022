using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Listener : MonoBehaviour
{
	private GameObject menu;
	public void Awake()
	{
		menu = GameObject.Find("Menu");
	}
	public void Update()
	{
		if (Input.GetKeyDown("escape"))
		{
			if (menu.activeInHierarchy) menu.SetActive(false);
			else { menu.SetActive(true);	}
		}
	}
}
