using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class script_player_controls : MonoBehaviour {

	CharacterController controller;

	public bool FPVModeOn = false;
	public bool controlsLocked = false;

	public GameObject[] worldBoundaries;
	public float fieldOfViewAngle = 70f;
	public float camera_panSpeed = 30f;
	public float camera_zoomSpeed = 30f;
	public GameObject mainCamera;
	public Transform shipTransform;
	//public GameObject destinationSettlement;
	
	public float shipSpeed_Game = 3f;
	public float shipSpeed_Actual = 7.408f; //km/h
	public float shipSpeed_Game_Modifier = .75f;
	public float shipSpeed_CrewModifier = 0f;
	public float shipSpeed_ShipHPModifier = 0f;
	public float shipSpeed_HungerModifier = 0f;
	public float shipSpeed_ThirstModifier = 0f;
	
	public float dailyFoodKG = .83f; //(NASA)
	public float dailyWaterKG = 5f; //based on nasa estimates of liters(kg) for astronauts--sailing is more physically intensive so I've upped it to 5 liters
	
	globalVariables MGV;
	public CharacterController cameraController;
	
	public Ship ship;
	public PlayerJourneyLog journey;
	public Vector3 lastPlayerShipPosition;
	public Vector3 travel_lastOrigin;
	public Vector3 currentDestination;
	
	public float numOfDaysTraveled = 0;
	public float totalDis = 0;
	public Vector3 originOfTrip;
	public bool travelingToSettlement = false;
	public bool allowPlayerSelectButton = false;
	
	public Vector3 currentWaterDirectionVector = Vector3.zero;
	public Vector3 currentWindDirectionVector = Vector3.zero;
	public Vector3 playerMovementVector = Vector3.zero;
	public float playerMovementMagnitude = 0f;
	public float currentWaterDirectionMagnitude = 0f; 
	public float currentWindDirectionMagnitude = 0f;
	public float current_shipSpeed_Magnitude = 0f;
	
	public bool getSettlementDockButtonReady = false;
	
	public float numOfDaysWithoutFood = 0;
	public float numOfDaysWithoutWater = 0;
	
	public int dayCounterStarving = 0;
	public int dayCounterThirsty = 0;
	
	public bool rollForThirstDeath = false;
	public bool rollForHungerDeath = false;
	
	public bool notEnoughSpeedToMove = false;
	
	float initialAngle = 0f;
	float initialCelestialAngle = 0f;
	float targetAngle = 0f;
	
	public float testPrecessionAngle = 0f;
	public double test_numOfJulianCenturies;
	public float test_processionSpeed;
	public float test_angle_offset = 0f; //This is to make sure the elliptic disc stays still while the actual stars rotate
	
	public bool isPerformingRandomEvent = false;

	public Material cursorRingMaterial;
	public GameObject cursorRing;
	public bool cursorRingIsGreen = true;
	public float cursorRingAnimationClock = 0f;
	public bool cursorRingIsGrowing = true;
	
	
	public GameObject fogWall;
	
	public bool shipTravelStartRotationFinished = false;
	public float shipTravelStartRotationLerp = .1f;
	public Quaternion shipTravelStartRotationQuat;
	public bool shipTravelQuatIsSet = false;
	
	
	public bool rayCheck_stopShip = false;
	public bool rayCheck_stopCurrents = false;
	public bool rayCheck_playBirdSong = false;
	
	public AudioSource SFX_birdsong;
	
	
	
	// Use this for initialization
	void Start () {
	MGV = GameObject.FindGameObjectWithTag("global_variables").GetComponent<globalVariables>();
	controller = gameObject.GetComponent<CharacterController>();
	cameraController = MGV.mainCamera.GetComponent<CharacterController>();
	shipTransform = transform.GetChild(0);
	ship = new Ship("Argo",7.408f,100,500f);
	ship.networkID = 1;
	journey = new PlayerJourneyLog();
	lastPlayerShipPosition = transform.position;
	ship.mainQuest = MGV.LoadMainQuestLine();
	
	//Now teleport the player ship to an appropriate location near the first target
	transform.position = new Vector3(1702.414f,transform.position.y,2168.358f);
		
	//Setup the day/night cycle
	UpdateDayNightCycle(MGV.IS_NEW_GAME);

	
	//TODO temp fix to add 10 crew members
	//for (int i = 0; i < 10; i++){
	//	ship.crewRoster.Add(MGV.GenerateRandomCrewMember());
	//}
	
	//Setup the Cursor Ring Material for Animation
	cursorRingMaterial = cursorRing.GetComponent<MeshRenderer>().sharedMaterial;
	
	//initiate the main questline
	 //MGV.InitiateMainQuestLineForPlayer();
	 
	//initialize players ghost route
	UpdatePlayerGhostRouteLineRenderer(MGV.IS_NEW_GAME);
	
	//DEBUG
	MGV.DEBUG_currentQuestLeg = ship.mainQuest.currentQuestSegment;
	}
	
	// Update is called once per frame
	void Update () {
		//DEBUG
		//DEBUG
		MGV.DEBUG_currentQuestLeg = ship.mainQuest.currentQuestSegment;
		if( MGV.DEBUG_currentQuestLegIncrease){
			ship.mainQuest.currentQuestSegment++;
			MGV.DEBUG_currentQuestLegIncrease = false;
		}
		
		
		//Update the size of the crew every update
		ship.crew = ship.crewRoster.Count;
		
		//Make sure the camera transform is always tied to the front of the ship model's transform if the FPV camera is enabled
		if (MGV.FPVCamera.activeSelf)
			MGV.FPVCamera.transform.parent.parent.position = shipTransform.TransformPoint(new Vector3(shipTransform.localPosition.x, .31f, shipTransform.localPosition.z+.182f));
		
		//Make sure horizon sky is always at player position
		MGV.skybox_horizonColor.transform.position = transform.position;
		
		//Make sure our celestial spheres are always tied to the position of the player ship
		//	--We are running a "Mariner-centric" universe and calculating the visual effects of the universe around the ship
		MGV.skybox_MAIN_CELESTIAL_SPHERE.transform.position = transform.position;
		//Make sure the player's latitude determines the angle of the north pole
		RotateCelestialSky();
		//Update the earth's precession
		CalculatePrecessionOfEarth();
		//Always check the ships coast line check ray casts every update
		//DetectCoastLinesWithRayCasts();
		//Always Update the current Speed before the logic below
		UpdateShipSpeed();
		
		//check for bird song
		if (rayCheck_playBirdSong) SFX_birdsong.enabled = true;
		else SFX_birdsong.enabled = false;
		
		
		
	//TODO: need to update all references to controlsLocked to the MGV.controlsLocked
	//controlsLocked = MGV.controlsLocked;
		//If NOT Game Over then go with the regular logic
		if (!MGV.isGameOver){
			//if the controls are not locked--we are anchored
			if (!MGV.controlsLocked){
				//check to see if we just left a port starving
				if (MGV.justLeftPort){
					MGV.justLeftPort = false;
					CheckIfShipLeftPortStarvingOrThirsty();
					//TODO need to add a check here for notification windows to lock controls
				}
				//check for settlement highlights
				//MouseCursorCheckForInteractionTrigger();
				CheckForPlayerNavigationCursor();
				AnimateCursorRing();
				//check for panning screen
				CheckCameraRotationControls();
				//check for zooming in / out
				CheckZoomControls();
					
				//check for click on highlighted settlement
			
				//show the settlement docking button if in a docking area
				if(getSettlementDockButtonReady){
					getSettlementDockButtonReady = false;
					MGV.showSettlementTradeButton = true;
				}
	
			//Controls Are Locked--so we are traveling	or new gaming
			} else if(!MGV.isGameOver){
				//Check if we are starting a new game and are at the title screen
				if (MGV.isTitleScreen || MGV.isStartScreen){
					//If the player triggers the GUI button to start the game, stop the animation and switch the camera off
					if(MGV.startGameButton_isPressed){
						//Debug.Log ("Quest Seg start new game: " + ship.mainQuest.currentQuestSegment);
						//Turn off title screen camera
						MGV.camera_titleScreen.SetActive(false);
						MGV.bg_startScreen.SetActive(false);
						
						//Turn on the environment fog
						RenderSettings.fog = true;
		
						//Now turn on the main player controls camera
						MGV.FPVCamera.SetActive(true);
						
						//Turn on the player distance fog wall
						fogWall.SetActive(true);
						
						//Now change titleScreen to false
						MGV.isTitleScreen = false;
						MGV.isStartScreen = false;
						
						//Now enable the controls
						MGV.controlsLocked = false;
						
						//Initiate the main questline
						MGV.InitiateMainQuestLineForPlayer();
						
						//Reset Start Game Button
						MGV.startGameButton_isPressed = false;
						
					}
				
				//Else we are not at the title screen and just in the game
				} else {
					//Check if we're in the menus or not
					//	-If we aren't in the settlement menu then we know we're traveling
					if (!MGV.showSettlementTradeGUI){
						//If the ship is dead in the water--don't do anything
						if (shipSpeed_Actual != 0)
							TravelToSelectedTarget(currentDestination);
						else {
							MGV.controlsLocked = false;
							MGV.isGameOver = true;
							current_shipSpeed_Magnitude = 0f;
						}
						CheckCameraRotationControls();
					} else {
						//we are in the settlement menu GUI so just do nothing
					}
				}
			}
			
		//It's GAME OVER
		} else {
		
		
		}
		
	}
	
// DEPRECATED FUNCTION   **********************************************************
//	//--------------------------------------------------------------------------------------
//	//  this function processes the point in 3d space that the mouse cursor is hovering over
//	//		and sees if that point coincides with an object that is also an interactive object.
//	//		If it is an interaction object, it will tell that object to activate or 'highlight'
//	//--------------------------------------------------------------------------------------
//	public void MouseCursorCheckForInteractionTrigger(){
//		// WE NEED TO MAKE SURE THE MOUSE CURSOR IS ACTUALLY OVER THE NAVIGATION MAP BEFORE ANY OTHER ACTION IS PROCESSED
//		Vector3 main_mouse = MGV.FPVCamera.GetComponent<Camera>().ScreenToViewportPoint (Input.mousePosition);
//		if (mainCamera.GetComponent<Camera>().rect.Contains (main_mouse)){
//			//Debug.Log ("Mouse is in map view");
//							//Debug.Log ("Shooting Ray  " + (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,1)) - Camera.main.transform.position) * 500 );
//		//Debug.DrawRay(Camera.main.transform.position, (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,.05f)) - Camera.main.transform.position) * 500, Color.red);
//			RaycastHit hitInfo;
//			Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,1));
//			//first make sure that the mouse cursor isn't positioned over a GUI element
//			if (!MGV.mouseCursorUsingGUI){
//				if (Physics.Raycast(Camera.main.transform.position, (cursorPosition - Camera.main.transform.position), out hitInfo,500f)){
//					//Once we have the cursor position on the map, we need to check the line of sight between the point and the ship
//					//we'll draw a raycast from the ship to the point on the map
//					//first let's make sure we are drawing a line along the water surface so we'll take the cursor point and translate the y value to that of the ship
//					//Debug.Log (hitInfo.transform.name);
//					Vector3 target = new Vector3(hitInfo.point.x, .05f, hitInfo.point.z);
//					RaycastHit obstacle;
//					Vector3 origin = new Vector3(transform.position.x, .05f, transform.position.z);
//					if(Physics.Raycast (origin, target - origin, out obstacle, Vector3.Distance(origin, target))){
//					 //we hit an obstacle so let's draw a red line to the location and not respond to player's clicking left mouse button
//					 //let's also make sure the obstacle isn't a settlement--in which case, we're fine.
//						if (obstacle.transform.gameObject.tag != "settlement"){
//							LineRenderer line = gameObject.GetComponent<LineRenderer>();
//							line.SetPosition(0,transform.position);
//							line.SetPosition(1,new Vector3(target.x,target.y+.1f,target.z));
//							line.SetColors(Color.red, Color.magenta);
//							//Debug.Log (obstacle.transform.name);
//							allowPlayerSelectButton = false;
//						} else {
//						Debug.Log ("IS A SETTLEMENT");
//							LineRenderer line = gameObject.GetComponent<LineRenderer>();
//							line.SetPosition(0,transform.position);
//							line.SetPosition(1,new Vector3(target.x,target.y+.1f,target.z));
//							line.SetColors(Color.green, Color.blue);
//							//Enable the info GUI
//							MGV.showSettlementInfoGUI = true;
//							//let's turn on the trigger object's mesh renderer to highlight it the interactive color
//							//hitInfo.transform.transform.SendMessage("ActivateHighlightOnMouseOver", SendMessageOptions.DontRequireReceiver);
//							travelingToSettlement = true;
//							allowPlayerSelectButton = true;
//						}
//					//Otherwise--if we DON'T hit an obstacle check to see if we are clicking on a settlement
//					} else {
//						allowPlayerSelectButton = true;
//						//let's draw a line to target location
//						LineRenderer line = gameObject.GetComponent<LineRenderer>();
//						line.SetPosition(0,transform.position);
//						line.SetPosition(1,new Vector3(target.x,target.y+.05f,target.z));
//						line.SetColors(Color.green, Color.blue);
//						
//						// If the object the mouse cursor is over is part of layer 8 (settlements)
//						if (hitInfo.transform.gameObject.tag == "settlement"){
//							//Enable the info GUI
//							MGV.showSettlementInfoGUI = true;
//							//let's turn on the trigger object's mesh renderer to highlight it the interactive color
//							//hitInfo.transform.transform.SendMessage("ActivateHighlightOnMouseOver", SendMessageOptions.DontRequireReceiver);
//							travelingToSettlement = true;
//						} else {
//							//Disable the info GUI
//							MGV.showSettlementInfoGUI = false;
//							//Disable the selection ring
//							MGV.selection_ring.SetActive(false);
//							//Debug.Log (hitInfo.transform.tag);
//							travelingToSettlement = false;
//						}
//		
//					}
//					//now let's see if the mouse button was clicked, and if so: start the ship traveling toward the destination
//					if (Input.GetButton("Select") && allowPlayerSelectButton){
//						//disable the info GUI
//						MGV.showSettlementInfoGUI = false;
//						//lock controls so that the travel function is triggered on the next update cycle
//						MGV.controlsLocked = true; 
//						//set the destination: using the players Y value so the ship always stays at a set elevation
//						currentDestination = new Vector3(target.x, transform.position.y, target.z);
//						//set the player ship's current position to be logged into the journey log
//						lastPlayerShipPosition = transform.position;
//						travel_lastOrigin = transform.position;
//						numOfDaysTraveled = 0;
//					}
//				}
//			} else {
//				LineRenderer line = gameObject.GetComponent<LineRenderer>();
//				line.SetPosition(0,Vector3.zero);
//				line.SetPosition(1,Vector3.zero);
//				line.SetColors(Color.green, Color.blue);
//			}
//		}
//	}
	
	public void CheckForPlayerNavigationCursor(){
	
		Vector3 main_mouse = MGV.FPVCamera.GetComponent<Camera>().ScreenToViewportPoint (Input.mousePosition);
		
		//Here we are first checking to see if the mouse cursor is over the actual gameplay window
		Rect FPVCamRect = MGV.FPVCamera.GetComponent<Camera>().rect;
		FPVCamRect.y = 0;
		FPVCamRect.height =1f;
		if (FPVCamRect.Contains (main_mouse)){
			//If the mouse cursor is hovering over the allowed gameplay window, then figure out the position of the mouse in worldspace
			RaycastHit hitInfo;
			Ray ray = MGV.FPVCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
			
				if (Physics.Raycast(ray.origin, ray.direction, out hitInfo,100f)){
					//if we get a hit, then turn the cursor ring on
					cursorRing.SetActive(true);
					//Move the animated cursor ring to the position of the mouse cursor
					cursorRing.transform.position = hitInfo.point;// + new Vector3(0,.03f,0);
					
					//Adjust the scale of the cursor ring to grow with distance
					float newScale = Vector3.Distance(cursorRing.transform.position , MGV.FPVCamera.transform.position) * .09f;
					cursorRing.transform.localScale = new Vector3(newScale, newScale, newScale);
					
					//Hide the cursor if it's touching the player ship to avoid weird visual glitches
					if (hitInfo.collider.tag == "playerShip") cursorRing.SetActive(false);
					else cursorRing.SetActive(true);
					
					//Now let's check for line of site to the target
					bool hasLineOfSight = true;
					RaycastHit lineOfSightHit;
					Vector3 lineOfSightOrigin = shipTransform.position + new Vector3(0,.02f,0);
					//We Fire a raycast in the direction of the selected point from the ship
					if (Physics.Raycast (lineOfSightOrigin,Vector3.Normalize(hitInfo.point - lineOfSightOrigin), out lineOfSightHit, 100f)){
						//If the ray intersects with anything but water then there is no line of sight
						//Debug.DrawRay(lineOfSightOrigin,Vector3.Normalize(hitInfo.point - lineOfSightOrigin) * 100, Color.yellow);
						//Debug.Log (lineOfSightHit.transform.tag);
						if (lineOfSightHit.transform.tag != "waterSurface")
							 hasLineOfSight = false;
						else
							hasLineOfSight = true;
					}
						
						//If the cursor is on water and has line of sight
						if (hitInfo.collider.tag == "waterSurface" && hasLineOfSight == true){
							//Make sure the cursor ring is Green
							cursorRingIsGreen = true;
							
							CalculateShipTrajectoryPreview(hitInfo.point);
				
							//Now check to see if the player clicks the left mouse button to travel
							if (Input.GetButton("Select")){
								//disable the info GUI
								MGV.showSettlementInfoGUI = false;
								//lock controls so that the travel function is triggered on the next update cycle
								MGV.controlsLocked = true; 
								//set the destination: using the players Y value so the ship always stays at a set elevation
								currentDestination = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
								//set the player ship's current position to be logged into the journey log
								lastPlayerShipPosition = transform.position;
								travel_lastOrigin = transform.position;
								numOfDaysTraveled = 0;
							}
							
						} else {
						//Since we aren't allowed to travel to the mouse position, change the cursor to red
						cursorRingIsGreen = false;
						//We also need to make sure the trajectory preview is turned off
						CalculateShipTrajectoryPreview(transform.position);
						}
				
				//If the point on the screen correlates to nothing--say the sky, then turn off the ship projectory preview		
				} else {
				cursorRing.SetActive(false);
				CalculateShipTrajectoryPreview(transform.position);
				
				}
			
		}
	}
	
	
	public void CheckCameraRotationControls(){
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		if (horizontal < 0){
		//Rotate right
			MGV.FPVCamera.transform.parent.parent.RotateAround(MGV.FPVCamera.transform.parent.parent.position, MGV.FPVCamera.transform.parent.parent.up,-70*Time.deltaTime);
		} else if (horizontal > 0) {
		//Rotate Left
			MGV.FPVCamera.transform.parent.parent.RotateAround(MGV.FPVCamera.transform.parent.parent.position, MGV.FPVCamera.transform.parent.parent.up,70*Time.deltaTime);
		}
		//Debug.Log (MGV.FPVCamera.transform.eulerAngles.x);
		//This first if statement sets the boundary range for the camera vertical rotation--the Unity engine makes this a bit wonky because it's 270-90 for a full 180 degree rotation
		if ((MGV.FPVCamera.transform.eulerAngles.x <= 40f && MGV.FPVCamera.transform.eulerAngles.x >= 0f) || (MGV.FPVCamera.transform.eulerAngles.x <= 360f && MGV.FPVCamera.transform.eulerAngles.x >= 295f)){
			if (vertical < 0 ){
				//Rotate down					
				MGV.FPVCamera.transform.RotateAround(MGV.FPVCamera.transform.position, MGV.FPVCamera.transform.right,1.5f);

	
			} else if (vertical > 0) {
				//Rotate up
				MGV.FPVCamera.transform.RotateAround(MGV.FPVCamera.transform.position, MGV.FPVCamera.transform.right,-1.5f);
				//Now we need to make sure we don't over rotate past our mark
							}
		}
		//Now we need to make sure we don't over rotate past our mark
		if(MGV.FPVCamera.transform.eulerAngles.x <= 50f && MGV.FPVCamera.transform.eulerAngles.x >= 39f){
			MGV.FPVCamera.transform.eulerAngles = new Vector3(39.9f,MGV.FPVCamera.transform.eulerAngles.y,MGV.FPVCamera.transform.eulerAngles.z);
			//Debug.Log ("We're chancing it to 40?");
		}
		if(MGV.FPVCamera.transform.eulerAngles.x <= 296f && MGV.FPVCamera.transform.eulerAngles.x >= 285f)
			MGV.FPVCamera.transform.eulerAngles = new Vector3(295.9f,MGV.FPVCamera.transform.eulerAngles.y,MGV.FPVCamera.transform.eulerAngles.z);
		if(MGV.FPVCamera.transform.eulerAngles.x < 0)
			MGV.FPVCamera.transform.eulerAngles = new Vector3(359f,MGV.FPVCamera.transform.eulerAngles.y,MGV.FPVCamera.transform.eulerAngles.z);
	}
	
	public void CheckZoomControls(){
		float zoom = Input.GetAxis("Mouse ScrollWheel");
		//if zooming out
		if (zoom < 0 && MGV.FPVCamera.transform.parent.localScale.x < 3.2f){
			MGV.FPVCamera.transform.parent.localScale = new Vector3(MGV.FPVCamera.transform.parent.localScale.x +.1f,MGV.FPVCamera.transform.parent.localScale.y + .001f,MGV.FPVCamera.transform.parent.localScale.z + 0.1f);
			MGV.FPVCamera.transform.parent.Translate(new Vector3(0,.023f,0));
			//MGV.FPVCamera.transform.position = Vector3.Lerp(MGV.FPVCamera.transform.position, new Vector3(MGV.FPVCamera.transform.parent.position.x,MGV.FPVCamera.transform.parent.position.y+.1f,MGV.FPVCamera.transform.parent.position.z+4f), .1f);//new Vector3(MGV.FPVCamera.transform.localPosition.x + .01f,MGV.FPVCamera.transform.localPosition.y + .01f,MGV.FPVCamera.transform.localPosition.z + .01f);

		} else if (zoom > 0 && MGV.FPVCamera.transform.parent.localScale.x > .0698f){
			MGV.FPVCamera.transform.parent.localScale = new Vector3(MGV.FPVCamera.transform.parent.localScale.x -.1f,MGV.FPVCamera.transform.parent.localScale.y - .001f,MGV.FPVCamera.transform.parent.localScale.z - 0.1f);
			MGV.FPVCamera.transform.parent.Translate(new Vector3(0,-.023f,0));
			//MGV.FPVCamera.transform.position = Vector3.Lerp(MGV.FPVCamera.transform.position, MGV.FPVCamera.transform.parent.position, .1f);//MGV.FPVCamera.transform.localPosition = new Vector3(MGV.FPVCamera.transform.localPosition.x - .01f,MGV.FPVCamera.transform.localPosition.y - .01f,MGV.FPVCamera.transform.localPosition.z - .01f);
	
		}
		
		//If the zoom over shoots its target, then reset it to the minimum
		if (MGV.FPVCamera.transform.parent.localScale.x < .0698f){
			MGV.FPVCamera.transform.parent.localScale = new Vector3(.0698f,MGV.FPVCamera.transform.parent.localScale.y,.0698f);
			MGV.FPVCamera.transform.parent.localPosition = new Vector3(0,-230f,0);
			}
	
		
	}
	
	
	
	public void TravelToSelectedTarget(Vector3 destination){
		//Let's slowly rotate the ship towards the direction it's traveling and then allow the ship to move
		if (!shipTravelStartRotationFinished){

			
			destination = new Vector3(destination.x, shipTransform.position.y, destination.z);
			Vector3 temprot = Vector3.RotateTowards(shipTransform.forward, Vector3.Normalize(destination - shipTransform.position), .1f, 0.0F);
			Vector3 targetDirection = Vector3.Normalize(destination - shipTransform.position);
				//Debug.Log (targetDirection + "         ==?   " + shipTransform.forward );
			//We use Mathf Approximately to compare float values and end the rotation sequence when the ships direction matches the target's direction
			if (MGV.FastApproximately(targetDirection.x, shipTransform.forward.x, .01f) && 
			    MGV.FastApproximately(targetDirection.y, shipTransform.forward.y, .01f) && 
			    MGV.FastApproximately(targetDirection.z, shipTransform.forward.z, .01f)){
				shipTravelStartRotationFinished = true;
			}
			//Lerp the rotation of the ship towards the destination
			shipTransform.rotation = Quaternion.LookRotation(temprot);
			//temprot = new Vector3(temprot.x, shipTransform.position.y, temprot.z);
			//shipTransform.LookAt(temprot);
		//Once we're finished, allow the ship to travel
		} else {
			//Turn off mouse selection cursor
			cursorRing.SetActive(false);
			
			//Draw red line from ship to destination
	//		LineRenderer line = gameObject.GetComponent<LineRenderer>();
	//		line.SetPosition(0,new Vector3(transform.position.x,transform.position.y-.23f,transform.position.z));
	//		line.SetPosition(1,new Vector3(destination.x,destination.y-.23f,destination.z));
	//		line.SetColors(Color.green, Color.blue);
	
			//rotate player ship to settlement
			shipTransform.LookAt(destination);
			shipTransform.eulerAngles = new Vector3(0,shipTransform.eulerAngles.y,0);
			//Get direction to settlement from ship
			Vector3 travelDirection = Vector3.Normalize(destination - transform.position);
			float distance = Vector3.Distance(destination , transform.position);
	
			
			
			//figure out the actual speed of the ship if currents/wind are present and if the sails are unfurled or not
			Vector3 windAndWaterVector = GetCurrentWindWaterForceVector(travelDirection);

			
			//Debug.Log (travel_lastOrigin + "  --<<TRAVEL LAST ORIGIN");
			float disTraveled = Vector3.Distance(travel_lastOrigin, transform.position);
			travel_lastOrigin = transform.position;
			
			//Debug.Log (current_shipSpeed_Magnitude + "   <current ship speed mag");
			float numOfDaysTraveledInSegment = ((disTraveled*(MGV.unityWorldUnitResolution / 1000f)) 
																/ 
													current_shipSpeed_Magnitude) / (24f);
													
			numOfDaysTraveled += numOfDaysTraveledInSegment;
			//Debug.Log (numOfDaysTraveled +  "    ---< num of days traveled");
			//Debug.Log (disTraveled);
			ship.totalNumOfDaysTraveled += numOfDaysTraveledInSegment;
			
			//Perform regular updates as the ship travels
			UpdateShipAtrophyAfterTravelTime(numOfDaysTraveledInSegment);
			CheckIfFoodOrWaterIsDepleted(numOfDaysTraveledInSegment);
			WillARandomEventHappen();
			UpdateDayNightCycle(MGV.IS_NOT_NEW_GAME);
			UpdateNavigatorBeaconAppearenceBasedOnDistance();
			
			//if the ship hasn't gotten to the direction, then keep moving
			if (distance > .2f && !notEnoughSpeedToMove && !rayCheck_stopShip) {
				//We need to make sure the ship has the speed to go against the wind and currents--if it doesn't then we need to stop traveling
				//	--we will check the angle between the final movement vector and the destination
				//	--if it's more than 160 degrees, then stop the ship (more than 160 can be buggy)
				playerMovementVector = ( (travelDirection * shipSpeed_Actual) + windAndWaterVector) * shipSpeed_Game_Modifier ;
				//Debug.Log (Vector3.Angle(travelDirection, playerMovementVector));
				if (Vector3.Angle(travelDirection, playerMovementVector) < 160f){
					playerMovementMagnitude = playerMovementVector.magnitude;
					
					controller.Move ( playerMovementVector  * Time.deltaTime);
				} else 
					notEnoughSpeedToMove = true;
					
				//Draw red line from ship to destination
				//line.SetPosition(0,new Vector3(transform.position.x,transform.position.y-.23f,transform.position.z));
				
				//Fire off the coast line raycasts to detect for the coast
				DetectCoastLinesWithRayCasts();
				
			} else if (!MGV.showSettlementTradeGUI || notEnoughSpeedToMove) { //check to see if we're in the trade menu otherwise we will indefintely write duplicate routes until we leave the trade menu
				//save this route to the PlayerJourneyLog
				journey.AddRoute(new PlayerRoute(lastPlayerShipPosition, transform.position, ship.totalNumOfDaysTraveled), gameObject.GetComponent<script_player_controls>(), MGV.currentCaptainsLog);
				//Update player ghost route
				UpdatePlayerGhostRouteLineRenderer(MGV.IS_NOT_NEW_GAME);
				//Reset the travel line to a distance of zero (turn it off)
				//line.SetPosition(0,Vector3.zero);
				//line.SetPosition(1,Vector3.zero);
				
				current_shipSpeed_Magnitude = 0f;
				
				//reset the not enough speed flag
				notEnoughSpeedToMove = false;
				MGV.controlsLocked = false;
				shipTravelStartRotationFinished = false;
				
				//reset coastline detection flag
				rayCheck_stopShip = false;
			}//End of Travel
		}//End of initial ship rotation
	}
	
	
	public void UpdateShipAtrophyAfterTravelTime(float travelTime){

		//deplete food based on number of crew  (NASA - .83kg per astronaut / day
		if(ship.cargo[1].amount_kg <= 0) ship.cargo[1].amount_kg = 0;
		else ship.cargo[1].amount_kg -= (travelTime * dailyFoodKG) * ship.crew;
		
		//deplete water based on number of crew (NASA 3kg min a day --we'll use 5 because conditions are harder--possibly 10 from rowing)
		if(ship.cargo[0].amount_kg <= 0) ship.cargo[0].amount_kg = 0;
		else ship.cargo[0].amount_kg -= (travelTime * dailyWaterKG) * ship.crew;
		
		//deplete ship hp (we'll say 1HP per day)
		if (ship.health <= 0) ship.health = 0;
		else ship.health -=  travelTime;
		
		//Debug.Log (travelTime + "days in segment   --- " +ship.cargo[0].amount_kg + "kg    " + ship.cargo[1].amount_kg + "kg    " + ship.health + "hp   lost to travel needs");
	
	}
	
	
	void OnTriggerEnter(Collider trigger){
	Debug.Log ("On trigger enter triggering" + trigger.transform.tag);
		if(trigger.transform.tag == "currentDirectionVector"){
		 	currentWaterDirectionVector = trigger.transform.GetChild(0).GetChild(0).up.normalized;
			currentWaterDirectionMagnitude = trigger.transform.GetChild(0).GetComponent<script_WaterWindCurrentVector>().currentMagnitude;
		} 
		if (trigger.transform.tag == "windDirectionVector"){
			currentWindDirectionVector = trigger.transform.GetChild (0).transform.forward.normalized;
			currentWindDirectionMagnitude = trigger.transform.GetChild(0).GetComponent<script_WaterWindCurrentVector>().currentMagnitude;
		}
		if (trigger.transform.tag == "settlement_dock_area"){
		//Here we first figure out what kind of 'settlement' we arrive at, e.g. is it just a point of interest or is it a actual dockable settlement
		//if it's a dockable settlement, then allow the docking menu to be accessed, otherwise run quest functions etc.
			if(trigger.transform.parent.GetComponent<script_settlement_functions>().thisSettlement.typeOfSettlement == 1){
			getSettlementDockButtonReady = true;
			MGV.currentSettlement = trigger.transform.parent.gameObject.GetComponent<script_settlement_functions>().thisSettlement;
			MGV.currentSettlementGameObject = trigger.transform.parent.gameObject;
			MGV.playerShipVariables.ship.playerJournal.AddNewSettlementToLog(MGV.currentSettlement.settlementID);
			//If it is a point of interest then run quest functions but don't allow settlement resource access
			} else if (trigger.transform.parent.GetComponent<script_settlement_functions>().thisSettlement.typeOfSettlement == 0){
				//change the current settlement to this location (normally this is done by opening the docking menu--but in this case there is no docking menu)
				MGV.currentSettlement = trigger.transform.parent.GetComponent<script_settlement_functions>().thisSettlement;
			//Check if current Settlement is part of the main quest line
				MGV.CheckIfCurrentSettlementIsPartOfMainQuest(MGV.currentSettlement.settlementID);
			}
		}
		if (trigger.transform.tag == "settlement"){
			Debug.Log ("Entering Area of: "+ trigger.GetComponent<script_settlement_functions>().thisSettlement.name + ". And the current status of the ghost route is: " + MGV.playerGhostRoute.activeSelf);
			//This zone is the larger zone of influence that triggers city specific messages to pop up in the captains log journal
			MGV.AddEntriesToCurrentLogPool(trigger.GetComponent<script_settlement_functions>().thisSettlement.settlementID);
			//We add the triggered settlement ID to the list of settlements to look for narrative bits from. In the OnTriggerExit() function, we remove them
			MGV.activeSettlementInfluenceSphereList.Add(trigger.GetComponent<script_settlement_functions>().thisSettlement.settlementID);
			//If the player got lost asea and the memory map ghost route is turned off--check to see if we're enteringg friendly waters
			if (MGV.playerGhostRoute.activeSelf == false){
				CheckIfPlayerFoundKnownSettlementAndTurnGhostTrailBackOn(trigger.GetComponent<script_settlement_functions>().thisSettlement.settlementID);
			}
		}
	}
	
	void OnTriggerExit(Collider trigger){
		if(trigger.transform.tag == "waterDirectionVector"){

		} 
		if (trigger.transform.tag == "windDirectionVector"){ //right now--there should always be trigger box--it should just update, and never need to reset upon exit

		}	
		if (trigger.transform.tag == "settlement_dock_area"){
			MGV.showSettlementTradeButton = false;
		}
		if (trigger.transform.tag == "settlement"){
			//This zone is the larger zone of influence that triggers city specific messages to pop up in the captains log journal
			MGV.RemoveEntriesFromCurrentLogPool(trigger.GetComponent<script_settlement_functions>().thisSettlement.settlementID);
			//We add the triggered settlement ID to the list of settlements to look for narrative bits from. In the OnTriggerExit() function, we remove them
			MGV.activeSettlementInfluenceSphereList.Remove(trigger.GetComponent<script_settlement_functions>().thisSettlement.settlementID);
		}
	}
	
	void CheckIfFoodOrWaterIsDepleted(float travelTimeToAddIfDepleted){
		if (ship.cargo[0].amount_kg <= 0) {
		 	numOfDaysWithoutWater += travelTimeToAddIfDepleted; 
		 	CheckToSeeIfCrewWillDieFromThirst();
		} else {
			numOfDaysWithoutWater = 0;
			dayCounterThirsty = 0;
		}
		 
		if (ship.cargo[1].amount_kg <= 0) {
			 numOfDaysWithoutFood += travelTimeToAddIfDepleted; 
			 CheckToSeeIfCrewWillDieFromStarvation();
		} else { 
			numOfDaysWithoutFood = 0;
			dayCounterStarving = 0;
		}
	}

	
		
	void CheckToSeeIfCrewWillDieFromStarvation(){
		//This uses a counter system to determine the number of days in order to make sure the death roll is only rolled
		//	--ONCE per day, rather than every time the function is called.
		//Every day without food--the chance of a crew member dying from starvation increases
		//	--The first day starts at 30%, and every day onward increases it by 10%
		int numOfDays = Mathf.FloorToInt(numOfDaysWithoutFood);
		//first check to see if we're at atleast one day
		if (numOfDays >= 1) {
			dayCounterStarving ++;
			numOfDaysWithoutFood = 0;
			int deathRate = 50 + (dayCounterStarving*10);
			
			//If a crewmember dies due to the percentage roll
			if (Random.Range(0,100) <= deathRate)
				//Kill a crewmember
				KillCrewMember();
		}
	
	}
	
	void CheckToSeeIfCrewWillDieFromThirst(){
		//This uses a counter system to determine the number of days in order to make sure the death roll is only rolled
		//	--ONCE per day, rather than every time the function is called.
		//Every day without Water--the chance of a crew member dying from starvation increases
		//	--The first day starts at 80%, and every day onward increases it by 10%
		int numOfDays = Mathf.FloorToInt(numOfDaysWithoutWater);
		//first check to see if we're at atleast one day
		if (numOfDays >= 1){
			dayCounterThirsty ++;
			numOfDaysWithoutWater = 0;
			int deathRate = 50 + (dayCounterThirsty*10);
			
			//If a crewmember dies due to the percentage roll
			if (Random.Range(0,100) <= deathRate)
				//Kill a crewmember
				KillCrewMember();
		}
	}
	
	void KillCrewMember(){
		//If there are crew members to kill off
		if (ship.crewRoster.Count > 0){
			//choose a random crew member to kill
			ship.crewRoster.RemoveAt( Random.Range(0,ship.crewRoster.Count) );
			//update any clout the player may have lost from a crew member's death
			if (ship.playerClout <= 0) ship.playerClout = 0;
			else ship.playerClout -= 5;
		}
	}
	
	CrewMember RemoveRandomCrewMember(){
		//Find a random crewmember to kill if they can be killed
		
		CrewMember killedMate = new CrewMember(-1);
		List<int> listOfPossibleCrewToKill = new List<int>();
		
		//generate a list of possible crew that can be killed
		for(int i = 0; i < ship.crewRoster.Count; i++){
			if (ship.crewRoster[i].isKillable) listOfPossibleCrewToKill.Add(i);
		}
		
		//if we don't find any available crewmembers to kill, return an empty crewman as a flag that none exist to be killed
		if(listOfPossibleCrewToKill.Count != 0){
			int randomMemberToKill = listOfPossibleCrewToKill[Random.Range (0,listOfPossibleCrewToKill.Count-1)];
					//Store the crewman in a temp variable
					killedMate = ship.crewRoster[randomMemberToKill];
					//Remove the crewmember
					ship.crewRoster.Remove (killedMate);
					//return the removed crewmember
					return killedMate;
				
			
		} 
		
		//If there are no available members then just return a null flagged member initialize in the beggining of this function
		return killedMate;
		
		
	}
	
	
	//#########################################################################################################
	//	RANDOM  EVENT  FUNCTION
	//=========================
	//		--This function determines whether or not a random event will happen to the ship. Regardless of 
	//		--whether or not a random event occurs, it will trigger journal messages based on whether or not
	//		--the ship is in open sea or near a sphere of influence of a settlement/location of interest
	//
	//#########################################################################################################
	
	void WillARandomEventHappen(){

		
		//Random Events have a chance to occur every half day of travel
		//-------------------------------------------------------------
		//These values help determine the half day of travel
		float tenthPlaceTemp = (ship.totalNumOfDaysTraveled - Mathf.FloorToInt(ship.totalNumOfDaysTraveled));
		tenthPlaceTemp *= 10;
		float hundredthPlaceTemp = (tenthPlaceTemp - Mathf.FloorToInt(tenthPlaceTemp)) * 10;
		int hundredthPlace = Mathf.FloorToInt(hundredthPlaceTemp);
		//Debug.Log (tenthPlaceTemp + "  " + hundredthPlaceTemp);
		//If we are at a half day's travel, then see if a random event occurs
		if ((Mathf.FloorToInt(tenthPlaceTemp) == 5 || Mathf.FloorToInt (tenthPlaceTemp) == 9) && !MGV.isPerformingRandomEvent){
			MGV.isPerformingRandomEvent = true;
			float chanceOfEvent = .95f; //0 - 1 value representing chance of a random event occuring
			//We determine if the 
			if(Random.Range(0f,1f) <= chanceOfEvent) {
				//Debug.Log ("Triggering Random Event");
				//When we trigger a random event, let's make the ship drop anchor!
				MGV.playerShipVariables.rayCheck_stopShip = true;
				
				//We separate Random events into two possible categories: Positive, and Negative.
				//First we need to determine if the player has a positive or negative event occur
				//--The basic chance is a 50/50 chance of either or, but we need to figure out if the
				//--crew makeup has any augers, and if so, each auger decreases the chance of a negative
				//--event by 10%. We then roll an aggregate clout score to further reduce the chance by a maximum of 20%
				
				//Count the number of augers on board the ship
				int numOfAugers = 0;
				foreach(CrewMember crewman in ship.crewRoster){if(crewman.typeOfCrew == 5) numOfAugers++;}
				
				//Get the 0-1 aggregate clout score. Here we use the current zone of influence's network id to check
				int currentZoneID = 0;
				//TODO Right now this just uses the relevant city's ID to check--but in the aggregate score function--it should start using the networks--not the city.
				if(MGV.activeSettlementInfluenceSphereList.Count > 0) currentZoneID = MGV.activeSettlementInfluenceSphereList[0];
				float aggregateCloutScore = MGV.GetOverallCloutModifier(currentZoneID);
				//Now determine the final weighted chance score that will be .5f and under
				chanceOfEvent = .5f - (.1f*numOfAugers) - (.2f*aggregateCloutScore);
				
				
				//If we roll under our range, that means we hit a NEGATIVE random event
				//**************************
				//******NEGATIVE EVENT
				//**************************
				if(Random.Range(0f,1f) <= chanceOfEvent) {
				
					int numOfEvents = Random.Range (1,4);
					//Here are negative random events that can happen while traveling
					//---------------------------------------------------------------		
					//*pirate attack: You lose cargo and/or crew members, and ship hp is reduced
					//*storm: your ship is moved to a random location within half a days travel, ship hp is reduced
					//*crewmember is sick: they may or may not die, but temporarily acts as one less crew member, and uses twice as much water
					//*military request: War trireme may demand supplies for war effort and ask for crew who might randomly join them
					switch (numOfEvents){
					
					//======================
					//PIRATE ATTACK
					case 1:
						//We need to determine whether or not the player successfully defends against the pirate attack
						//The first check is to see if the pirates are part of the same network as the player--if they are, they apologize and leave the player alone
						//if they aren't in the same network, the player has a base 20% chance of succeeding plus 5% per warrior present on board, plus a max of 20% based on aggregate clout
						int numOfWarriors = 0;
						foreach(CrewMember crewman in ship.crewRoster){if(crewman.typeOfCrew == 1) numOfWarriors++;}
						//TODO Right now we'll just assume they aren't in the network
						chanceOfEvent = .2f + (.05f*numOfWarriors) + (.2f*aggregateCloutScore);
						//Damage the ship regardlesss of the outcome
						MGV.AdjustPlayerShipHealth(-20);
						//If the roll is lower than the chanceOfEvent variable--the pirates were unsuccessful
						if(Random.Range(0f,1f) <= chanceOfEvent) {
							//Raise the player and crews clout after the successful event
							MGV.AdjustCrewsClout(3);
							MGV.AdjustPlayerClout(3);

							//Despite their lack of success--there might be a chance of losing a crewman to the attack
							//The chance of a death is 20%  minus a total of 10% from the agregate clout score.
							chanceOfEvent = .2f - (.1f * aggregateCloutScore);
							//If a crew member dies
							if(Random.Range (0f,1f) < chanceOfEvent){
								//get a random crewmember
								CrewMember crewToKill = RemoveRandomCrewMember();
								//If there is a crewmember to kill
								if (crewToKill.ID != -1){
									MGV.notificationMessage = "Your crew spots a distant ship closing in fast--definitely pirates! By the gods! You manage to escape after fending off their attack! As your crew cheers with victorious honor, "
															+ "they suddenly stop upon realizing they lost a good crewman... " + crewToKill.name + "'s sacrifice has earned him great honor. You prepare the proper rites and cast the sailor asea.";
									MGV.showNotification = true;
								//otherwise there is no crewmember to kill--either from not having enough crewmembers or no crewmembers that are killable so just give the success response without a death
								} else {
									MGV.notificationMessage = "Your crew spots a distant ship closing in fast--definitely pirates! By the gods! After a heated battle, you manage to escape after fending off their attack without any casualties! Your crew cheers with victorious honor!";
									MGV.showNotification = true;
								}
							//No crewmembers died so it was a perfect defensive victory	
							} else {
								MGV.notificationMessage = "Your crew spots a distant ship closing in fast--definitely pirates! By the gods! After a heated battle, you manage to escape after fending off their attack without any casualties! Your crew cheers with victorious honor!";
								MGV.showNotification = true;
							}

						
						//Otherwise the pirates were successful and the necessary penalties occur
						} else {
							//Reduce the clout of the player and crew
							MGV.AdjustCrewsClout(-3);
							MGV.AdjustPlayerClout(-3);
							
							// penalty:loss of half the ship's cargo across the board.
							foreach(Resource resource in ship.cargo){
								int newAmount = (int) (resource.amount_kg / 2);
								resource.amount_kg -= newAmount;
							
							}
							
							// penalty:the death of up to 6 crew members if available/killable
							int numOfCrewToKill = Random.Range (1,6);
							List<CrewMember> lostCrew = new List<CrewMember>();
							string lostNames = "";
							//Fill a list of killed crew
							for(int i = 0; i <= numOfCrewToKill; i ++){
								CrewMember temp = RemoveRandomCrewMember();
								//if the removed crewmember is flagged as null, then there are no crewmembers to kill
								if (temp.ID != -1) {
									lostCrew.Add(temp);
									//add the name to the compiled string with a comma if not the last index
									if (i != numOfCrewToKill) lostNames += temp.name + ", "; else lostNames += temp.name;
								}
							} 
							//If the list of killed crew is empty, then we didn't kill any crewmembers so add a message that explains why
							if (lostCrew.Count == 0) {
								MGV.notificationMessage = "Your crew spots a distant ship closing in fast--they are definitely pirates! Your crew prepares for battle as the ship rams at full speed into your hull." + 
														  " Your crew fights valiantly against the onslaught, but the pirates successfully bring you to your knees! They rummage through your holds and take half your supplies with them as a generous bounty." +
														  " They intended to take a number of you prisoner, but the pirate captain sensed a strange omen surrounding you and wanted no part in your crew's fate. They leave your dishonored ship with their newly acquired supplies.";
								MGV.showNotification = true;	
							//Otherwise members died so alert the player
							} else {
								MGV.notificationMessage = "Your crew spots a distant ship closing in fast--they are definitely pirates! Your crew prepares for battle as the ship rams at full speed into your hull." + 
															" Your crew fights valiantly against the onslaught, but the pirates successfully bring you to your knees! They rummage through your holds and take half your supplies with them as a generous bounty." +
															" Unfortunately, you lose " + lostNames + " to death and kidnapping. The remaining crew are unsettled but fortunate for their lives.";
								MGV.showNotification = true;	
							
							}
							
					
						}
						break;
					
					//=====================	
					//STORM AT SEA	
					case 2:
						//We need to dtermine whether or not the player sucessfully navigates through the storm.
						//The player has a 20% chance of succeeding plus 5% per sailor on board, plus a max of 20% based on aggregate clout
						int numOfSailors = 0;
						foreach(CrewMember crewman in ship.crewRoster){if(crewman.typeOfCrew == 0) numOfSailors++;}
						chanceOfEvent = .2f + (.005f*numOfSailors) + (.2f*aggregateCloutScore);
						Debug.Log (chanceOfEvent);
						//If the roll is lower than the chanceOfEvent variable--the storm was unsuccessful in throwing the player off course
						if(Random.Range(0f,1f) <= chanceOfEvent) {
							//Adjust crew clout
							MGV.AdjustCrewsClout(3);
							MGV.AdjustPlayerClout(3);
							
							//Despite their lack of success--there might be a chance of losing a crewman to the storm
							//The chance of a death is 20%  minus a total of 10% from the agregate clout score.
							MGV.notificationMessage = "A storm suddenly surges across the seas filling your crew with worry. The waves crash upon your ship and your sails whip in the winds, but your crew holds fast and successfully navigates the storm!";
							MGV.showNotification = true;
							
							//Otherwise the storm was successful and the necessary penalties occur
						} else {
							//Adjust crew clout
							MGV.AdjustCrewsClout(-3);
							MGV.AdjustPlayerClout(-3);
							
							//The first penalty is a possibility for the death of 3 crew members
							//second penalty is the movement of the ship in a random direction for 50 in-game units ~50km or until a shoreline is reached
							//--this is accomplished through a raycast shot in a random direction from the ship
							//set the distance to 50
							float offCourseDistance = 50f;
							//set the layer mask to only check for collisions on layer 10 ("terrain")--this helps ignore the multiple irrelevant hit boxes that exist in the environment(including the player)
							int terrainLayerMask = 1 << 10;
							//We get a random directional vector3 by keeping the y at 0 and providing a value of -1 to 1 for the x and z values
							Vector3 offCourseDirection = new Vector3(Random.Range (-1f,1f),0,Random.Range (-1f,1f));
							//Set the origin to be at the base of the ship
							Vector3 rayOrigin = transform.position;// + new Vector3(0,-.23f,0);
							
							RaycastHit possibleTerrain;
							//If we get a hit then push the players ship 5 units (~5km) before the impact off the shore
							if (Physics.Raycast(rayOrigin, offCourseDirection, out possibleTerrain, offCourseDistance, terrainLayerMask)){
								//Determine the location of the shore hit
								Vector3 hitLocation = possibleTerrain.point;
								//Determine the location of the off-shore hit by cycling back 5 units in the direction of the ray origin
								Vector3 courseDirectionOpposite = offCourseDirection * -1f;
								Vector3 adjustedLocation = hitLocation + (courseDirectionOpposite * 5f);
								//Move the player's ship to the location
								transform.position = adjustedLocation;
								

							
							//If we don't get a hit, then just move the player ship to that position.
							} else {
								//move the player to the new position
								Vector3 offCoursePosition = rayOrigin + (offCourseDirection * 50f);
								transform.position = offCoursePosition;
							}
							
							//TODO Turn off the ghost trail path of the player to reduce their ability to find their location--this will turn on when revisiting a known settlement.
							MGV.playerGhostRoute.SetActive(false);
							
							//Kill up to 6 crew members!
							int numOfCrewToKill = Random.Range (1,6);
							List<CrewMember> lostCrew = new List<CrewMember>();
							string lostNames = "";
							//Fill a list of killed crew
							for(int i = 0; i <= numOfCrewToKill; i ++){
								CrewMember temp = RemoveRandomCrewMember();
								//if the removed crewmember is flagged as null, then there are no crewmembers to kill
								if (temp.ID != -1) {
									lostCrew.Add(temp);
									//add the name to the compiled string with a comma if not the last index
									if (i != numOfCrewToKill) lostNames += temp.name + ", "; else lostNames += temp.name;
								}
							}	
							//Display message telling the player what occured
							if (lostCrew.Count == 0) {
								MGV.notificationMessage = "A storm suddenly surges across the seas filling your crew with worry. They struggle for hours " + 
														 "until the storm overcomes their senses and abilities. You all hold tight and let the storm take your ship where "+
														"it will, leaving you lost asea without any known bearings across the waters.";
								MGV.showNotification = true;	
							} else {
									MGV.notificationMessage = "A storm suddenly surges across the seas filling your crew with worry. They struggle for hours " + 
														"until the storm overcomes their senses and abilities. You all hold tight and let the storm take your ship where "+
														"it will, leaving you lost asea without any known bearings across the waters. Unfortunately, you lose " + lostNames + " to the storm's wrath! You say a few prayers to Poseidon " +
														"and struggle onward to find your way!";
								MGV.showNotification = true;
							}
							
												
						}
						break;
					
					
					//SICK CREW MEMBER		
					case 3:
						//A random crewmember gets sick, and there is a chance up to two members die from the event
						//Kill up to 2 crew members!
						int numOfSickCrewToKill = Random.Range (1,2);
						List<CrewMember> sickCrew = new List<CrewMember>();
						string lostSickNames = "";
						//Fill a list of killed crew
						for(int i = 0; i <= numOfSickCrewToKill; i ++){
							CrewMember temp = RemoveRandomCrewMember();
							//if the removed crewmember is flagged as null, then there are no crewmembers to kill
							if (temp.ID != -1) {
								sickCrew.Add(temp);
								//add the name to the compiled string with a 'and' if not the last index
								if (i != numOfSickCrewToKill) lostSickNames += temp.name + " & "; else lostSickNames += temp.name;
							}
						}	
						string finalMessage = "The crew alerts you to a terrible predicament--someone has been sick and it may have spread!";
						
						//check to see if anyone dies
						//If no one dies, then let the player know they survived
						if (lostSickNames == ""){
							finalMessage += " Fortunately, the crew seems fine and the sickness doesn't seem to be disease. Maybe they are just overworked a bit."+
							"The members who feel ill, express their need for a little rest and they'll be fine. You agree and the journey continues!";
						} else {
						//Someone was sick!
							//If it was one person
							if(!lostSickNames.Contains("&")){
								finalMessage += "When you inspect " + lostSickNames + ", you make the decision to throw him overboard--he's definitely diseased, and the crew can't afford to catch it."+
								"It may seem heartless, but he knew what he signed up for when he decided to go on this journey!";
							} else {
								finalMessage += "When you have a look at " + lostSickNames + ", you give a knowing sigh--that they both have a terrible plague, and the rest of the crew agrees to throw them off the ship. It's better for two to die, than the entire crew!";	
							}
						
						}
						
						finalMessage += " You continue the journey with the crew--thankful that it wasn't any worse. A plague on a ship asea is quite dangerous.";
						MGV.notificationMessage = finalMessage;
						MGV.showNotification = true;
						break;
					
					
					//MILITARY CONFISCATION		
					case 4:
						//A fleet of triremes heading to battle stop your ship and demand some of your stores for their journey
						finalMessage = "You spot a small fleet of ships in the distance closing in fast on your own vessel. The crew is terribly worried it may be pirates!" +
						"As they approach you realize it's not pirates--but it may as well be--a military expedition! They hail you and explain their war efforts."+
						"You acknowledge their courage and praise their victories to come, all the while waiting for the captain to make his demands upon your ship."+
						" Everyone is a pirate these days it seems!";
						
						//Find a random resource on board the ship that is available and remove half of it.
						bool whileBreaker = true;
						while(whileBreaker){
							int cargoIndex = Random.Range (0,14);
							if(ship.cargo[cargoIndex].amount_kg > 0){
								int amountToRemove = Mathf.CeilToInt(ship.cargo[cargoIndex].amount_kg / 2f);
								whileBreaker = false;
								ship.cargo[cargoIndex].amount_kg /= 2;
								finalMessage += " The troops demand a manifest and upon inspection, determine they require " + amountToRemove + "kg of " + ship.cargo[cargoIndex].name + " from your stores. You grit your teeth but smile and agree. You're in no position to argue!";
							}
								
						
						}
						//Remove a random crew member who is taken for the war effort
						CrewMember tempCrew = RemoveRandomCrewMember();
						//if the removed crewmember isn't flagged as null, then there are crewmembers to lose
						if (tempCrew.ID != -1) {
							finalMessage += "The captain also eyes your crew before explaining how he needs another set of strong arms to man an oar on his trireme! He looks at "+
							tempCrew.name + " and demands he come aboard his ship. " + tempCrew.name + " looks at you and sighs--knowing there's nothing that can be done! He wishes you the best and thanks you for the stores and crewman!";
						
						//otherwise, the military doesn't want any of your crew
						} else {
							finalMessage += "The captain eyes over your crew and makes a strange sound of displeasure. He explains none of your crew seem capable enough for the war effort and that they'll be on their way. They wish you luck on your journey!";
						}
						
						finalMessage += "You think to yourself, as the commander sails away with his small fleet, how odd it is to thank someone for stealing from them. Your crew seems equally frustrated, but equally glad they aren't sailing to some unknown battle against some unknown king. You unfurl the sails and go about your journey!";
						
						MGV.notificationMessage = finalMessage;
						MGV.showNotification = true;	
						break;
					
					
					
					
					
					
					
					}
					
					
				//**************************
				//*****POSITIVE EVENT
				//**************************
				} else {
				//TODO This is a temporary fix to show something positive happened
				//	MGV.notificationMessage = 
				//	MGV.showNotification = true;	
				
				
				Debug.Log ("POSITIVE EVENT TRIGGER");
					//Here are positive random events that can happen while traveling
					//---------------------------------------------------------------		
					//friendly ship: offer out of network information--if low on water/food they may offer you some stores and suggest a port to visit
					//Abandoned/Raided Ship: crew is dead but you find left over stores to add to your cargo
					//Favor of the Gods: Your crew feels suddenly uplifted and courageous after the siting of a mysterious event and the ship's base speed is permanently increased by 1km an hour for the next 12 hours.
					//Poseidon's Bounty: The crew realizes there is an abundance of fish so you stop to cast nets and add additional units of food to your stores.
					int numOfEvents = Random.Range (1,5);
					switch (numOfEvents){
						//=====================	
						//FRIENDLY SHIP
						case 1:
							string finalMessage = "You encounter a ship asea--worried at first that it seems like pirates! Fortunately it appears to be"+
											      " a friendly ship who says hello!";
							string messageWaterModifier = "";
							string messageFoodModifier = "";
							//First determine if the player is low on food or water
							//--If the player is low on water
							if(ship.cargo[0].amount_kg <= 100){
								//add a random amount of water to the stores between 30 and 60 and modified by clout
								int waterBonus = Mathf.FloorToInt( Random.Range (30,60) * aggregateCloutScore);
								messageWaterModifier += " You received " + waterBonus + " kg of water ";
							
							}
							//--If the player is low on food
							if(ship.cargo[1].amount_kg <= 100){
								//add a random amount of food to the stores between 30 and 60 and modified by clout
								int foodBonus = Mathf.FloorToInt( Random.Range (30,60) * aggregateCloutScore);
								messageFoodModifier += "Thankfully you were given  " + foodBonus + " kg of food ";
								
							}
							
							//Determine which message to show based on what the ship did for you!
							//If there are stores given--let the player know
							if (messageWaterModifier != "" || messageFoodModifier !=""){
								finalMessage += "They notice you are low on supplies and offer a bit of their own!";
							} else {
								finalMessage += "All they can offer are food and water if you are in need, but your stores seem full enough!";
							}
							
							//Now add what food and water they give you to the message
							finalMessage += messageWaterModifier + messageFoodModifier + " They bid you farewell and wish Poseidon's favor upon you!";
							MGV.notificationMessage = finalMessage;
							MGV.showNotification = true;	
							break;
						//=====================	
						//ABADONED SHIP		
						case 2:
							finalMessage = "You come upon a derelict ship floating in the distance. Cautiously you approach it--wary of piracy--but the"+
							" smell of the salty air can't mask the stench from the corpses laying about...drying like grapes in the sun. Suddenly one of the bodies" +
							" had a voice and with his dying gasps, the sailor begs you to sink the ship with them aboard to make peace with Poseidon. Your crew" +
							" makes the final preparations, but search the ship's stores for anything useful.";
							
							//first determine the type of cargo to add to the ship's stores.
							int typeOfCargo = Random.Range(2,14);
							//Now determine how much cargo the player can hold
							int amountCanHold = (int) (ship.cargo_capicity_kg - ship.GetTotalCargoAmount());
							
							//If there is room on board(There will almost ALWAYS be some room) then tell the player how much they found
							if (amountCanHold > 0){
								int amountToAdd = (int) (Random.Range (1,amountCanHold) * aggregateCloutScore);
								finalMessage += "The crew finds " + amountToAdd + " kg of " + ship.cargo[typeOfCargo].name + ". What luck! I'm sure Poseidon won't mind if we just take a little something for our troubles! This should fetch a fair price at the market!";	
								ship.cargo[typeOfCargo].amount_kg += amountToAdd;
							} else {
								finalMessage += "The crew finds some " + ship.cargo[typeOfCargo].name + " but there isn't room on board! It's probably for the best--we shouldn't take from Poseidon.";
							}
							
							//now add the final bit to the event
							finalMessage += "We watch the ship sink as we sail away--ever mindful that if we aren't careful, the same could happen to us!";
							MGV.notificationMessage = finalMessage;
							MGV.showNotification = true;	
							break;
						//=====================	
						//FAVOR OF GODS		
						case 3:
						finalMessage = "Your crew suddenly felt uneasy causing you to drop anchor, but there were no pirates or storms in sight. "+
										"After prayers to Poseidon for your good fortune, a group of dolphins jump about your ship playing for a moment before disappearing. "+
										"Your crew takes it as a good sign and their spirits are lifted! As they begin to raise anchor, you notice the ship feels a bit faster than before."+
										" The waters seem to push you forward in a suspicious but fortunate manner!";
							shipSpeed_Actual++;
							MGV.notificationMessage = finalMessage;
							MGV.showNotification = true;
							break;
						//=====================	
						//POSEIDON'S BOUNTY		
						case 4:
						
							finalMessage = "The crew stirs you from deep contemplation to yell about an abnormal abundance of fish jumping out of the water--practically onto the boat itself!"+
							" They want to drop anchor and reap the bounty that Poseidon has deemed the crew worthy of! You agree to drop anchor and cast nets--all the while wary of the tricks" +
							" the gods play upon mortals.";
							
							//Determine how much cargo the player can hold
							amountCanHold = (int) (ship.cargo_capicity_kg - ship.GetTotalCargoAmount());
						
							//If there is room on board(There will almost ALWAYS be some room so let's say at least 50kg) then tell the player how much they found
							//if there is less than 50kg of room, but the ship is low on food, then the crew can have the food
							if (amountCanHold > 50 || ship.cargo[1].amount_kg <= 100){
								int amountToAdd = (int) (Random.Range (1,amountCanHold) * aggregateCloutScore);
								finalMessage += "The crew catches " + amountToAdd + " kg of food from the fish. What luck! Praise be to Poseidon! Hopefully this isn't one of his tricks!";	
								ship.cargo[2].amount_kg += amountToAdd;
							} else {
								finalMessage += " Suddenly you stop the crew, shouting that the stores are already full enough! It would be foolish to take the bounty--"+
								"obviously it is a test by Poseidon on our avarice! We continue on our journey--despite the grumblings of the crew.";
							}
						
							MGV.notificationMessage = finalMessage;
							MGV.showNotification = true;	
							break;
						//=====================	
						//ZEUS'S BOUNTY		
						case 5:
							
							finalMessage = "As you stare at cloudy skies, wondering if it's an ill omen, a ray of sun shoots through the clouds"+
								" and a calm light rain of fresh water pours on the ship and its crew! The crew asks you if they should drop anchor and catch the gift of water from Zeus!" +
									" You look over the cargo holds and check the supplies before answering--always cautious not to anger the gods.";
							
							//Determine how much cargo the player can hold
							amountCanHold = (int) (ship.cargo_capicity_kg - ship.GetTotalCargoAmount());
							
							//If there is room on board(There will almost ALWAYS be some room so let's say at least 50kg) then tell the player how much they found
							//if there is less than 50kg of room, but the ship is low on water, then the crew can have the water
							if (amountCanHold > 50 || ship.cargo[0].amount_kg <= 100){
								int amountToAdd = (int) (Random.Range (1,amountCanHold) * MGV.GetOverallCloutModifier(MGV.currentSettlement.settlementID));
								finalMessage += "The crew catches " + amountToAdd + " kg of water from the rain. What luck! Praise be to Poseidon! Hopefully this isn't one of his tricks!";	
								ship.cargo[2].amount_kg += amountToAdd;
							} else {
								finalMessage += " Suddenly you stop the crew, shouting that the stores are already full enough! It would be foolish to take the bounty--"+
									"obviously it's a gift to Zeus' brother--not to us and not worth the risk! We continue on our journey--despite the grumblings of the crew.";
							}
							
							MGV.notificationMessage = finalMessage;
							MGV.showNotification = true;	
							break;
					
					}
					
				}
				

				

				

			

			} 
			//If we do or don't get a random event, we should always get a message from the crew--let's call them tales
			//here they describe things like any cities nearby if the crew is familiar or snippets of greek mythology, or they
			//may be from a list of messages concering any nearby zones of influence from passing settlements/locations of interest
			ship.shipCaptainsLog.Add(MGV.currentLogPool[Random.Range (0, MGV.currentLogPool.Count)]);
			ship.shipCaptainsLog[ship.shipCaptainsLog.Count-1].dateTimeOfEntry = ship.totalNumOfDaysTraveled + " days";
			MGV.currentCaptainsLog = ship.shipCaptainsLog[ship.shipCaptainsLog.Count-1].dateTimeOfEntry + "\n" + ship.shipCaptainsLog[ship.shipCaptainsLog.Count-1].logEntry + "\n\n" + MGV.currentCaptainsLog;
		}
		
		
		//let's make sure the trigger for a new log  / event doesn't happen again until needed by
		//	by turning it off when the the trigger number changes--which means it won't take effect
		//	again until the next time the trigger number occurs
		Debug.Log (Mathf.FloorToInt(tenthPlaceTemp));
		if(Mathf.FloorToInt(tenthPlaceTemp) != 5 && Mathf.FloorToInt (tenthPlaceTemp) != 9) MGV.isPerformingRandomEvent = false;
		
	}
	
	void UpdateShipSpeed(){
		//Figure out the crew modifier for speed -- we need to make the crew a ratio to subtract 
		//	--crew modifier is ((totalPossibleCrew / 100) * current#OfCrew ) / 10 --this gives a percentage of crew available
		//	--we multiply this percentage by the ship speed e.g. 7.5km/h  * (50% crew--.5)
		// 	--and then subtract that amount from the total speed to get our modifier
		shipSpeed_CrewModifier =  ship.speed - (ship.speed * ((((float)ship.crewCapacity/100f) * (float)ship.crewRoster.Count) / 10) ); 
		//Debug.Log (ship.speed + "  *  (" + ship.crewCapacity/100f + " * " + ship.crewRoster.Count);
		
		//Figure out the Hunger and Thirst modifier for speed
		//	--while hungry, ship speed is reduced by 25 % from the original top speed
		if (dayCounterStarving >= 1) shipSpeed_HungerModifier = ship.speed * .15f;
		else shipSpeed_HungerModifier = 0f;
		//	--while thirsty, ship speed is reduced by 25 % from the original top speed
		if (dayCounterThirsty >= 1)  shipSpeed_ThirstModifier = ship.speed * .15f;
		else shipSpeed_ThirstModifier = 0f;
		
		//Figure out the Ship Health Modifier for speed
		//	--every point of hp missing reduces the speed by that many percentage points
		//	--this only takes effect after 10 hp are lost, e.g. 91-100hp the ship's speed will still be unchanged from damage
		//	--once the ship hits 90hp, the speed will immediately be reduced by 10% and 1% for every point of damage sustained after
		if (ship.health <= 90f) shipSpeed_ShipHPModifier = (ship.speed / 100f) * (100f - ship.health);
		else shipSpeed_ShipHPModifier = 0f;
		
		//Update the current actual speed of the ship
		shipSpeed_Actual = ship.speed - shipSpeed_CrewModifier - shipSpeed_HungerModifier - shipSpeed_ThirstModifier - shipSpeed_ShipHPModifier;
	
		//Always makes sure the ship does not have a negative speed--keep it at zero if it goes negative
		if (shipSpeed_Actual < 0) shipSpeed_Actual = 0;
	}
	
	void CheckIfShipLeftPortStarvingOrThirsty(){
	//We need to check to see if there is enough food for a single days journey for all the crew
	//if there isn't--some of the crew will leave the ship
	
	string notificationMessage = "";
	bool foodDeaths = false;
	
		if(ship.cargo[1].amount_kg < dailyFoodKG * ship.crewRoster.Count){
			//start a counter of how many crew members die
			int crewDeathCount = 0;
			//make sure we roll for each crew member
			for (int i = 0; i < ship.crewRoster.Count; i++){
				//--we'll base this chance off of a static 30% plus any effect from clout(a maximum change of 25 % in either direction
				float rollChance = 30 - (((MGV.playerShipVariables.ship.playerClout - 50f) / 100)/2);
				//some crew needs to leave
				if (Random.Range(0,100) <= rollChance) {
					//Kill a crewmember
					KillCrewMember();
					crewDeathCount++;
					MGV.showNotification = true;
					foodDeaths = true;
				}
			}
			notificationMessage += crewDeathCount + " crewmember(s) quit because you left without a full store of food";
		}
		if (foodDeaths) notificationMessage += ", and ";
		//We need to check to see if there is enough water for a single days journey for all the crew
		//if there isn't--some of the crew will leave the ship
		if(ship.cargo[0].amount_kg < dailyWaterKG * ship.crewRoster.Count){
			//start a counter of how many crew members die
			int crewDeathCount = 0;
			//make sure we roll for each crew member
			for (int i = 0; i < ship.crewRoster.Count; i++){
				//--we'll base this chance off of a static 30% plus any effect from clout(a maximum change of 25 % in either direction
				float rollChance = 30 - (((MGV.playerShipVariables.ship.playerClout - 50f) / 100)/2);
				//some crew needs to leave
				if (Random.Range(0,100) <= rollChance) {
					//Kill a crewmember
					KillCrewMember();
					crewDeathCount++;
					MGV.showNotification = true;
				}
			}
			notificationMessage += crewDeathCount + " crewmember(s) quit because you left without a full store of water.";
		}
		//now update the notification string with the message
		MGV.notificationMessage = notificationMessage;
	}

	public void UpdateDayNightCycle(bool restartCycle){
	//This Function Updates the Day and Night Cycle of the game world.
	//The time is split up by days, so there are .5 days of sunlight and .5 days of no light
	//We'll update the directional light sources intensity and color to reflect the changes
	//There are 3 different light changes:
	//	--Day Light: full light / yellow orange tint  0 days - .25 days
	//	--Morning/Evening: half light / slight blue tinge .25 days - .5 days   / .75 days - 0 days
	//	--Nightime: low light / blue tint  .5 - .75 days
	//The measurements will chop off all numbers left of the decimal and rotate the light sources qualities based on a .0 - .99 scale
	//	--to figure out the formulas for blending, I solved for 2 linear equations: e.g.
	//	-- For the Day 2 Night blend on the main light intensity
	//	-- .25x + y = .78   *when the time of day is .25, the answer should be .78
	//	-- .5x + y  = 0		*when the time of day is .5, the answer should be 0
	//		--solving these equations yields x = -3.12 and y = 1.56
	//		--Our combined formula that provides a range of answers between 0 and .78 with TimeOfDay as the only variable is:
	//		--LightIntensity = (TimeOfDay * -3.12) + 1.56
	//	--This formula logic was used for all blending-- it creates a 'smooth' range of numbers to blend through
	
	
	//First let's set up our directional light attributes
	float intensityDay = .78f;
	float intensityNight = .78f;
	float intensityDuskDawn = 0f;
	
	float intensityDayAmbient = .53f;
	float timeModifier = ship.totalNumOfDaysTraveled;
	
		Color colorDay = new Color(255f/255f,235f/255f,169f/255f);
		Color colorNight = new Color(0f/255f,192f/255f,255f/255f);
		Color colorDuskDawn = new Color(0f/255f,0f/255f,0f/255f);
	
		Color waterColorDay = new Color(48f/255f,141f/255f,255f/255f,.97f);
		Color waterColorNight = new Color(0f/255f,21f/255f,8f/255f,.97f);
		
		Color currentColorNight = new Color (0/255f, 37/255f, 12/255f, 1f);
		//Debug.Log (ship.totalNumOfDaysTraveled + "    NUM OF DAYS TRAVELED!!!!");
		float timeOfDay = (timeModifier - Mathf.Floor(timeModifier)); // this removes the numbers left of the decimal so we can work with the fraction
		float testAngle = 0f;
		
		//TODO add rotational light possible to reflect the sun rising / setting
		//rotatingLight.transform.localEulerAngles = new Vector3((timeOfDay*360)-90,0,0);
		
		//First set things to default if it's a new game we've started
		//	--This ensures any editor changes are reset. Since it sets to '0' the blends will not activate 
		if( restartCycle ){
			MGV.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Blend", 1f);
			//RenderSettings.skybox.SetFloat("_Blend", 0);
			RenderSettings.ambientIntensity = .53f;
			MGV.mainLightSource.intensity = .78f;
			MGV.mainLightSource.color = colorDay;
			MGV.mat_water.SetColor("_Color", waterColorDay);
			MGV.mat_waterCurrents.color = Color.white;
			testAngle = MGV.skybox_MAIN_CELESTIAL_SPHERE.transform.localRotation.y;
			Debug.Log (initialAngle + "*********************");
			MGV.skybox_horizonColor.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, 1f, 1f, 1f);
			MGV.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f,1f,1f);
			MGV.skybox_sun.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, 1f, 1f);
			MGV.skybox_moon.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, 1f, 1f, .5f);
			RenderSettings.fogColor = new Color(203f/255f,239f/255f,254f/255f);
			fogWall.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(203f/255f,239f/255f,254f/255f);
			
		}
		
		Color deepPurple = new Color(82f/255f,39f/255f,101f/255f);
		Color brightSky = new Color(203f/255f,239f/255f,254f/255f);
		//Blending Day to Night
		if(timeOfDay >= .25f && timeOfDay <= 0.5f) {
			MGV.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Blend", GetRange(timeOfDay, .5f, .25f, 0, 1f));
			RenderSettings.ambientIntensity = (timeOfDay*-2.12f) + 1.06f;
			MGV.mainLightSource.intensity = (timeOfDay*-3.12f) + 1.56f;
			MGV.mainLightSource.color = Color.Lerp(colorDay, colorNight, GetRange(timeOfDay, .5f, .25f, 1, 0));
			//Fade Out Water Colors
			MGV.mat_water.color = Color.Lerp(waterColorDay, waterColorNight, GetRange(timeOfDay, .5f, .25f, 1f, 0));
			//Fade Out Water Current Sprite Colors to Black
			MGV.mat_waterCurrents.color = Color.Lerp(Color.white, currentColorNight, GetRange(timeOfDay, .5f, .25f, 1f, 0));
			//Fade Out Sky/Atmosphere Color
			MGV.skybox_horizonColor.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, GetRange(timeOfDay, .5f, .25f, 0, 1f), 1f, GetRange(timeOfDay, .5f, .25f, 0, 1f));
			//Fade Out Sun Color
			MGV.skybox_sun.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, GetRange(timeOfDay, .5f, .25f, 70f/255f, 1f), GetRange(timeOfDay, .5f, .25f, 0, 1f));
			//Fade Out Clouds
			MGV.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(GetRange(timeOfDay, .5f, .25f, 30f/255f, 1f),GetRange(timeOfDay, .5f, .25f, 30f/255f, 1f),GetRange(timeOfDay, .5f, .25f, 50f/255f, 1f));
			//Fade In Moon(transparency to opaque)
			MGV.skybox_moon.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f,1f,1f,GetRange(timeOfDay, .5f, .25f, 1f, 28f/255f));
			//Fade in Dark Fog: This breaks up the fog colro fade into two shades to better match the sunset
			if(timeOfDay >= .25f && timeOfDay <= 0.35f) {
				RenderSettings.fogColor = Color.Lerp(brightSky, deepPurple, GetRange(timeOfDay, .35f, .25f, 1f, 0));
				fogWall.GetComponent<MeshRenderer>().sharedMaterial.color = Color.Lerp(brightSky, deepPurple, GetRange(timeOfDay, .35f, .25f, 1f, 0));
			}  else {
				//Also we';; turn on the city lights here right as sunset
				MGV.cityLightsParent.SetActive(true);
				RenderSettings.fogColor = Color.Lerp(deepPurple, waterColorNight, GetRange(timeOfDay, .5f, .35f, 1f, 0));
				fogWall.GetComponent<MeshRenderer>().sharedMaterial.color = Color.Lerp(deepPurple, waterColorNight, GetRange(timeOfDay, .5f, .35f, 1f, 0));
			}
		
		}
		//Blending Night to Day
		if(timeOfDay > 0.75f && timeOfDay <= 1f) {
			MGV.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Blend", GetRange(timeOfDay, 1f, .75f, 1f, 0));
			RenderSettings.ambientIntensity = (timeOfDay*2.12f) - 1.59f;
			MGV.mainLightSource.intensity = (timeOfDay*3.12f)-2.34f;
			MGV.mainLightSource.color = Color.Lerp(colorNight, colorDay, GetRange(timeOfDay, 1f, .75f, 1, 0));
			//Fade In Water Colors
			MGV.mat_water.color = Color.Lerp(waterColorNight, waterColorDay, GetRange(timeOfDay, 1f, .75f, 1f, 0));
			//Fade Out Water Current Sprite Colors to Black
			MGV.mat_waterCurrents.color = Color.Lerp(currentColorNight, Color.white, GetRange(timeOfDay, 1f, .75f, 1f, 0));
			//Fade In Sky/Atmosphere Color
			MGV.skybox_horizonColor.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, GetRange(timeOfDay, 1f, .75f, 1f, 0), 1f, GetRange(timeOfDay, 1f, .75f, 1f, 0));
			//Fade In Sun Color
			MGV.skybox_sun.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, GetRange(timeOfDay, 1f, .75f, 1f, 70f/255f), GetRange(timeOfDay, 1f, .75f, 1f, 0));
			//Fade In Clouds
			MGV.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(GetRange(timeOfDay, 1f, .75f, 1f, 30f/255f),GetRange(timeOfDay, 1f, .75f, 1f, 30f/255f),GetRange(timeOfDay, 1f, .75f, 1f, 50f/255f));
			//Fade out Moon(opaque to transparency)
			MGV.skybox_moon.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f,1f,1f,GetRange(timeOfDay, 1f, .75f, 28f/255f, 1f));
			//Fade in Normal Fog: This breaks up the fog colro fade into two shades to better match the sunrise
			if(timeOfDay >= .75f && timeOfDay <= 0.85f) {
				RenderSettings.fogColor = Color.Lerp(waterColorNight, deepPurple, GetRange(timeOfDay, .85f, .75f, 1f, 0));
				fogWall.GetComponent<MeshRenderer>().sharedMaterial.color = Color.Lerp(waterColorNight, deepPurple, GetRange(timeOfDay, .85f, .75f, 1f, 0));
			}  else {
				//Also we';; turn off the city lights here right as sun rises
				MGV.cityLightsParent.SetActive(false);
				RenderSettings.fogColor = Color.Lerp(deepPurple, brightSky, GetRange(timeOfDay, 1f, .85f, 1f, 0));
				fogWall.GetComponent<MeshRenderer>().sharedMaterial.color = Color.Lerp(deepPurple, brightSky, GetRange(timeOfDay, 1f, .85f, 1f, 0));
			}		}
		//------------------------ Rotate the sky for day night cycle
		targetAngle = GetRange(timeOfDay, 1f, 0, 360+testAngle, testAngle);
		MGV.skybox_MAIN_CELESTIAL_SPHERE.transform.Rotate(0, targetAngle - initialAngle, 0, Space.Self);
