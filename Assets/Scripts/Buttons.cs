using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
	[SerializeField]
	GameObject credits;

	public void ExitGame()
	{
		Application.Quit();
	}

	public void Credits()
	{
		if (credits.activeInHierarchy) credits.SetActive(false);
		else
		{
			credits.SetActive(true);
		}
	}
}