using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour {

	private CanvasGroup fadeGroup;
	private float loadTime;
	private float minLogoTime = 1.5f; //min time loaded

	// Use this for initialization
	private void Start () {
		//only one canvasGroup
		fadeGroup = FindObjectOfType<CanvasGroup>();

		//start off with white screen

		fadeGroup.alpha = 1;

		//preload the game
		//$$

		//timestamp of comp time 
		//gives bufer time to loadtime

		if (Time.time < minLogoTime)
			loadTime = minLogoTime;
		else
			loadTime = Time.time;


	}
	
	// Update is called once per frame
	private void Update () {
		//fade in
		if (Time.time < minLogoTime) {
			fadeGroup.alpha = 1 - Time.time;
		}
			
		//fade out
		if (Time.time > minLogoTime && loadTime != 0)
		{
			fadeGroup.alpha = Time.time - minLogoTime;
			if (fadeGroup.alpha >= 1) {
				//set fade to zero completely
				SceneManager.LoadScene ("Menu");
			}
		}
	}
}
