using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScreen : MonoBehaviour {
	private CanvasGroup fadeGroup;
	//private float fadeInspeed= 0.33f; //three seconds
	private float fadeInspeed= 0.11f;

	public Transform colorPanel;
	public Transform trailPanel;

	private Texture previousTrail;
	private GameObject lastPreviewObject;

	public Transform trailPreviewObject;
	public RenderTexture trailPreviewTexture;

	public RectTransform menuContainer;
	public Transform LevelPanel;

	private GameObject currentTrail;

	private Vector3 desiredMenuPosition;

	public Text colorBuySetText;
	public Text trailBuySetText;
	public Text goldText;

	private MenuCamera menuCam;

	private int[] colorCost = new int[] {0,5,5,10,10,10,10,15,15,10};
	private int[] trailCost = new int[] {0,20,40,40,60,60,80,85,115,120};
	private int selectedColorIndex;
	private int selectedTrailIndex;
	private int activeColorIndex;
	private int activeTrailIndex;

	public AnimationCurve enteringLevelZoomCurve;
	private bool isEnteringLevel = false;
	private float zoomDuration = 3.0f;
	private float zoomTransition;

	// Use this for initialization
	private void Start () {
		//Find only menuCamera and assign it
		menuCam=FindObjectOfType<MenuCamera>();

		//TEMPORARY ASSIGNMENT OF GOLD
		SaveManager.Instance.state.gold=999;


		//Position our camera on the focused menu
		SetCameraTo(Manager.Instance.menuFocus);

		//shows amount of gold
		UpdateGoldText();

		//grabs the only Canvas group in the scene
		fadeGroup=FindObjectOfType<CanvasGroup>();

		//starts off white screen
		fadeGroup.alpha =1;
		InitShop ();

		//Add buttons on-click events to level
		InitLevel();

		//set player's prefereces for both color and trail
		OnColorSelect(SaveManager.Instance.state.activeColor);
		SetColor(SaveManager.Instance.state.activeColor);

		OnTrailSelect(SaveManager.Instance.state.activeTrail);
		SetTrail(SaveManager.Instance.state.activeTrail);

		//makes butt bigger for selected items
		colorPanel.GetChild(SaveManager.Instance.state.activeColor).GetComponent<RectTransform>().localScale = Vector3.one *1.125f;
		trailPanel.GetChild(SaveManager.Instance.state.activeTrail).GetComponent<RectTransform>().localScale = Vector3.one *1.125f;
			
	}
	
	// Update is called once per frame
	private void Update () {
		//fade In
		fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInspeed;
		//menu nav
		menuContainer.anchoredPosition3D=Vector3.Lerp(menuContainer.anchoredPosition3D,desiredMenuPosition,0.1f);

		//entering level zoom
		if(isEnteringLevel){
			//add tp tje zpp,Tramsotopm f;pat
			zoomTransition +=(1/zoomDuration)* Time.deltaTime;
			//change the scalefollowing the animation curve
			menuContainer.localScale =Vector3.Lerp(Vector3.one,Vector3.one *5, enteringLevelZoomCurve.Evaluate(zoomTransition));

			//change the desired position of the canvas so it can hollow the scale out
			//zooms in the center 
			Vector3 newDesiredPosition= desiredMenuPosition *5;
			//This adds to the specific position of the level on the canvas
			RectTransform rt= LevelPanel.GetChild(Manager.Instance.currentLevel).GetComponent<RectTransform>();
			newDesiredPosition -= rt.anchoredPosition3D * 5;


			//This line will override the previous position update
			menuContainer.anchoredPosition3D=Vector3.Lerp(desiredMenuPosition,newDesiredPosition,enteringLevelZoomCurve.Evaluate(zoomTransition));

			//fade to white screen, override the first line of update
			fadeGroup.alpha=zoomTransition;


			//loads the level when animation is done
			if (zoomTransition >=1 ) {
				//Enter the level
				SceneManager.LoadScene("Game");
			}
		}
	}

	//on click events to shop buttons
	private void InitShop()
	{
		//assign references
		if (colorPanel == null || trailPanel == null)
			Debug.Log ("Need assignment color/trail panael in inspector");

		//For every childern transform under color panel, get butt and add onclicks
		int i=0;
		foreach (Transform t in colorPanel)
		{
			int currentIndex = i;

			Button b = t.GetComponent<Button> ();
			b.onClick.AddListener (() => OnColorSelect (currentIndex));

			//Set color of the image, based on if owned or not
			Image img=t.GetComponent<Image>();
			img.color = SaveManager.Instance.IsColorOwned (i) 
				? Manager.Instance.playerColor[currentIndex] 
				: Color.Lerp(Manager.Instance.playerColor[currentIndex], new Color(0,0,0,1),0.25f);
			//slightly darker background for new color.

			i++;
		}
		//reset
		i = 0;
		//Do the same for the trail panel
		foreach (Transform t in trailPanel)
		{
			int currentIndex = i;

			Button b = t.GetComponent<Button> ();
			b.onClick.AddListener (() => OnTrailSelect (currentIndex));
			i++;

			//Set trail of the image, based on if owned or not
			RawImage img=t.GetComponent<RawImage>();
			img.color = SaveManager.Instance.IsTrailOwned (i) ? Color.white : new Color (0.7f, 0.7f, 0.7f);
		}
		//intilize and set the previous trail to prevent bug swapping
		previousTrail = trailPanel.GetChild(SaveManager.Instance.state.activeTrail).GetComponent<RawImage>().texture;
	}
		


	private void InitLevel(){
		//assign reference
		if (LevelPanel == null) {
			Debug.Log ("Did not assign the level in inspector");
		}
			int i = 0;
			foreach (Transform t in LevelPanel) {
				int currentIndex = i;

				Button b = t.GetComponent<Button> ();
				b.onClick.AddListener(() => OnLevelSelect(currentIndex));
				
			Image img = t.GetComponent<Image> ();

			//Is it unlocked?
			if (i <= SaveManager.Instance.state.completedLevel) {
				
				//it is unlocked
				if (i == SaveManager.Instance.state.completedLevel) {
					//It is not completed
					img.color = Color.white;
				} else {
					//Level is already completed
					img.color = Color.green;
				}

			} else {
				//Level is not unlocked! Disable the button
				b.interactable=false;

				//set to a dark color
				img.color=Color.gray;
			}

				i++;
			}
	}


	private void SetCameraTo(int menuIndex){
		NavigateTo (menuIndex);
		menuContainer.anchoredPosition3D = desiredMenuPosition;
	}


	private void NavigateTo(int menuIndex){
		switch(menuIndex){
		default:
		//0 and default case is main menu
		case 0:
			desiredMenuPosition = Vector3.zero;
			menuCam.BackToMainMenu ();
			break;
		case 1:
			desiredMenuPosition = Vector3.right * 1280;
			menuCam.MoveToLevel ();
			break;
		case 2:
			desiredMenuPosition = Vector3.left * 1280;
			menuCam.MoveToShop();
			break;
		}
	}

	private void SetColor(int index){
	//sets active index
	activeColorIndex=index;
	SaveManager.Instance.state.activeColor = index;

	//change color of model
	Manager.Instance.playerMaterial.color=Manager.Instance.playerColor[index];
	

	//change buy/set button text
		colorBuySetText.text="Current";

	//saves preferences
	SaveManager.Instance.Save ();

	}

	private void SetTrail(int index){
		//sets active index
		activeTrailIndex=index;
		SaveManager.Instance.state.activeTrail = index;

		//change color of model
		if (currentTrail != null)
			Destroy(currentTrail);

		//create the new trail and cast to a Game Object
		currentTrail = Instantiate(Manager.Instance.playerTrails[index]) as GameObject;

		//set it as childern of player
		//currentTrail.transform.SetParent(FindObjectOfType<MenuPlayer>().transform);
		currentTrail.transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);

		//fix scaling and rotation
		currentTrail.transform.localPosition = Vector3.zero;
		currentTrail.transform.localRotation = Quaternion.Euler (0, 0, 90);
		currentTrail.transform.localScale = Vector3.one / 100f ;

		//change buy/set button text
		trailBuySetText.text="Current";

		//saves preferences
		SaveManager.Instance.Save ();
	}

	private void UpdateGoldText(){
		goldText.text = SaveManager.Instance.state.gold.ToString ();
	
	}


	///Buttons
	public void OnPlayClick(){
		NavigateTo (1);
		Debug.Log ("Play button has been clicked");
	}

	public void OnShopClick(){
		NavigateTo (2);
		Debug.Log("Shop button has been clicked");
	}
	public void OnBackClick(){
		NavigateTo (0);
		Debug.Log("Back Button has been clicked");
	}
	private void OnLevelSelect(int currentIndex){
		Manager.Instance.currentLevel = currentIndex;
		// Removed out to add cool animation effect
		//SceneManager.LoadScene ("Game");
		isEnteringLevel=true;
		Debug.Log ("Selecting level :" + currentIndex);
	}
	private void OnColorSelect(int currentIndex){
		Debug.Log("selecting color butt" +currentIndex);

		//if button click selected already, exit
		if (selectedColorIndex == currentIndex)
			return;
		
		//make the icon bigger
		colorPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale=Vector3.one *1.125f;
		//put the previous one on normal scale
		colorPanel.GetChild(selectedColorIndex).GetComponent<RectTransform>().localScale=Vector3.one;

		//set the selected color
		selectedColorIndex=currentIndex;

		//change the content of buy/set buttton depending on the state of the color
		if(SaveManager.Instance.IsColorOwned(currentIndex)){
			//color is owned
			//It is already our current color
			if (activeColorIndex == currentIndex) {
				colorBuySetText.text = "Current";

			} else {
				colorBuySetText.text = "Select";
			}
		}
		else{
			//color not owned
			colorBuySetText.text="Buy: " + colorCost[currentIndex].ToString();
		}
	}
	private void OnTrailSelect(int currentIndex){
		Debug.Log("selecting trail butt" +currentIndex);

		//if button click selected already, exit
		if (selectedTrailIndex == currentIndex)
			return;
		


		//preview trail and get image of preview button
		trailPanel.GetChild(selectedTrailIndex).GetComponent<RawImage>().texture=previousTrail;
		//keeps new trail's preview image in previous trail as a backup
		previousTrail= trailPanel.GetChild(currentIndex).GetComponent<RawImage>().texture;
		//Sets new trail preview to other camera
		trailPanel.GetChild(currentIndex).GetComponent<RawImage>().texture = trailPreviewTexture;

		if (lastPreviewObject != null)
			Destroy (lastPreviewObject);

		//makes trail preview 
		lastPreviewObject = GameObject.Instantiate(Manager.Instance.playerTrails[currentIndex]) as GameObject;
		lastPreviewObject.transform.SetParent (trailPreviewObject);
		lastPreviewObject.transform.localPosition = Vector3.zero;

		//make the icon bigger
		trailPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale=Vector3.one *1.125f;
		//put the previous one on normal scale
		trailPanel.GetChild(selectedTrailIndex).GetComponent<RectTransform>().localScale=Vector3.one;

		//sets the selected color
		selectedTrailIndex=currentIndex;

		//change the content of buy/set buttton depending on the state of the color
		if (SaveManager.Instance.IsTrailOwned (currentIndex)) {
			
			//trail is owned
			//It is already our current color
			if (activeTrailIndex == currentIndex) {
				trailBuySetText.text = "Current";

			} else {
				trailBuySetText.text = "Select";
			}
		} else {
			//color not owned
			trailBuySetText.text = "Buy: " + trailCost [currentIndex].ToString ();
		}
	}

	//public as used by inspector
	public void OnColorBuySet(){
		Debug.Log("Buy/set color");

		// Is the selected color owned
		if (SaveManager.Instance.IsColorOwned (selectedColorIndex)) {
			//set the color
			SetColor (selectedColorIndex);
		} else {
			//attempt to buy the color
			if (SaveManager.Instance.BuyColor (selectedColorIndex, colorCost [selectedColorIndex])) {
				//Buys color successful
				SetColor(selectedColorIndex);

				//Change color of button 
				colorPanel.GetChild(selectedColorIndex).GetComponent<Image>().color = Manager.Instance.playerColor[selectedColorIndex];
				//.25f slightly darker background for new colorm, from previous color to new color
				//update gold text
				UpdateGoldText();

			} else {
			//not enough gold
				Debug.Log("Not enough gold");
			}
	}
	}

	public void OnTrailBuySet(){
		Debug.Log("Buy/Set trail");
		// Is the selected trail owned
		if (SaveManager.Instance.IsTrailOwned (selectedTrailIndex)) {
			//set the color
			SetTrail(selectedTrailIndex);
		} else {
				if(SaveManager.Instance.BuyTrail(selectedTrailIndex, trailCost [selectedTrailIndex])){
				//buys the trail successful
					SetTrail(selectedTrailIndex);

				//Change color of button
				trailPanel.GetChild(selectedTrailIndex).GetComponent<RawImage>().color = Color.white;

				//update gold text
				UpdateGoldText();
				}
				else{
			//attempt to buy the color
			Debug.Log("No GOld");
				}
		}

	}
}
