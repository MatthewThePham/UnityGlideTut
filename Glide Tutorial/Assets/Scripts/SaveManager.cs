using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {
	public static SaveManager Instance { set; get;}
	public SaveSlate state;

	private void Awake(){
		
		//resets every game
		//ResetSave();

		//keeps the data of trails and colors
		DontDestroyOnLoad (gameObject);
		Instance=this;
		Load ();

		/*
		Debug.Log (state.colorOwned); //16 bits 		0000 0000 0000 0000 =0
		UnlockColor (0);
		Debug.Log (state.colorOwned);///16 bits 		0000 0000 0000 0001 =1
		UnlockColor (1);
		Debug.Log (state.colorOwned);///16 bits 		0000 0000 0000 0011 =3
		UnlockColor (2);
		*/

		Debug.Log (Helper.Serialize<SaveSlate> (state));
	}

	//Save the whole state of this saveState script to player pref instead of a seperate save file
	public void Save(){
		PlayerPrefs.SetString("save",Helper.Serialize<SaveSlate>(state));
	}

	// Load the previous saved state from the player prefs
	public void Load(){
		//checks if already there
		if(PlayerPrefs.HasKey("save")){
			state = Helper.Deserialize<SaveSlate> (PlayerPrefs.GetString ("save"));
		}
		else{
			state=new SaveSlate();
			Save();
			Debug.Log("No Save file found,creating a new one");
		}
	}

	//checks color if owned
	public bool IsColorOwned(int index){
		//checks if the bit is set, if so the color is owned
		return(state.colorOwned & (1 << index)) !=0;
	}

	//checks trail if owned
	public bool IsTrailOwned(int index){
		//checks if the bit is set, if so the color is owned
		return(state.trailOwned & (1 << index)) !=0;
	}

	//attempt to buy color returnl boolean value
	public bool BuyColor(int index, int cost){
		if (state.gold >= cost) {
			state.gold -= cost;
			UnlockColor (index);

			Save ();

			return true;
		} else {
			//not enough money
			return false;
		}
	}

	public bool BuyTrail(int index, int cost){
		if (state.gold >= cost) {
			state.gold -= cost;
			UnlockTrail (index);

			Save ();

			return true;
		} else {
			//not enough money
			return false;
		}
	}


	//unlock color in "colorOwned" int
	public void UnlockColor(int index){
		//Toggle on the bit at index
		state.colorOwned |=1 <<index;
		//toggle off bit state.colorOwned ^=1 <<index;
	}

	//unlock trail in "trailOwned" int
	public void UnlockTrail(int index){
		//Toggle on the bit at index
		state.trailOwned |=1 <<index;
		//toggle off bit state.colorOwned ^=1 <<index;
	}

	//complete level
	public void CompleteLevel(int index){
		//if this is the current activve level
		if(state.completedLevel==index){
			state.completedLevel++;
			Save ();
		}

	}

	//resets save file
	public void ResetSave(){
		PlayerPrefs.DeleteKey ("save");
	}
}
