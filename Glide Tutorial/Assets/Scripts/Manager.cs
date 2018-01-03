using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
	//allow access from everywhere
	public static Manager Instance {set;get;}

	public Material playerMaterial;
	public Color[] playerColor = new Color[10];
	public GameObject[] playerTrails = new GameObject[10];



	private void Awake(){
		DontDestroyOnLoad (gameObject);
		Instance = this;
	}

	//used when chagning from menu to the game scenes
	public int currentLevel=0;
	// used when entering the menu scene, looks at level select right away
	public int menuFocus=0;

}
