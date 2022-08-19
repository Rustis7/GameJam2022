using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
	[SerializeField]
	GameObject options;

	[SerializeField]
	GameObject credits;

	public void ExitGame()
	{
		Application.Quit();
	}

	public void Options()
	{
		if (options.activeInHierarchy) options.SetActive(false);
		else
		{
			credits.SetActive(false);
			options.SetActive(true);
		}
	}

	public void Credits()
	{
		if (credits.activeInHierarchy) credits.SetActive(false);
		else
		{
			options.SetActive(false);
			credits.SetActive(true);
		}
	}
}