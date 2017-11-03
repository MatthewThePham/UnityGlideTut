using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour {
	private CanvasGroup fadeGroup;
	private float fadeInDuration =1;
	private bool gameStarted;

	private void Start(){
		//gets the only Ccanvas Group in the scene
		fadeGroup = FindObjectOfType<CanvasGroup> ();
		//set fade to full opacity
		fadeGroup.alpha =1;
	}

	private void Update(){
		if (Time.timeSinceLevelLoad <= fadeInDuration) {
		// initial fade in
			fadeGroup.alpha= 1- (Time.timeSinceLevelLoad/fadeInDuration);
		}
		//If the initial fade-in is completed, and the game has not been started yet
		else if(!gameStarted){
			//ensure that the fade group is completely alpha
			fadeGroup.alpha = 0;
			gameStarted = true;
		}

	}

	public void CompleteLeve(){
		//complete the level and save the progress
		SaveManager.Instance.CompleteLevel(Manager.Instance.currentLevel);

		//Focus the level selection when we return to the menu scene
		Manager.Instance.menuFocus=1;

		ExitScene ();
	}

	public void ExitScene(){
		SceneManager.LoadScene ("Menu");

	}
}
