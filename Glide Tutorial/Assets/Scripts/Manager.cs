using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
	//allow access from everywhere
	public static Manager Instance {set;get;}

	private void Awake(){
		DontDestroyOnLoad (gameObject);
		Instance = this;
	}

	//used when chagning from menu to the game scenes
	public int currentLevel=0;
	// used when entering the menu scene, looks at level select right away
	public int menuFocus=0;

}