//		//Debug.Log (initialAngle +  "***********" + targetAngle);
		RotateClouds(targetAngle - initialAngle);
		initialAngle = targetAngle;
		

		
	}
	
	float GetRange(float Xinput, float Xmax, float Xmin, float Ymax, float Ymin){

	return ( ((Xinput - Xmin) / (Xmax - Xmin)) * (Ymax - Ymin) ) + Ymin;

	}
	
	public void RotateCelestialSky(){
		//We need to get the players latitude to determine the vertical angle of the celestial globe
		//	--This is the angle of the north celestial pole from the horizon line
		//	--This is our x Angle of the sphere--0 degrees in Unity is the same as 90 degrees of latitude
		Vector2 playerLatLong = MGV.ConvertWebMercatorToWGS1984(MGV.Convert_UnityWorld_WebMercator(transform.position));
		Transform celestialSphere = MGV.skybox_MAIN_CELESTIAL_SPHERE.transform;
		float latitude = playerLatLong.y;
		
		float targetAngle  = GetRange(latitude, 90f, -90f, 0, -180);//(90 - latitude);
		float angleChange = initialCelestialAngle - targetAngle;
		initialCelestialAngle = targetAngle;
		
		celestialSphere.transform.Rotate( angleChange, 0, 0, Space.World);
		//Debug.Log (angleChange);
	}	
	
	public void CalculatePrecessionOfEarth(){
	
		
		//Calculate the position of the precession around the ecliptic orbit
		
		// Julian Date in days set to J2000 epoch
		double JD_J2000 = 2457455.500000; 
		//Calculate the current in-game Julian Date
		double JD_inGame = PropleticGregorianToJulianDateCalculator(MGV.TD_year, MGV.TD_month, MGV.TD_day, MGV.TD_hour, MGV.TD_minute, MGV.TD_second);
		//Find the difference between the J2000 Julian Date and the in-Game Date
		double numOfJulianDaysBeforeOrAfterJ2000 = Mathf.Abs((float)(JD_J2000 - JD_inGame));
		//Debug.Log ("JD: DIFFERENCE:  " + numOfJulianDaysBeforeOrAfterJ2000);
		//Determine whether the difference is Before (-) or After (+) J2000
		if (JD_inGame < JD_J2000) numOfJulianDaysBeforeOrAfterJ2000 *= -1;
		//Convert the difference to Julian Centuries
		double numOfJulianCenturiesBeforeOrAfterJ2000 = numOfJulianDaysBeforeOrAfterJ2000 / 36525;
		//Debug.Log ("JD: IN GAME:  " + JD_inGame + "   NUM OF CENTURIES:   " + numOfJulianCenturiesBeforeOrAfterJ2000);
		
	//	double numOfJulianCenturies = test_numOfJulianCenturies;
		float totalPrecession = CalculateEarthPrecessionAngle(numOfJulianCenturiesBeforeOrAfterJ2000);
		test_processionSpeed = totalPrecession;
		float angleArcSec = totalPrecession;//
		//Debug.Log (angleArcSec/3600);
	
	//we need to calculate a point in space for the local y axis of the 'earth' to 'look at'
	float precessionAngle = 0; // in degrees
	//precessionAngle = testPrecessionAngle;
	precessionAngle = (angleArcSec/3600);
	
	float earthTilt = 23.44f; // in degrees
	
	float radius = 3000; //this value is arbitrary and dependent upon the y-value of the precession ring
	float precessionDiscDistance = radius / Mathf.Tan(earthTilt*Mathf.Deg2Rad); //this is a constant and will not change--it represents the height, in world space, that the precession ring exists
				  //	--this is largely arbitrary and a higher value is used for visualization purposes
				  // To calculate the radius or height(distance from player origin) we can use the conical formula:
				  //	--  radius = coneHeight * tan(earthTilt)
				  //	--  coneHeight = radius / tan(earthTilt)
				  //	--Since the radius is used for more calculations, I want to determine an easier to use number e.g. 1, or 3000
				  //	--height isn't used as much
	//The direction from the player ship to the disc will always be the ecliptic sphere's LOCAL y axis
	Vector3 precessionDiscDirection = MGV.skybox_ecliptic_sphere.transform.up;
				//The local cordinates for a point on the circumference in the model are:
				//float horizontalDistance = radius * Mathf.Cos(precessionAngle*Mathf.Deg2Rad);
				//float forwardDistance = radius * Mathf.Sin(precessionAngle*Mathf.Deg2Rad);
	//Now we need to find this center of the conic base in unity world coordinates
	//	--We can achieve this by shooting a ray from the player ship to the direction
	//	--of the ecliptic's local y axis(which always intersects the center of the cone(a right cone)
	Ray rayToDiscOrigin = new Ray (transform.position, precessionDiscDirection);
	Vector3 discCenter_World = rayToDiscOrigin.GetPoint(precessionDiscDistance);
	//Now we need to find the point on the circumference
	//	--To do this, we'll rotate a Vector3 that points towards the local transform.left : (-) right
	//	--by the precession angle, along the local y axis
	Vector3 direction_DiscOriginToXYZOnRing = -MGV.skybox_ecliptic_sphere.transform.right;
	direction_DiscOriginToXYZOnRing = Quaternion.AngleAxis(precessionAngle, MGV.skybox_ecliptic_sphere.transform.up) * direction_DiscOriginToXYZOnRing;
	//Now we need to shoot a ray from the center of the precession conical base, in the direction
	//	--of this new angle in the distance of the radius of the base to find the Unity World
	//	--coordinate of the point along the precessional ring/disc/conical base.
	Ray rayFromDiscOriginToXYZOnRing =  new Ray (discCenter_World, direction_DiscOriginToXYZOnRing);
	Vector3 worldCoordinateForRingXYZ = rayFromDiscOriginToXYZOnRing.GetPoint(radius);
			//Debug.Log (worldCoordinateForRingXYZ);
	//Because the world is 'mariner-centric' in that the ship is always at the center,
	//	--we need to point the celestial sphere's local.y axis from the ship's current position
	//	--to the world coordinate of the point on the ring we just discovered
	Vector3 OriginPosition = transform.position; //the player ship will always reside at the origin of space
	Vector3 PPGoal = worldCoordinateForRingXYZ;
	Vector3 direction = PPGoal - OriginPosition;
	Quaternion toRotation = Quaternion.FromToRotation(transform.up, direction);
	MGV.skybox_MAIN_CELESTIAL_SPHERE.transform.DetachChildren();
	MGV.skybox_celestialGrid.transform.rotation = toRotation;
	
	//testing....This fixed the error with changing time and the rotation not working--I'm not sure why this works..It just does
	//need to figure this blackbox I made out later. it was one experiment of many
	MGV.skybox_MAIN_CELESTIAL_SPHERE.transform.rotation = MGV.skybox_celestialGrid.transform.rotation;
	MGV.skybox_ecliptic_sphere.transform.SetParent(MGV.skybox_MAIN_CELESTIAL_SPHERE.transform);	
	MGV.skybox_celestialGrid.transform.SetParent(MGV.skybox_MAIN_CELESTIAL_SPHERE.transform);
	//MGV.skybox_sun.transform.SetParent(MGV.skybox_MAIN_CELESTIAL_SPHERE.transform);	
	MGV.skybox_MAIN_CELESTIAL_SPHERE.transform.localEulerAngles = new Vector3(MGV.skybox_MAIN_CELESTIAL_SPHERE.transform.localEulerAngles.x,0,0);
	
	}
	
	float CalculateEarthPrecessionAngle(double numOfCenturiesBeforeOrAfterJ2000Epoch){
	//This calculation for the total precession of the Earth is referenced from:
	//	--N. Capitaine et al 2003. Expressions for IAU 2000 precession quantities. DOI: 10.1051/0004-6361:20031539
		//--Their formula is: pA=502800".796195t + 1".1054348t^2 + 0".00007964t^3 - 0".000023857t^4 - 0".0000000383t^5
		//--Where pA is the total precession in arcseconds, and t is time in Julian Centuries +- J2000
		//The constant term of this speed (5,028.796195 arcseconds per century in above equation) 
		//	--corresponds to one full precession circle in 25,771.57534 years (one full circle of 360 degrees 
		//	--divided with 5,028.796195 arcseconds per century)
		//This is formula (39) on p.581
	
	double JDtcb = 2457455.500000; // Julian Date in Time--right now this is set to J2000 epoch
	double precessionSpeed;
	
	//Time in Julian Centuries T = 1  -> 36525 days  (365.25 days X 100) before or after J2000 epoch
	double T = numOfCenturiesBeforeOrAfterJ2000Epoch; 
	double arcSecConstant = 5028.796195;
	
	precessionSpeed = (arcSecConstant*T) + (1.1054348 * T*T) + (0.00007964*T*T*T) - (0.000023857*T*T*T*T) - (0.0000000383*T*T*T*T*T);
	
	return (float) precessionSpeed;
	
	}
	
	double PropleticGregorianToJulianDateCalculator(string str_y, string str_mon, string str_d, string str_h, string str_min, string str_s){
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//THIS FUNCTION IS NOT A JULIAN --CALENDAR-- DATE CALCULATER
		//	--It converts to the Astronomical JULIAN DATE standard or JD
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//Debug.Log (str_y);
		float year = (float) int.Parse(str_y);
		float month = (float) int.Parse(str_mon);
		float day = (float) int.Parse(str_d);
		float hour = (float) int.Parse(str_h);
		float minute = (float) int.Parse(str_min);
		float second = (float) int.Parse(str_s);
		
		double JDN = 0; // Julian Day Number e.g. 20000
		double JD = 0; //full Julian Date e.g. 20000.5
		//All years in BC must be converted to Astronomical years: 1 BC == 0, 2 BC == -1, etc. a 1 day ++ increment will work
		float BCModifier = 0;
		if (year < 0) year++;
		
		float a = Mathf.Floor(14-month/12);	//where a = monthly offset
		float y = year + 4800 - a;			//where y = years
		float m = month + (12 * a) - 3;		//where m = months

		
		//Then perform the Gregorian Calendar Conversion to JDN(Julian Day Number)
		JDN = day + Mathf.Floor(((153*m)+2)/ 5) + (365*y) + Mathf.Floor(y/4) - Mathf.Floor(y/100) + Mathf.Floor(y/400) - 32045; 		
		//Now Convert the JDN to the Julian Date by accounting for the hours/minutes/seconds
		JD = JDN + ((hour-12)/24) + (minute/1440) +  (second/86400);
		
		return JD;
	}
	
	void RotateClouds(float angle){
		MGV.skybox_clouds.transform.RotateAround(transform.position, Vector3.left, angle/4);
		MGV.skybox_clouds.transform.position = transform.position;
	}
	
	public void UpdatePlayerGhostRouteLineRenderer(bool isANewGame){
		//Update the Line Renderer with the last route position added. If a new game we initially set the origin point to the players position
		//We have to take this offset into account later because the player route array will always have 1 less in the array because it doesn't have the origin position as a separate route index(it's not a route)
		//	--rather than use Count-1 to get the last index of the line renderer, we can just use Count from the route log
		if(isANewGame){
			MGV.playerGhostRoute.GetComponent<LineRenderer>().SetVertexCount(1);
			MGV.playerGhostRoute.GetComponent<LineRenderer>().SetPosition(0, transform.position - new Vector3(0,transform.position.y,0)); //We subtract the y value so that the line sits on the surface of the water and not in the air
		//TODO this is a quick and dirty fix to load games--the origin point is already established in a loaded game so if we add 1 to the index, it creates a 'blank' Vector.zero route index in the ghost trail
		} else if (MGV.isLoadedGame) {
			MGV.playerGhostRoute.GetComponent<LineRenderer>().SetVertexCount(journey.routeLog.Count);
			MGV.playerGhostRoute.GetComponent<LineRenderer>().SetPosition(journey.routeLog.Count-1, journey.routeLog[journey.routeLog.Count-1].theRoute[1] - new Vector3(0,transform.position.y,0));
		//if it isn't a loaded game then do the original code
		} else {
			MGV.playerGhostRoute.GetComponent<LineRenderer>().SetVertexCount(journey.routeLog.Count+1);//we add one here because the route list never includes the origin position--so we add it manually for a new game
			MGV.playerGhostRoute.GetComponent<LineRenderer>().SetPosition(journey.routeLog.Count, journey.routeLog[journey.routeLog.Count-1].theRoute[1] - new Vector3(0,transform.position.y,0));//we always use the destination coordinate of the route, because the origin point was already added the last time so [1] position		
		}
	}
	
	
	void CalculateShipTrajectoryPreview(Vector3 destination){
		Vector3 shipPosition = transform.position;
		bool isCalculatingRoute = true;
		float shipSpeedMagnitude = 0;
		Vector3 travelDirection = Vector3.zero;
		float distance = 0f;
		Vector3 windAndWaterVector = Vector3.zero;
		Vector3 lastPlayerPosition = Vector3.zero;
		List<Vector3> trajectoryToDraw = new List<Vector3>();
		
		int infiniteLoopBreak  = 0;
		
		//Add the first point on the route--the player's current position
		trajectoryToDraw.Add(shipPosition - new Vector3(0,shipPosition.y,0));
		
		//Now we need to figure out the actual trajectory along the chosen path 
		while (isCalculatingRoute){
			//Get direction to settlement from ship
			travelDirection = Vector3.Normalize(destination - shipPosition);
			distance = Vector3.Distance(destination , shipPosition);
			windAndWaterVector = GetCurrentWindWaterForceVector(travelDirection);//(currentWaterDirectionVector * currentWaterDirectionMagnitude) + (currentWindDirectionVector * currentWindDirectionMagnitude);
			//calculate speed of ship with wind vectors
			shipSpeedMagnitude = ((travelDirection * shipSpeed_Actual) + windAndWaterVector).magnitude; 
			//save the last position of the ship
			lastPlayerPosition = shipPosition;
			
			
		
			
			//if the ship hasn't gotten to the destination, then keep moving
			if (distance > .03f) { //This .03f value determines how close the trail will be to the selected spot
				//We need to make sure the ship has the speed to go against the wind and currents--if it doesn't then we need to stop traveling
				//	--we will check the angle between the final movement vector and the destination
				//	--if it's more than 160 degrees, then stop the ship (more than 160 can be buggy)
				playerMovementVector = ( (travelDirection * shipSpeed_Actual) + windAndWaterVector) * shipSpeed_Game_Modifier/20 ; //This / 20  value is to set the resolution of the line--e.g. the slower the ship, the more segments to the line
				//Debug.Log (Vector3.Angle(travelDirection, playerMovementVector));
				if (Vector3.Angle(travelDirection, playerMovementVector) < 160f){
					//playerMovementMagnitude = playerMovementVector.magnitude;
					//Move the ship
					shipPosition += playerMovementVector;
					trajectoryToDraw.Add (shipPosition - new Vector3(0,shipPosition.y,0));
				} else{
					isCalculatingRoute = false;
				}
			} else {
				isCalculatingRoute = false;
			}
			//Make sure we break the while loop if it iterates over 100 times so we add the counter here
			infiniteLoopBreak++;
			if(infiniteLoopBreak > 250){ isCalculatingRoute = false;}
			
		}
		
		//Now draw the actual trajectory to the screen
		LineRenderer line = MGV.playerTrajectory.GetComponent<LineRenderer>();
		line.SetVertexCount(trajectoryToDraw.Count);
		for (int i = 0; i < trajectoryToDraw.Count; i++){
			line.SetPosition(i, trajectoryToDraw[i]);
		}
		
		
	}
	
	public void UpdateNavigatorBeaconAppearenceBasedOnDistance(){
		//Get position of player and beacon
		Vector3 playerPos = transform.position;
		Vector3 beaconPos = MGV.navigatorBeacon.transform.position;
		//Get Distance
		float distance = Vector3.Distance(playerPos, beaconPos);
		
		//If the distance is less than 100, start fading the beacon to transparent, and fading it's size and reverse
		//	that if it is 100. We'll be using the same formula to fade the celestial sphere colors.
		
		//Update size
		float calculatedWidth = GetRange(distance, 0, 100f, .1f, 5f);
		MGV.navigatorBeacon.GetComponent<LineRenderer>().SetWidth(calculatedWidth, calculatedWidth);
		
		Color colorEnd = new Color(6f/255f,167f/255f,1f,0);
		
		//Update transparency
		float alpha = GetRange(distance, 0, 100f, 0, 1f);
		MGV.navigatorBeacon.GetComponent<LineRenderer>().SetColors(new Color (88f/255f,1f,211/255f,alpha),colorEnd);
	}
	
	
	public Vector3 GetCurrentWindWaterForceVector(Vector3 travelDirection){
		Vector3 windAndWaterVector = Vector3.zero;
		Vector3 windVector = Vector3.zero;
		Vector3 waterVector = Vector3.zero;
		
		if(!rayCheck_stopCurrents) waterVector = currentWaterDirectionVector;
		if (MGV.sailsAreUnfurled)  windVector = currentWindDirectionVector;
		
		windAndWaterVector = (waterVector * currentWaterDirectionMagnitude) + (windVector * currentWindDirectionMagnitude);
		
		//This sets the ship's actual speed once the vectors have been taken into account. The value is stored in a public variable accessible by all functions
		current_shipSpeed_Magnitude = ((travelDirection * shipSpeed_Actual) + windAndWaterVector).magnitude; 
		
		return windAndWaterVector;
	}
	
	public void AnimateCursorRing(){
	
		//First determine if enough time has passed to animate the sprite
		if (Time.time > cursorRingAnimationClock){
			Vector2 newOffset = cursorRingMaterial.mainTextureOffset;
			//First change the color of the cursor if needed by switching rows on the sprite sheet
			if (cursorRingIsGreen) newOffset.y = 0.5f;
			else newOffset.y = 0f;
			//Now animate each frame (there are 4) When the last frame is reached (.75) reset to 0 rather than infinitely climbing
			if (newOffset.x == .875f) newOffset.x = 0f;
			else newOffset.x += .125f;
			//Set the new offset
			cursorRingMaterial.mainTextureOffset = newOffset;
			//Update the animation clock
			cursorRingAnimationClock = Time.time + .06f;
		}
		
		//Now let's animate its scale to create a 'bobbing' effect
//		if (cursorRingIsGrowing){
//			if (cursorRing.transform.localScale.x >= .1f + cursorRing.transform.localScale.x) cursorRingIsGrowing = false;
//			else cursorRing.transform.localScale += new Vector3(.001f,.001f,.001f);
//		}else {
//			if (cursorRing.transform.localScale.x <= .05f + cursorRing.transform.localScale.x) cursorRingIsGrowing = true;
//			else cursorRing.transform.localScale -= new Vector3(.001f,.001f,.001f);
//		}
		 //Make sure the cursor's z axis is always facing the player's camera
		 cursorRing.transform.LookAt(MGV.FPVCamera.transform.position);
		 //cursorRing.transform.eulerAngles = -cursorRing.transform.eulerAngles; 
		 cursorRing.transform.eulerAngles = new Vector3(105f,cursorRing.transform.eulerAngles.y,-1*cursorRing.transform.eulerAngles.z);
	}
	
	
	public void CheckIfPlayerFoundKnownSettlementAndTurnGhostTrailBackOn(int ID){
	//Check if the current settlement approach area is part of the player's knowledge network--if it is, then turn the memory
	//ghost route back on and alert the player to finding their way back to known locations
		foreach (int id in ship.playerJournal.knownSettlements){
		Debug.Log (id + " =? " + ID);
			//if we find a match then turn the memory ghost route back on
			if (id == ID){
			Debug.Log ("Found match in memory lookup");
				string settlementName = MGV.GetSettlementFromID(id).name;
				MGV.playerGhostRoute.SetActive(true);
				MGV.notificationMessage = "After a long and difficult journey, you and your crew finally found your bearings in the great sea!" +
										  " You and your crew recognize the waters surrounding " +settlementName+ " and remember the sea routes," +
										  " you are all familiar with!";
				MGV.showNotification = true;
				break;
			}
		}
	}
	
	
	public void DetectCoastLinesWithRayCasts(){
	//This function Detects coast lines from 3 separate distances:
	//	--long range: roughly 15km detects coast lines to determine if seagull sound occurs
	//	--medium range: if the player is within 4km of the coast line they will no longer be affected by sea currents--this represents the benefit of sticking close to coastlines vs going asea
	//	--short range : detects if coast line is super close and then stops the player from moving if it's the case(this is to prevent the player clipping into terrain or land)
	//It shoots 8 rays from the center of the ship at equal 45 deg angles--more than 8 is probably not necessary, but more can be added if necessary
	
	//We'll shoot starting from the 12 o clock position and work counter clock wise
	//Every time the function is called the bools are reset to false--they will turn true if ANY ray cast fits the criteria
		float maxDistance = 25f;
		float hitDistance = 0;
		float distCoastLine = .1f;
		float distStopCurrent = 4f;
		bool stopShip = false;
		bool stopCurrents = false;
		bool playBirdSong = false;
		//set the layer mask to only check for collisions on layer 10 ("terrain")
		int terrainLayerMask = 1 << 10;
		RaycastHit hitInfo;
		Vector3 rayOrigin = transform.position + new Vector3(0,-.23f,0);
		//12 O clock
		//Debug.DrawRay(rayOrigin, transform.forward*maxDistance, Color.yellow);
		if (Physics.Raycast(rayOrigin, transform.forward, out hitInfo, maxDistance, terrainLayerMask )){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
			//If less than 1km~ stop ship movement
				stopShip = true;// Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
			//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
			//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
		//1.5 o Clock
		}
		if (Physics.Raycast(rayOrigin, transform.forward + transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
		//3 o clock
		} 
		if (Physics.Raycast(rayOrigin, transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
		//4.5 o clock	
		} 
		if (Physics.Raycast(rayOrigin, -transform.forward + transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
		//6 o clock	
		} 
		if (Physics.Raycast(rayOrigin, -transform.forward, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
		//7.5 o clock		
		} 
		if (Physics.Raycast(rayOrigin, -transform.forward - transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
		//9 o clock	
		} 
		if (Physics.Raycast(rayOrigin, -transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
		//10.5 o clock		
		} 
		if (Physics.Raycast(rayOrigin, transform.forward - transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
		}
		
		//now set the main bool flags to the temp flags
		rayCheck_stopShip = stopShip;
		rayCheck_stopCurrents = stopCurrents;
		rayCheck_playBirdSong = playBirdSong;	
	}
	
	public void KeepNearestWindAndCurrentZonesOn(){
	//All this function does is send out two raycasts in 8 directions, one for wind zones and one for water current zones.
	//If it hits one, it sends a flag to keep it turned on.
		float maxDistance = 25f;
		float hitDistance = 0;
		float distCoastLine = .1f;
		float distStopCurrent = 4f;
		bool stopShip = false;
		bool stopCurrents = false;
		bool playBirdSong = false;
		//set the layer mask to only check for collisions on layer 10 ("terrain")
		int terrainLayerMask = 1 << 10;
		RaycastHit hitInfo;
		Vector3 rayOrigin = transform.position + new Vector3(0,-.23f,0);
		//12 O clock
		//Debug.DrawRay(rayOrigin, transform.forward*maxDistance, Color.yellow);
		if (Physics.Raycast(rayOrigin, transform.forward, out hitInfo, maxDistance, terrainLayerMask )){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;// Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//1.5 o Clock
		}
		if (Physics.Raycast(rayOrigin, transform.forward + transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//3 o clock
		} 
		if (Physics.Raycast(rayOrigin, transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//4.5 o clock	
		} 
		if (Physics.Raycast(rayOrigin, -transform.forward + transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//6 o clock	
		} 
		if (Physics.Raycast(rayOrigin, -transform.forward, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//7.5 o clock		
		} 
		if (Physics.Raycast(rayOrigin, -transform.forward - transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//9 o clock	
		} 
		if (Physics.Raycast(rayOrigin, -transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//10.5 o clock		
		} 
		if (Physics.Raycast(rayOrigin, transform.forward - transform.right, out hitInfo, maxDistance, terrainLayerMask)){
			//Debug.Log ("Hit coastline");
			if(hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine){
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if(hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent){
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if(hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance){
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
		}
		
		//now set the main bool flags to the temp flags
		rayCheck_stopShip = stopShip;
		rayCheck_stopCurrents = stopCurrents;
		rayCheck_playBirdSong = playBirdSong;	
	}
	
}
