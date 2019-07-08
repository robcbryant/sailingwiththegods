using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class script_player_controls : MonoBehaviour
{

	CharacterController controller;

	Transform shipTransform;

	ShipSpeedModifiers shipSpeedModifiers = new ShipSpeedModifiers();

	const float dailyProvisionsKG = .83f; //(NASA)
	const float dailyWaterKG = 5f; //based on nasa estimates of liters(kg) for astronauts--sailing is more physically intensive so I've upped it to 5 liters

	GameVars GameVars;

	[HideInInspector] public Ship ship;
	[HideInInspector] public PlayerJourneyLog journey;
	[HideInInspector] public Vector3 lastPlayerShipPosition;
	[HideInInspector] public Vector3 travel_lastOrigin;

	Vector3 currentDestination;

	[HideInInspector] public float numOfDaysTraveled = 0;
	[HideInInspector] public Vector3 originOfTrip;

	Vector3 currentWaterDirectionVector = Vector3.zero;
	Vector3 currentWindDirectionVector = Vector3.zero;
	Vector3 playerMovementVector = Vector3.zero;
	float currentWaterDirectionMagnitude = 0f;
	float currentWindDirectionMagnitude = 0f;

	[HideInInspector] public float current_shipSpeed_Magnitude = 0f;

	bool getSettlementDockButtonReady = false;

	[HideInInspector] public float numOfDaysWithoutProvisions = 0;
	[HideInInspector] public float numOfDaysWithoutWater = 0;

	[HideInInspector] public int dayCounterStarving = 0;
	[HideInInspector] public int dayCounterThirsty = 0;

	bool notEnoughSpeedToMove = false;

	float initialAngle = 0f;
	float initialCelestialAngle = 0f;
	float targetAngle = 0f;


	public GameObject cursorRing;

	Material cursorRingMaterial;
	bool cursorRingIsGreen = true;
	float cursorRingAnimationClock = 0f;


	public GameObject fogWall;

	bool shipTravelStartRotationFinished = false;


	[HideInInspector] public bool rayCheck_stopShip = false;
	bool rayCheck_stopCurrents = false;
	bool rayCheck_playBirdSong = false;

	public AudioSource SFX_birdsong;

	List<string> windZoneNamesToTurnOn = new List<string>();
	List<string> currentZoneNamesToTurnOn = new List<string>();

	public void Reset() {
		// NOTE KD: Changed ship base speed to 16 so that even with low crew, you can still counter the current and wind. was 7.408 researched or arbitrary?
		ship = new Ship("Argo", 42, 100, 500);//7.408f, 100, 500f);
		ship.networkID = 246;
		journey = new PlayerJourneyLog();
		lastPlayerShipPosition = transform.position;
		ship.mainQuest = CSVLoader.LoadMainQuestLine();

		//Setup the day/night cycle
		UpdateDayNightCycle(GameVars.IS_NEW_GAME);

		//initialize players ghost route
		UpdatePlayerGhostRouteLineRenderer(GameVars.IS_NEW_GAME);
	}

	// Use this for initialization
	void Start() {
		GameVars = Globals.GameVars;
		controller = gameObject.GetComponent<CharacterController>();
		shipTransform = transform.GetChild(0);

		Reset();

		//Now teleport the player ship to an appropriate location near the first target
		transform.position = new Vector3(1702.414f, transform.position.y, 2168.358f);

		//Setup the Cursor Ring Material for Animation
		cursorRingMaterial = cursorRing.GetComponent<MeshRenderer>().sharedMaterial;

		//DEBUG
		GameVars.DEBUG_currentQuestLeg = ship.mainQuest.currentQuestSegment;

		//Start the infinite loop of checking for wind and water current zones
		StartCoroutine(waterCurrentZoneMaintenance());
		StartCoroutine(WindZoneMaintenance());

		CheckCurrentWaterWindZones();
	}

	// Update is called once per frame
	void Update() {
		//DEBUG
		//DEBUG
		//MGV.DEBUG_currentQuestLeg = ship.mainQuest.currentQuestSegment;
		//if( MGV.DEBUG_currentQuestLegIncrease){
		//	ship.mainQuest.currentQuestSegment++;
		//	MGV.DEBUG_currentQuestLegIncrease = false;
		//}


		//Update the size of the crew every update
		ship.crew = ship.crewRoster.Count;

		//Make sure the camera transform is always tied to the front of the ship model's transform if the FPV camera is enabled
		if (GameVars.FPVCamera.activeSelf)
			GameVars.FPVCamera.transform.parent.parent.position = shipTransform.TransformPoint(new Vector3(shipTransform.localPosition.x, .31f, shipTransform.localPosition.z + .182f));

		//Make sure horizon sky is always at player position
		GameVars.skybox_horizonColor.transform.position = transform.position;

		//Make sure our celestial spheres are always tied to the position of the player ship
		//	--We are running a "Mariner-centric" universe and calculating the visual effects of the universe around the ship
		GameVars.skybox_MAIN_CELESTIAL_SPHERE.transform.position = transform.position;
		//Make sure the player's latitude determines the angle of the north pole
		RotateCelestialSky();
		//Update the earth's precession
		CalculatePrecessionOfEarth();

		//Always Update the current Speed before the logic below
		UpdateShipSpeed();

		//check for bird song
		if (rayCheck_playBirdSong) SFX_birdsong.enabled = true;
		else SFX_birdsong.enabled = false;


		// TODO: Make a game state system instead of all these booleans

		//TODO: need to update all references to controlsLocked to the MGV.controlsLocked
		//controlsLocked = MGV.controlsLocked;
		//If NOT Game Over then go with the regular logic
		if (!GameVars.isGameOver) {
			//if the controls are not locked--we are anchored
			if (!GameVars.controlsLocked) {
				//check to see if we just left a port starving
				if (GameVars.justLeftPort) {
					GameVars.justLeftPort = false;
					CheckIfShipLeftPortStarvingOrThirsty();
					//TODO need to add a check here for notification windows to lock controls

					//
				}

				// don't let the player use the cursor, rotate the camera, or zoom when the mouse is over a blocking UI
				if(!UISystem.IsMouseOverUI()) {
					//If we aren't locking the controls for a GUI pop up then look for player cursor
					CheckForPlayerNavigationCursor();
					AnimateCursorRing();
					//check for panning screen
					CheckCameraRotationControls();
					//check for zooming in / out
					CheckZoomControls();
				}

				//show the settlement docking button if in a docking area
				if (getSettlementDockButtonReady) {
					getSettlementDockButtonReady = false;
					GameVars.showSettlementTradeButton = true;
				}

				//Controls Are Locked--so we are traveling	or new gaming
			}
			else if (!GameVars.isGameOver) {
				//Check if we are starting a new game and are at the title screen
				if (GameVars.isTitleScreen || GameVars.isStartScreen) {
					//If the player triggers the GUI button to start the game, stop the animation and switch the camera off
					if (GameVars.startGameButton_isPressed) {
						//Debug.Log ("Quest Seg start new game: " + ship.mainQuest.currentQuestSegment);
						//Turn off title screen camera
						GameVars.camera_titleScreen.SetActive(false);
						GameVars.bg_startScreen.SetActive(false);

						//Turn on the environment fog
						RenderSettings.fog = true;

						//Now turn on the main player controls camera
						GameVars.FPVCamera.SetActive(true);

						//Turn on the player distance fog wall
						fogWall.SetActive(true);

						//Now change titleScreen to false
						GameVars.isTitleScreen = false;
						GameVars.isStartScreen = false;

						//Now enable the controls
						GameVars.controlsLocked = false;

						//Initiate the main questline
						GameVars.InitiateMainQuestLineForPlayer();

						//Reset Start Game Button
						GameVars.startGameButton_isPressed = false;

					}

					//Else if we are passing time at rest
				}
				else if (GameVars.isPassingTime) {
					//Debug.Log ("passing time....");
					CheckCameraRotationControls();
					//Else we are not at the title screen and just in the game
				}
				else {
					//Check if we're in the menus or not
					//	-If we aren't in the settlement menu then we know we're traveling
					if (!GameVars.menuControlsLock) {
						//If the ship is dead in the water--don't do anything
						if (shipSpeed_Actual != 0)
							TravelToSelectedTarget(currentDestination);
						else {
							GameVars.controlsLocked = false;
							GameVars.isGameOver = true;
							current_shipSpeed_Magnitude = 0f;
						}
						CheckCameraRotationControls();
					}
					else {
						//we are in the settlement menu GUI so just do nothing
					}
				}
			}

			//It's GAME OVER
		}
		else {


		}

	}


	public void CheckForPlayerNavigationCursor() {

		Vector3 main_mouse = GameVars.FPVCamera.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
		//Debug.Log (main_mouse);
		//Here we are first checking to see if the mouse cursor is over the actual gameplay window
		Rect FPVCamRect = GameVars.FPVCamera.GetComponent<Camera>().rect;
		//Debug.Log (FPVCamRect);
		if (FPVCamRect.Contains(main_mouse) && !UISystem.IsMouseOverUI()) {
			//If the mouse cursor is hovering over the allowed gameplay window, then figure out the position of the mouse in worldspace
			Ray ray = GameVars.FPVCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			//Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
			//Debug.Log ("I guess?");

			System.Func<RaycastHit, bool> isRelevant = (RaycastHit h) =>
				h.collider.tag == "playerShip" ||
				h.collider.tag == "waterSurface" ||
				h.collider.tag == "terrain" ||
				h.collider.tag == "settlementClick";

			var hits = Physics.RaycastAll(ray.origin, ray.direction, 100f)
				.Where(isRelevant)
				.OrderBy(h => h.distance);

			if (hits.Any()) {
				var firstRelevantHit = hits.First();

				//if we get a hit, then turn the cursor ring on
				cursorRing.SetActive(true);
				//Move the animated cursor ring to the position of the mouse cursor
				cursorRing.transform.position = firstRelevantHit.point;// + new Vector3(0,.03f,0);

				//Adjust the scale of the cursor ring to grow with distance
				float newScale = Vector3.Distance(cursorRing.transform.position, GameVars.FPVCamera.transform.position) * .09f;
				cursorRing.transform.localScale = new Vector3(newScale, newScale, newScale);

				//Hide the cursor if it's touching the player ship to avoid weird visual glitches
				if (hits.Any(h => h.collider.tag == "playerShip")) cursorRing.SetActive(false);
				else cursorRing.SetActive(true);

				//Now let's check for line of site to the target
				bool hasLineOfSight = true;
				RaycastHit lineOfSightHit;
				Vector3 lineOfSightOrigin = shipTransform.position + new Vector3(0, .02f, 0);
				//We Fire a raycast in the direction of the selected point from the ship
				if (Physics.Raycast(lineOfSightOrigin, Vector3.Normalize(firstRelevantHit.point - lineOfSightOrigin), out lineOfSightHit, 100f)) {
					//If the ray intersects with anything but water then there is no line of sight
					//Debug.DrawRay(lineOfSightOrigin,Vector3.Normalize(hitInfo.point - lineOfSightOrigin) * 100, Color.yellow);
					//Debug.Log (lineOfSightHit.transform.tag);
					if (lineOfSightHit.transform.tag != "waterSurface")
						hasLineOfSight = false;
					else
						hasLineOfSight = true;
				}

				//If the cursor is on water and has line of sight
				if (firstRelevantHit.collider.tag == "waterSurface" && hasLineOfSight == true) {
					//Make sure the cursor ring is Green
					cursorRingIsGreen = true;

					CalculateShipTrajectoryPreview(firstRelevantHit.point);

					//Now check to see if the player clicks the left mouse button to travel
					if (Input.GetButton("Select")) {
						//lock controls so that the travel function is triggered on the next update cycle
						GameVars.controlsLocked = true;
						//set the destination: using the players Y value so the ship always stays at a set elevation
						currentDestination = new Vector3(firstRelevantHit.point.x, transform.position.y, firstRelevantHit.point.z);
						//set the player ship's current position to be logged into the journey log
						lastPlayerShipPosition = transform.position;
						travel_lastOrigin = transform.position;
						numOfDaysTraveled = 0;
					}

				}
				else if (hits.Any(h => h.collider.tag == "settlementClick") && 
						hits.First(h => h.collider.tag == "settlementClick").collider.GetComponentInParent<script_settlement_functions>().thisSettlement == GameVars.currentSettlement && 
						GameVars.currentSettlement != null) {

					// TODO: should probably change it to some other color or something?
					cursorRingIsGreen = true;
					//We also need to make sure the trajectory preview is turned off
					CalculateShipTrajectoryPreview(transform.position);

					//Now check to see if the player clicks the left mouse button to open the port menu
					//Clicking here will pop the city dialog as long as you're in the docking zone. 
					if (Input.GetButton("Select")) {
						GameObject.FindObjectOfType<script_GUI>().GUI_checkOutOrDockWithPort(true);		// TODO: Move this into Globals for now until I've pulled everything out.
					}

				}
				else {
					//Since we aren't allowed to travel to the mouse position, change the cursor to red
					cursorRingIsGreen = false;
					//We also need to make sure the trajectory preview is turned off
					CalculateShipTrajectoryPreview(transform.position);
				}

				//If the point on the screen correlates to nothing--say the sky, then turn off the ship projectory preview		
			}
			else {
				cursorRing.SetActive(false);
				CalculateShipTrajectoryPreview(transform.position);

			}

		}
	}

	public void CheckCameraRotationControls() {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		// don't allow manual rotation when auto-rotating
		if (GameVars.CameraLookTarget.HasValue) return;

		if (horizontal < 0) {
			//Rotate right
			GameVars.FPVCamera.transform.parent.parent.RotateAround(GameVars.FPVCamera.transform.parent.parent.position, GameVars.FPVCamera.transform.parent.parent.up, -70 * Time.deltaTime);
		}
		else if (horizontal > 0) {
			//Rotate Left
			GameVars.FPVCamera.transform.parent.parent.RotateAround(GameVars.FPVCamera.transform.parent.parent.position, GameVars.FPVCamera.transform.parent.parent.up, 70 * Time.deltaTime);
		}
		//Debug.Log (MGV.FPVCamera.transform.eulerAngles.x);
		//This first if statement sets the boundary range for the camera vertical rotation--the Unity engine makes this a bit wonky because it's 270-90 for a full 180 degree rotation
		if ((GameVars.FPVCamera.transform.eulerAngles.x <= 40f && GameVars.FPVCamera.transform.eulerAngles.x >= 0f) || (GameVars.FPVCamera.transform.eulerAngles.x <= 360f && GameVars.FPVCamera.transform.eulerAngles.x >= 295f)) {
			if (vertical < 0) {
				//Rotate down					
				GameVars.FPVCamera.transform.RotateAround(GameVars.FPVCamera.transform.position, GameVars.FPVCamera.transform.right, 1.5f);


			}
			else if (vertical > 0) {
				//Rotate up
				GameVars.FPVCamera.transform.RotateAround(GameVars.FPVCamera.transform.position, GameVars.FPVCamera.transform.right, -1.5f);
				//Now we need to make sure we don't over rotate past our mark
			}
		}
		//Now we need to make sure we don't over rotate past our mark
		if (GameVars.FPVCamera.transform.eulerAngles.x <= 50f && GameVars.FPVCamera.transform.eulerAngles.x >= 39f) {
			GameVars.FPVCamera.transform.eulerAngles = new Vector3(39.9f, GameVars.FPVCamera.transform.eulerAngles.y, GameVars.FPVCamera.transform.eulerAngles.z);
			//Debug.Log ("We're chancing it to 40?");
		}
		if (GameVars.FPVCamera.transform.eulerAngles.x <= 296f && GameVars.FPVCamera.transform.eulerAngles.x >= 285f)
			GameVars.FPVCamera.transform.eulerAngles = new Vector3(295.9f, GameVars.FPVCamera.transform.eulerAngles.y, GameVars.FPVCamera.transform.eulerAngles.z);
		if (GameVars.FPVCamera.transform.eulerAngles.x < 0)
			GameVars.FPVCamera.transform.eulerAngles = new Vector3(359f, GameVars.FPVCamera.transform.eulerAngles.y, GameVars.FPVCamera.transform.eulerAngles.z);
	}

	public void CheckZoomControls() {
		float zoom = Input.GetAxis("Mouse ScrollWheel");
		//if zooming out
		if (zoom < 0 && GameVars.FPVCamera.transform.parent.localScale.x < 3.2f) {
			GameVars.FPVCamera.transform.parent.localScale = new Vector3(GameVars.FPVCamera.transform.parent.localScale.x + .1f, GameVars.FPVCamera.transform.parent.localScale.y + .001f, GameVars.FPVCamera.transform.parent.localScale.z + 0.1f);
			GameVars.FPVCamera.transform.parent.Translate(new Vector3(0, .023f, 0));
			//MGV.FPVCamera.transform.position = Vector3.Lerp(MGV.FPVCamera.transform.position, new Vector3(MGV.FPVCamera.transform.parent.position.x,MGV.FPVCamera.transform.parent.position.y+.1f,MGV.FPVCamera.transform.parent.position.z+4f), .1f);//new Vector3(MGV.FPVCamera.transform.localPosition.x + .01f,MGV.FPVCamera.transform.localPosition.y + .01f,MGV.FPVCamera.transform.localPosition.z + .01f);

		}
		else if (zoom > 0 && GameVars.FPVCamera.transform.parent.localScale.x > .0698f) {
			GameVars.FPVCamera.transform.parent.localScale = new Vector3(GameVars.FPVCamera.transform.parent.localScale.x - .1f, GameVars.FPVCamera.transform.parent.localScale.y - .001f, GameVars.FPVCamera.transform.parent.localScale.z - 0.1f);
			GameVars.FPVCamera.transform.parent.Translate(new Vector3(0, -.023f, 0));
			//MGV.FPVCamera.transform.position = Vector3.Lerp(MGV.FPVCamera.transform.position, MGV.FPVCamera.transform.parent.position, .1f);//MGV.FPVCamera.transform.localPosition = new Vector3(MGV.FPVCamera.transform.localPosition.x - .01f,MGV.FPVCamera.transform.localPosition.y - .01f,MGV.FPVCamera.transform.localPosition.z - .01f);

		}

		//If the zoom over shoots its target, then reset it to the minimum
		if (GameVars.FPVCamera.transform.parent.localScale.x < .0698f) {
			GameVars.FPVCamera.transform.parent.localScale = new Vector3(.0698f, GameVars.FPVCamera.transform.parent.localScale.y, .0698f);
			GameVars.FPVCamera.transform.parent.localPosition = new Vector3(0, -230f, 0);
		}


	}



	public void TravelToSelectedTarget(Vector3 destination) {
		//Let's slowly rotate the ship towards the direction it's traveling and then allow the ship to move
		if (!shipTravelStartRotationFinished) {


			destination = new Vector3(destination.x, shipTransform.position.y, destination.z);
			Vector3 temprot = Vector3.RotateTowards(shipTransform.forward, Vector3.Normalize(destination - shipTransform.position), .1f, 0.0F);
			Vector3 targetDirection = Vector3.Normalize(destination - shipTransform.position);
			//Debug.Log (targetDirection + "         ==?   " + shipTransform.forward );
			//Debug.Log (destination);
			//Debug.Log ("TRAVELING");
			//We use Mathf Approximately to compare float values and end the rotation sequence when the ships direction matches the target's direction
			if (Utils.FastApproximately(targetDirection.x, shipTransform.forward.x, .01f) &&
				Utils.FastApproximately(targetDirection.y, shipTransform.forward.y, .01f) &&
				Utils.FastApproximately(targetDirection.z, shipTransform.forward.z, .01f)) {
				shipTravelStartRotationFinished = true;
			}
			//Lerp the rotation of the ship towards the destination
			shipTransform.rotation = Quaternion.LookRotation(temprot);
			//temprot = new Vector3(temprot.x, shipTransform.position.y, temprot.z);
			//shipTransform.LookAt(temprot);
			//Once we're finished, allow the ship to travel
		}
		else {
			//Turn off mouse selection cursor
			cursorRing.SetActive(false);

			//Draw red line from ship to destination
			//		LineRenderer line = gameObject.GetComponent<LineRenderer>();
			//		line.SetPosition(0,new Vector3(transform.position.x,transform.position.y-.23f,transform.position.z));
			//		line.SetPosition(1,new Vector3(destination.x,destination.y-.23f,destination.z));
			//		line.SetColors(Color.green, Color.blue);

			//rotate player ship to settlement
			shipTransform.LookAt(destination);
			shipTransform.eulerAngles = new Vector3(0, shipTransform.eulerAngles.y, 0);
			//Get direction to settlement from ship
			Vector3 travelDirection = Vector3.Normalize(destination - transform.position);
			float distance = Vector3.Distance(destination, transform.position);



			//figure out the actual speed of the ship if currents/wind are present and if the sails are unfurled or not
			Vector3 windAndWaterVector = GetCurrentWindWaterForceVector(travelDirection);


			//Debug.Log (travel_lastOrigin + "  --<<TRAVEL LAST ORIGIN");
			float disTraveled = Vector3.Distance(travel_lastOrigin, transform.position);
			travel_lastOrigin = transform.position;

			//Debug.Log (current_shipSpeed_Magnitude + "   <current ship speed mag");
			float numOfDaysTraveledInSegment = ((disTraveled * (CoordinateUtil.unityWorldUnitResolution / 1000f))
																/
													current_shipSpeed_Magnitude) / (24f);

			numOfDaysTraveled += numOfDaysTraveledInSegment;
			//Debug.Log (numOfDaysTraveled +  "    ---< num of days traveled");
			//Debug.Log (disTraveled);
			ship.totalNumOfDaysTraveled += numOfDaysTraveledInSegment;

			//Perform regular updates as the ship travels
			UpdateShipAtrophyAfterTravelTime(numOfDaysTraveledInSegment, false);
			CheckIfProvisionsOrWaterIsDepleted(numOfDaysTraveledInSegment);
			RandomEvents.WillARandomEventHappen(GameVars, ship, shipSpeedModifiers, transform);
			UpdateDayNightCycle(GameVars.IS_NOT_NEW_GAME);

			// update beacons
			UpdateNavigatorBeaconAppearenceBasedOnDistance(GameVars.navigatorBeacon);
			UpdateNavigatorBeaconAppearenceBasedOnDistance(GameVars.crewBeacon);

			//if the ship hasn't gotten to the direction, then keep moving
			if (distance > .2f && !notEnoughSpeedToMove && !rayCheck_stopShip) {
				//We need to make sure the ship has the speed to go against the wind and currents--if it doesn't then we need to stop traveling
				//	--we will check the angle between the final movement vector and the destination
				//	--if it's more than 160 degrees, then stop the ship (more than 160 can be buggy)
				playerMovementVector = ((travelDirection * shipSpeed_Actual) + windAndWaterVector) * shipSpeedModifiers.Game;
				//Debug.Log (Vector3.Angle(travelDirection, playerMovementVector));
				if (Vector3.Angle(travelDirection, playerMovementVector) < 160f) {
					controller.Move(playerMovementVector * Time.deltaTime);
				}
				else
					notEnoughSpeedToMove = true;

				//Draw red line from ship to destination
				//line.SetPosition(0,new Vector3(transform.position.x,transform.position.y-.23f,transform.position.z));

				//Fire off the coast line raycasts to detect for the coast
				DetectCoastLinesWithRayCasts();

			}
			else if (!GameVars.showSettlementGUI || notEnoughSpeedToMove) { //check to see if we're in the trade menu otherwise we will indefintely write duplicate routes until we leave the trade menu
																			//save this route to the PlayerJourneyLog
				journey.AddRoute(new PlayerRoute(lastPlayerShipPosition, transform.position, ship.totalNumOfDaysTraveled), gameObject.GetComponent<script_player_controls>(), GameVars.currentCaptainsLog);
				//Update player ghost route
				UpdatePlayerGhostRouteLineRenderer(GameVars.IS_NOT_NEW_GAME);
				//Reset the travel line to a distance of zero (turn it off)
				//line.SetPosition(0,Vector3.zero);
				//line.SetPosition(1,Vector3.zero);

				current_shipSpeed_Magnitude = 0f;

				//reset the not enough speed flag
				notEnoughSpeedToMove = false;
				GameVars.controlsLocked = false;
				shipTravelStartRotationFinished = false;

				//reset coastline detection flag
				rayCheck_stopShip = false;
			}//End of Travel
		}//End of initial ship rotation
	}


	public void UpdateShipAtrophyAfterTravelTime(float travelTime, bool isPassingTime) {
		float dailyProvisionsKG = .83f;
		float dailyWaterKG = 5f;
		//If we're passing time--then our consumption should be lower due to less physical exertion
		//--I'm simply cutting this in half for now--TODO: It would be nice to take the time of day into account--
		//--e.g. the hotter hours should require more expenditure and therefore more resources. 
		//--Additionally--I would liek to turn off rowing as well as sails--so if one is just sailing without oaring--less resources are used.
		if (isPassingTime) { dailyProvisionsKG /= 2; dailyWaterKG /= 2; }
		//deplete Provisions based on number of crew  (NASA - .83kg per astronaut / day
		if (ship.cargo[1].amount_kg <= 0) ship.cargo[1].amount_kg = 0;
		else ship.cargo[1].amount_kg -= (travelTime * dailyProvisionsKG) * ship.crew;

		//deplete water based on number of crew (NASA 3kg minimum a day --we'll use 5 because conditions are harder--possibly 10 from rowing)
		if (ship.cargo[0].amount_kg <= 0) ship.cargo[0].amount_kg = 0;
		else ship.cargo[0].amount_kg -= (travelTime * dailyWaterKG) * ship.crew;

		//deplete ship hp (we'll say 1HP per day)
		if (ship.health <= 0) ship.health = 0;
		else ship.health -= travelTime;

		//Debug.Log (travelTime + "days in segment   --- " +ship.cargo[0].amount_kg + "kg    " + ship.cargo[1].amount_kg + "kg    " + ship.health + "hp   lost to travel needs");

	}


	void OnTriggerEnter(Collider trigger) {
		Debug.Log("On trigger enter triggering" + trigger.transform.tag);
		if (trigger.transform.tag == "currentDirectionVector") {
			currentWaterDirectionVector = trigger.transform.GetChild(0).GetChild(0).up.normalized;
			currentWaterDirectionMagnitude = trigger.transform.GetChild(0).GetComponent<script_WaterWindCurrentVector>().currentMagnitude;
			KeepNearestWaterZonesOn(trigger.transform.name);
		}
		if (trigger.transform.tag == "windDirectionVector") {
			currentWindDirectionVector = trigger.transform.GetChild(0).transform.forward.normalized;
			currentWindDirectionMagnitude = trigger.transform.GetChild(0).GetComponent<script_WaterWindCurrentVector>().currentMagnitude;
			KeepNearestWindZonesOn(trigger.transform.name);
		}
		if (trigger.transform.tag == "settlement_dock_area") {
			//Here we first figure out what kind of 'settlement' we arrive at, e.g. is it just a point of interest or is it a actual dockable settlement
			//if it's a dockable settlement, then allow the docking menu to be accessed, otherwise run quest functions etc.
			Debug.Log(trigger.transform.parent.GetComponent<script_settlement_functions>().thisSettlement);
			if (trigger.transform.parent.GetComponent<script_settlement_functions>().thisSettlement.typeOfSettlement == 1) {
				getSettlementDockButtonReady = true;
				GameVars.currentSettlement = trigger.transform.parent.gameObject.GetComponent<script_settlement_functions>().thisSettlement;
				GameVars.currentSettlementGameObject = trigger.transform.parent.gameObject;
				GameVars.playerShipVariables.ship.playerJournal.AddNewSettlementToLog(GameVars.currentSettlement.settlementID);
				//If it is a point of interest then run quest functions but don't allow settlement resource access
			}
			else if (trigger.transform.parent.GetComponent<script_settlement_functions>().thisSettlement.typeOfSettlement == 0) {
				//change the current settlement to this location (normally this is done by opening the docking menu--but in this case there is no docking menu)
				GameVars.currentSettlement = trigger.transform.parent.GetComponent<script_settlement_functions>().thisSettlement;
				//Check if current Settlement is part of the main quest line
				GameVars.CheckIfCurrentSettlementIsPartOfMainQuest(GameVars.currentSettlement.settlementID);
				GameVars.showNonPortDockButton = true;
			}
		}
		if (trigger.transform.tag == "settlement") {
			Debug.Log("Entering Area of: " + trigger.GetComponent<script_settlement_functions>().thisSettlement.name + ". And the current status of the ghost route is: " + GameVars.playerGhostRoute.activeSelf);
			//This zone is the larger zone of influence that triggers city specific messages to pop up in the captains log journal
			GameVars.AddEntriesToCurrentLogPool(trigger.GetComponent<script_settlement_functions>().thisSettlement.settlementID);
			//We add the triggered settlement ID to the list of settlements to look for narrative bits from. In the OnTriggerExit() function, we remove them
			GameVars.activeSettlementInfluenceSphereList.Add(trigger.GetComponent<script_settlement_functions>().thisSettlement.settlementID);
			//If the player got lost asea and the memory map ghost route is turned off--check to see if we're enteringg friendly waters
			if (GameVars.playerGhostRoute.activeSelf == false) {
				CheckIfPlayerFoundKnownSettlementAndTurnGhostTrailBackOn(trigger.GetComponent<script_settlement_functions>().thisSettlement.settlementID);
			}
		}
	}

	void OnTriggerExit(Collider trigger) {
		if (trigger.transform.tag == "waterDirectionVector") {

		}
		if (trigger.transform.tag == "windDirectionVector") { //right now--there should always be trigger box--it should just update, and never need to reset upon exit

		}
		if (trigger.transform.tag == "settlement_dock_area") {
			GameVars.showSettlementTradeButton = false;
		}
		if (trigger.transform.tag == "settlement") {
			//This zone is the larger zone of influence that triggers city specific messages to pop up in the captains log journal
			GameVars.RemoveEntriesFromCurrentLogPool(trigger.GetComponent<script_settlement_functions>().thisSettlement.settlementID);
			//We add the triggered settlement ID to the list of settlements to look for narrative bits from. In the OnTriggerExit() function, we remove them
			GameVars.activeSettlementInfluenceSphereList.Remove(trigger.GetComponent<script_settlement_functions>().thisSettlement.settlementID);
		}
	}

	void CheckIfProvisionsOrWaterIsDepleted(float travelTimeToAddIfDepleted) {
		if (ship.cargo[0].amount_kg <= 0) {
			numOfDaysWithoutWater += travelTimeToAddIfDepleted;
			CheckToSeeIfCrewWillDieFromThirst();
		}
		else {
			numOfDaysWithoutWater = 0;
			dayCounterThirsty = 0;
		}

		if (ship.cargo[1].amount_kg <= 0) {
			numOfDaysWithoutProvisions += travelTimeToAddIfDepleted;
			CheckToSeeIfCrewWillDieFromStarvation();
		}
		else {
			numOfDaysWithoutProvisions = 0;
			dayCounterStarving = 0;
		}
	}



	void CheckToSeeIfCrewWillDieFromStarvation() {
		//This uses a counter system to determine the number of days in order to make sure the death roll is only rolled
		//	--ONCE per day, rather than every time the function is called.
		//Every day without Provisions--the chance of a crew member dying from starvation increases
		//	--The first day starts at 30%, and every day onward increases it by 10%
		int numOfDays = Mathf.FloorToInt(numOfDaysWithoutProvisions);
		//first check to see if we're at atleast one day
		if (numOfDays >= 1) {
			dayCounterStarving++;
			numOfDaysWithoutProvisions = 0;
			int deathRate = 50 + (dayCounterStarving * 10);

			//If a crewmember dies due to the percentage roll
			if (Random.Range(0, 100) <= deathRate)
				//Kill a crewmember
				KillCrewMember();
		}

	}

	void CheckToSeeIfCrewWillDieFromThirst() {
		//This uses a counter system to determine the number of days in order to make sure the death roll is only rolled
		//	--ONCE per day, rather than every time the function is called.
		//Every day without Water--the chance of a crew member dying from starvation increases
		//	--The first day starts at 80%, and every day onward increases it by 10%
		int numOfDays = Mathf.FloorToInt(numOfDaysWithoutWater);
		//first check to see if we're at atleast one day
		if (numOfDays >= 1) {
			dayCounterThirsty++;
			numOfDaysWithoutWater = 0;
			int deathRate = 50 + (dayCounterThirsty * 10);

			//If a crewmember dies due to the percentage roll
			if (Random.Range(0, 100) <= deathRate)
				//Kill a crewmember
				KillCrewMember();
		}
	}

	void KillCrewMember() {
		//If there are crew members to kill off
		if (ship.crewRoster.Count > 0) {
			//choose a random crew member to kill
			ship.crewRoster.RemoveAt(Random.Range(0, ship.crewRoster.Count));
			//update any clout the player may have lost from a crew member's death
			if (ship.playerClout <= 0) ship.playerClout = 0;
			else ship.playerClout -= 5;
		}
	}

	void UpdateShipSpeed() {
		//Figure out the crew modifier for speed -- we need to make the crew a ratio to subtract 
		//	--crew modifier is ((totalPossibleCrew / 100) * current#OfCrew ) / 10 --this gives a percentage of crew available
		//	--we use a curve: (x-.25)^2*3 to figure out the reduced speed impact so the speed impact isn't a direct 1:1 effect on speed.
		// 	--and then subtract that amount from the total speed to get our modifier
		float finalcreweffect = ((((float)ship.crewCapacity / 100f) * (float)ship.crewRoster.Count) / 10f);
		finalcreweffect -= .25f;
		finalcreweffect *= finalcreweffect;
		finalcreweffect *= 3f;
		if (finalcreweffect > 1f) finalcreweffect = 1f;
		shipSpeedModifiers.Crew = ship.speed - (ship.speed * finalcreweffect);
		//Debug.Log (ship.speed + "  *  (" + ship.crewCapacity/100f + " * " + ship.crewRoster.Count);

		//Figure out the Hunger and Thirst modifier for speed
		//	--while hungry, ship speed is reduced by 25 % from the original top speed
		if (dayCounterStarving >= 1) shipSpeedModifiers.Hunger = ship.speed * .15f;
		else shipSpeedModifiers.Hunger = 0f;
		//	--while thirsty, ship speed is reduced by 25 % from the original top speed
		if (dayCounterThirsty >= 1) shipSpeedModifiers.Thirst = ship.speed * .15f;
		else shipSpeedModifiers.Thirst = 0f;

		//Figure out the Ship Health Modifier for speed
		//	--every point of hp missing reduces the speed by that many percentage points
		//	--this only takes effect after 10 hp are lost, e.g. 91-100hp the ship's speed will still be unchanged from damage
		//	--once the ship hits 90hp, the speed will immediately be reduced by 10% and 1% for every point of damage sustained after
		if (ship.health <= 90f) shipSpeedModifiers.ShipHP = (ship.speed / 100f) * (100f - ship.health);
		else shipSpeedModifiers.ShipHP = 0f;
	}

	/// <summary>
	/// the current actual speed of the ship
	/// </summary>
	float shipSpeed_Actual {
		get {
			var result = ship.speed - shipSpeedModifiers.Crew - shipSpeedModifiers.Hunger - shipSpeedModifiers.Thirst - shipSpeedModifiers.ShipHP + shipSpeedModifiers.Event;

			//Always makes sure the ship does not have a negative speed--keep it at zero if it goes negative
			if (result < 0) result = 0;

			return result;
		}
	}


	void CheckIfShipLeftPortStarvingOrThirsty() {
		//We need to check to see if there is enough Provisions for a single days journey for all the crew
		//if there isn't--some of the crew will leave the ship

		string notificationMessage = "";
		bool ProvisionsDeaths = false;

		if (ship.cargo[1].amount_kg < dailyProvisionsKG * ship.crewRoster.Count) {
			//start a counter of how many crew members die
			int crewDeathCount = 0;
			//make sure we roll for each crew member
			for (int i = 0; i < ship.crewRoster.Count; i++) {
				//--we'll base this chance off of a static 30% plus any effect from clout(a maximum change of 25 % in either direction
				float rollChance = 30 - (((GameVars.playerShipVariables.ship.playerClout - 50f) / 100) / 2);
				//some crew needs to leave
				if (Random.Range(0, 100) <= rollChance) {
					//Kill a crewmember
					KillCrewMember();
					crewDeathCount++;
					GameVars.showNotification = true;
					ProvisionsDeaths = true;
				}
			}
			notificationMessage += crewDeathCount + " crewmember(s) quit because you left without a full store of Provisions";
		}
		if (ProvisionsDeaths) notificationMessage += ", and ";
		//We need to check to see if there is enough water for a single days journey for all the crew
		//if there isn't--some of the crew will leave the ship
		if (ship.cargo[0].amount_kg < dailyWaterKG * ship.crewRoster.Count) {
			//start a counter of how many crew members die
			int crewDeathCount = 0;
			//make sure we roll for each crew member
			for (int i = 0; i < ship.crewRoster.Count; i++) {
				//--we'll base this chance off of a static 30% plus any effect from clout(a maximum change of 25 % in either direction
				float rollChance = 30 - (((GameVars.playerShipVariables.ship.playerClout - 50f) / 100) / 2);
				//some crew needs to leave
				if (Random.Range(0, 100) <= rollChance) {
					//Kill a crewmember
					KillCrewMember();
					crewDeathCount++;
					GameVars.showNotification = true;
				}
			}
			notificationMessage += crewDeathCount + " crewmember(s) quit because you left without a full store of water.";
		}
		//now update the notification string with the message
		GameVars.notificationMessage = notificationMessage;
	}

	public void UpdateDayNightCycle(bool restartCycle) {
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
		float timeModifier = ship.totalNumOfDaysTraveled;

		Color colorDay = new Color(255f / 255f, 235f / 255f, 169f / 255f);
		Color colorNight = new Color(0f / 255f, 192f / 255f, 255f / 255f);

		Color waterColorDay = new Color(48f / 255f, 141f / 255f, 255f / 255f, .97f);
		Color waterColorNight = new Color(0f / 255f, 21f / 255f, 8f / 255f, .97f);

		Color currentColorNight = new Color(0 / 255f, 37 / 255f, 12 / 255f, 1f);
		//Debug.Log (ship.totalNumOfDaysTraveled + "    NUM OF DAYS TRAVELED!!!!");
		float timeOfDay = (timeModifier - Mathf.Floor(timeModifier)); // this removes the numbers left of the decimal so we can work with the fraction
		float testAngle = 0f;

		//TODO add rotational light possible to reflect the sun rising / setting
		//rotatingLight.transform.localEulerAngles = new Vector3((timeOfDay*360)-90,0,0);

		//First set things to default if it's a new game we've started
		//	--This ensures any editor changes are reset. Since it sets to '0' the blends will not activate 
		if (restartCycle) {
			GameVars.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Blend", 1f);
			//RenderSettings.skybox.SetFloat("_Blend", 0);
			RenderSettings.ambientIntensity = .53f;
			GameVars.mainLightSource.intensity = .78f;
			GameVars.mainLightSource.color = colorDay;
			GameVars.mat_water.SetColor("_Color", waterColorDay);
			GameVars.mat_waterCurrents.color = Color.white;
			testAngle = GameVars.skybox_MAIN_CELESTIAL_SPHERE.transform.localRotation.y;
			Debug.Log(initialAngle + "*********************");
			GameVars.skybox_horizonColor.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, 1f, 1f, 1f);
			GameVars.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, 1f, 1f);
			GameVars.skybox_sun.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, 1f, 1f);
			GameVars.skybox_moon.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, 1f, 1f, .5f);
			RenderSettings.fogColor = new Color(203f / 255f, 239f / 255f, 254f / 255f);
			fogWall.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(203f / 255f, 239f / 255f, 254f / 255f);

		}

		Color deepPurple = new Color(82f / 255f, 39f / 255f, 101f / 255f);
		Color brightSky = new Color(203f / 255f, 239f / 255f, 254f / 255f);
		//Blending Day to Night
		if (timeOfDay >= .25f && timeOfDay <= 0.5f) {
			GameVars.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Blend", Utils.GetRange(timeOfDay, .5f, .25f, 0, 1f));
			RenderSettings.ambientIntensity = Utils.GetRange(timeOfDay, .25f, .5f, .53f, .16f);
			GameVars.mainLightSource.intensity = Utils.GetRange(timeOfDay, .25f, .5f, .78f, .16f);
			GameVars.mainLightSource.color = Color.Lerp(colorDay, colorNight, Utils.GetRange(timeOfDay, .5f, .25f, 1, 0));
			//Fade Out Water Colors
			GameVars.mat_water.color = Color.Lerp(waterColorDay, waterColorNight, Utils.GetRange(timeOfDay, .5f, .25f, 1f, 0));
			//Fade Out Water Current Sprite Colors to Black
			GameVars.mat_waterCurrents.color = Color.Lerp(Color.white, currentColorNight, Utils.GetRange(timeOfDay, .5f, .25f, 1f, 0));
			//Fade Out Sky/Atmosphere Color
			GameVars.skybox_horizonColor.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, Utils.GetRange(timeOfDay, .5f, .25f, 0, 1f), 1f, Utils.GetRange(timeOfDay, .5f, .25f, 0, 1f));
			//Fade Out Sun Color
			GameVars.skybox_sun.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, Utils.GetRange(timeOfDay, .5f, .25f, 70f / 255f, 1f), Utils.GetRange(timeOfDay, .5f, .25f, 0, 1f));
			//Fade Out Clouds
			GameVars.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(Utils.GetRange(timeOfDay, .5f, .25f, 30f / 255f, 1f), Utils.GetRange(timeOfDay, .5f, .25f, 30f / 255f, 1f), Utils.GetRange(timeOfDay, .5f, .25f, 50f / 255f, 1f));
			//Fade In Moon(transparency to opaque)
			GameVars.skybox_moon.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, 1f, 1f, Utils.GetRange(timeOfDay, .5f, .25f, 1f, 28f / 255f));
			//Fade in Dark Fog: This breaks up the fog colro fade into two shades to better match the sunset
			if (timeOfDay >= .25f && timeOfDay <= 0.35f) {
				RenderSettings.fogColor = Color.Lerp(brightSky, deepPurple, Utils.GetRange(timeOfDay, .35f, .25f, 1f, 0));
				fogWall.GetComponent<MeshRenderer>().sharedMaterial.color = Color.Lerp(brightSky, deepPurple, Utils.GetRange(timeOfDay, .35f, .25f, 1f, 0));
			}
			else {
				//Also we';; turn on the city lights here right as sunset
				GameVars.cityLightsParent.SetActive(true);
				RenderSettings.fogColor = Color.Lerp(deepPurple, waterColorNight, Utils.GetRange(timeOfDay, .5f, .35f, 1f, 0));
				fogWall.GetComponent<MeshRenderer>().sharedMaterial.color = Color.Lerp(deepPurple, waterColorNight, Utils.GetRange(timeOfDay, .5f, .35f, 1f, 0));
			}

		}
		//Blending Night to Day
		if (timeOfDay > 0.75f && timeOfDay <= 1f) {
			GameVars.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Blend", Utils.GetRange(timeOfDay, 1f, .75f, 1f, 0));
			RenderSettings.ambientIntensity = Utils.GetRange(timeOfDay, .75f, 1f, .16f, .53f);
			GameVars.mainLightSource.intensity = Utils.GetRange(timeOfDay, .75f, 1f, .16f, .78f);
			GameVars.mainLightSource.color = Color.Lerp(colorNight, colorDay, Utils.GetRange(timeOfDay, 1f, .75f, 1, 0));
			//Fade In Water Colors
			GameVars.mat_water.color = Color.Lerp(waterColorNight, waterColorDay, Utils.GetRange(timeOfDay, 1f, .75f, 1f, 0));
			//Fade Out Water Current Sprite Colors to Black
			GameVars.mat_waterCurrents.color = Color.Lerp(currentColorNight, Color.white, Utils.GetRange(timeOfDay, 1f, .75f, 1f, 0));
			//Fade In Sky/Atmosphere Color
			GameVars.skybox_horizonColor.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, Utils.GetRange(timeOfDay, 1f, .75f, 1f, 0), 1f, Utils.GetRange(timeOfDay, 1f, .75f, 1f, 0));
			//Fade In Sun Color
			GameVars.skybox_sun.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, Utils.GetRange(timeOfDay, 1f, .75f, 1f, 70f / 255f), Utils.GetRange(timeOfDay, 1f, .75f, 1f, 0));
			//Fade In Clouds
			GameVars.skybox_clouds.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(Utils.GetRange(timeOfDay, 1f, .75f, 1f, 30f / 255f), Utils.GetRange(timeOfDay, 1f, .75f, 1f, 30f / 255f), Utils.GetRange(timeOfDay, 1f, .75f, 1f, 50f / 255f));
			//Fade out Moon(opaque to transparency)
			GameVars.skybox_moon.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1f, 1f, 1f, Utils.GetRange(timeOfDay, 1f, .75f, 28f / 255f, 1f));
			//Fade in Normal Fog: This breaks up the fog colro fade into two shades to better match the sunrise
			if (timeOfDay >= .75f && timeOfDay <= 0.85f) {
				RenderSettings.fogColor = Color.Lerp(waterColorNight, deepPurple, Utils.GetRange(timeOfDay, .85f, .75f, 1f, 0));
				fogWall.GetComponent<MeshRenderer>().sharedMaterial.color = Color.Lerp(waterColorNight, deepPurple, Utils.GetRange(timeOfDay, .85f, .75f, 1f, 0));
			}
			else {
				//Also we';; turn off the city lights here right as sun rises
				GameVars.cityLightsParent.SetActive(false);
				RenderSettings.fogColor = Color.Lerp(deepPurple, brightSky, Utils.GetRange(timeOfDay, 1f, .85f, 1f, 0));
				fogWall.GetComponent<MeshRenderer>().sharedMaterial.color = Color.Lerp(deepPurple, brightSky, Utils.GetRange(timeOfDay, 1f, .85f, 1f, 0));
			}
		}
		//------------------------ Rotate the sky for day night cycle
		targetAngle = Utils.GetRange(timeOfDay, 1f, 0, 360 + testAngle, testAngle);
		GameVars.skybox_MAIN_CELESTIAL_SPHERE.transform.Rotate(0, targetAngle - initialAngle, 0, Space.Self);
		//		//Debug.Log (initialAngle +  "***********" + targetAngle);
		RotateClouds(targetAngle - initialAngle);
		initialAngle = targetAngle;



	}

	public void RotateCelestialSky() {
		//We need to get the players latitude to determine the vertical angle of the celestial globe
		//	--This is the angle of the north celestial pole from the horizon line
		//	--This is our x Angle of the sphere--0 degrees in Unity is the same as 90 degrees of latitude
		Vector2 playerLatLong = CoordinateUtil.ConvertWebMercatorToWGS1984(CoordinateUtil.Convert_UnityWorld_WebMercator(transform.position));
		Transform celestialSphere = GameVars.skybox_MAIN_CELESTIAL_SPHERE.transform;
		float latitude = playerLatLong.y;

		float targetAngle = Utils.GetRange(latitude, 90f, -90f, 0, -180);//(90 - latitude);
		float angleChange = initialCelestialAngle - targetAngle;
		initialCelestialAngle = targetAngle;

		celestialSphere.transform.Rotate(angleChange, 0, 0, Space.World);
		//Debug.Log (angleChange);
	}

	public void CalculatePrecessionOfEarth() {


		//Calculate the position of the precession around the ecliptic orbit

		// Julian Date in days set to J2000 epoch
		double JD_J2000 = 2457455.500000;
		//Calculate the current in-game Julian Date
		double JD_inGame = PropleticGregorianToJulianDateCalculator(GameVars.TD_year, GameVars.TD_month, GameVars.TD_day, GameVars.TD_hour, GameVars.TD_minute, GameVars.TD_second);
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
		float angleArcSec = totalPrecession;//
											//Debug.Log (angleArcSec/3600);

		//we need to calculate a point in space for the local y axis of the 'earth' to 'look at'
		float precessionAngle = 0; // in degrees
								   //precessionAngle = testPrecessionAngle;
		precessionAngle = (angleArcSec / 3600);

		float earthTilt = 23.44f; // in degrees

		float radius = 3000; //this value is arbitrary and dependent upon the y-value of the precession ring
		float precessionDiscDistance = radius / Mathf.Tan(earthTilt * Mathf.Deg2Rad); //this is a constant and will not change--it represents the height, in world space, that the precession ring exists
																					  //	--this is largely arbitrary and a higher value is used for visualization purposes
																					  // To calculate the radius or height(distance from player origin) we can use the conical formula:
																					  //	--  radius = coneHeight * tan(earthTilt)
																					  //	--  coneHeight = radius / tan(earthTilt)
																					  //	--Since the radius is used for more calculations, I want to determine an easier to use number e.g. 1, or 3000
																					  //	--height isn't used as much
																					  //The direction from the player ship to the disc will always be the ecliptic sphere's LOCAL y axis
		Vector3 precessionDiscDirection = GameVars.skybox_ecliptic_sphere.transform.up;
		//The local cordinates for a point on the circumference in the model are:
		//float horizontalDistance = radius * Mathf.Cos(precessionAngle*Mathf.Deg2Rad);
		//float forwardDistance = radius * Mathf.Sin(precessionAngle*Mathf.Deg2Rad);
		//Now we need to find this center of the conic base in unity world coordinates
		//	--We can achieve this by shooting a ray from the player ship to the direction
		//	--of the ecliptic's local y axis(which always intersects the center of the cone(a right cone)
		Ray rayToDiscOrigin = new Ray(transform.position, precessionDiscDirection);
		Vector3 discCenter_World = rayToDiscOrigin.GetPoint(precessionDiscDistance);
		//Now we need to find the point on the circumference
		//	--To do this, we'll rotate a Vector3 that points towards the local transform.left : (-) right
		//	--by the precession angle, along the local y axis
		Vector3 direction_DiscOriginToXYZOnRing = -GameVars.skybox_ecliptic_sphere.transform.right;
		direction_DiscOriginToXYZOnRing = Quaternion.AngleAxis(precessionAngle, GameVars.skybox_ecliptic_sphere.transform.up) * direction_DiscOriginToXYZOnRing;
		//Now we need to shoot a ray from the center of the precession conical base, in the direction
		//	--of this new angle in the distance of the radius of the base to find the Unity World
		//	--coordinate of the point along the precessional ring/disc/conical base.
		Ray rayFromDiscOriginToXYZOnRing = new Ray(discCenter_World, direction_DiscOriginToXYZOnRing);
		Vector3 worldCoordinateForRingXYZ = rayFromDiscOriginToXYZOnRing.GetPoint(radius);
		//Debug.Log (worldCoordinateForRingXYZ);
		//Because the world is 'mariner-centric' in that the ship is always at the center,
		//	--we need to point the celestial sphere's local.y axis from the ship's current position
		//	--to the world coordinate of the point on the ring we just discovered
		Vector3 OriginPosition = transform.position; //the player ship will always reside at the origin of space
		Vector3 PPGoal = worldCoordinateForRingXYZ;
		Vector3 direction = PPGoal - OriginPosition;
		Quaternion toRotation = Quaternion.FromToRotation(transform.up, direction);
		GameVars.skybox_MAIN_CELESTIAL_SPHERE.transform.DetachChildren();
		GameVars.skybox_celestialGrid.transform.rotation = toRotation;

		//testing....This fixed the error with changing time and the rotation not working--I'm not sure why this works..It just does
		//need to figure this blackbox I made out later. it was one experiment of many
		GameVars.skybox_MAIN_CELESTIAL_SPHERE.transform.rotation = GameVars.skybox_celestialGrid.transform.rotation;
		GameVars.skybox_ecliptic_sphere.transform.SetParent(GameVars.skybox_MAIN_CELESTIAL_SPHERE.transform);
		GameVars.skybox_celestialGrid.transform.SetParent(GameVars.skybox_MAIN_CELESTIAL_SPHERE.transform);
		//MGV.skybox_sun.transform.SetParent(MGV.skybox_MAIN_CELESTIAL_SPHERE.transform);	
		GameVars.skybox_MAIN_CELESTIAL_SPHERE.transform.localEulerAngles = new Vector3(GameVars.skybox_MAIN_CELESTIAL_SPHERE.transform.localEulerAngles.x, 0, 0);

	}

	float CalculateEarthPrecessionAngle(double numOfCenturiesBeforeOrAfterJ2000Epoch) {
		//This calculation for the total precession of the Earth is referenced from:
		//	--N. Capitaine et al 2003. Expressions for IAU 2000 precession quantities. DOI: 10.1051/0004-6361:20031539
		//--Their formula is: pA=502800".796195t + 1".1054348t^2 + 0".00007964t^3 - 0".000023857t^4 - 0".0000000383t^5
		//--Where pA is the total precession in arcseconds, and t is time in Julian Centuries +- J2000
		//The constant term of this speed (5,028.796195 arcseconds per century in above equation) 
		//	--corresponds to one full precession circle in 25,771.57534 years (one full circle of 360 degrees 
		//	--divided with 5,028.796195 arcseconds per century)
		//This is formula (39) on p.581

		double precessionSpeed;

		//Time in Julian Centuries T = 1  -> 36525 days  (365.25 days X 100) before or after J2000 epoch
		double T = numOfCenturiesBeforeOrAfterJ2000Epoch;
		double arcSecConstant = 5028.796195;

		precessionSpeed = (arcSecConstant * T) + (1.1054348 * T * T) + (0.00007964 * T * T * T) - (0.000023857 * T * T * T * T) - (0.0000000383 * T * T * T * T * T);

		return (float)precessionSpeed;

	}

	double PropleticGregorianToJulianDateCalculator(string str_y, string str_mon, string str_d, string str_h, string str_min, string str_s) {
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//THIS FUNCTION IS NOT A JULIAN --CALENDAR-- DATE CALCULATER
		//	--It converts to the Astronomical JULIAN DATE standard or JD
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//Debug.Log (str_y);
		float year = (float)int.Parse(str_y);
		float month = (float)int.Parse(str_mon);
		float day = (float)int.Parse(str_d);
		float hour = (float)int.Parse(str_h);
		float minute = (float)int.Parse(str_min);
		float second = (float)int.Parse(str_s);

		double JDN = 0; // Julian Day Number e.g. 20000
		double JD = 0; //full Julian Date e.g. 20000.5
					   //All years in BC must be converted to Astronomical years: 1 BC == 0, 2 BC == -1, etc. a 1 day ++ increment will work
		if (year < 0) year++;

		float a = Mathf.Floor(14 - month / 12); //where a = monthly offset
		float y = year + 4800 - a;          //where y = years
		float m = month + (12 * a) - 3;     //where m = months


		//Then perform the Gregorian Calendar Conversion to JDN(Julian Day Number)
		JDN = day + Mathf.Floor(((153 * m) + 2) / 5) + (365 * y) + Mathf.Floor(y / 4) - Mathf.Floor(y / 100) + Mathf.Floor(y / 400) - 32045;
		//Now Convert the JDN to the Julian Date by accounting for the hours/minutes/seconds
		JD = JDN + ((hour - 12) / 24) + (minute / 1440) + (second / 86400);

		return JD;
	}

	void RotateClouds(float angle) {
		GameVars.skybox_clouds.transform.RotateAround(transform.position, Vector3.left, angle / 4);
		GameVars.skybox_clouds.transform.position = transform.position;
	}

	public void UpdatePlayerGhostRouteLineRenderer(bool isANewGame) {

		//Update the Line Renderer with the last route position added. If a new game we initially set the origin point to the players position
		//We have to take this offset into account later because the player route array will always have 1 less in the array because it doesn't have the origin position as a separate route index(it's not a route)
		//	--rather than use Count-1 to get the last index of the line renderer, we can just use Count from the route log
		if (isANewGame) {
			GameVars.playerGhostRoute.GetComponent<LineRenderer>().positionCount = 1;
			GameVars.playerGhostRoute.GetComponent<LineRenderer>().SetPosition(0, transform.position - new Vector3(0, transform.position.y, 0)); //We subtract the y value so that the line sits on the surface of the water and not in the air
																																			//TODO this is a quick and dirty fix to load games--the origin point is already established in a loaded game so if we add 1 to the index, it creates a 'blank' Vector.zero route index in the ghost trail
		}
		else if (GameVars.isLoadedGame) {
			GameVars.playerGhostRoute.GetComponent<LineRenderer>().positionCount = journey.routeLog.Count;
			//TODO This is a quick fix--we use a 0,0,0 to designate the settlement as a stopping points rather than a normal one. This ruins the ghost trail however so we will just use position [0] instead --which just makes no visual diference in the trail
			if (journey.routeLog[journey.routeLog.Count - 1].theRoute[1].x < 1)
				GameVars.playerGhostRoute.GetComponent<LineRenderer>().SetPosition(journey.routeLog.Count - 1, journey.routeLog[journey.routeLog.Count - 1].theRoute[0] - new Vector3(0, transform.position.y, 0));//we always use the destination coordinate of the route, because the origin point was already added the last time so [1] position			
			else
				GameVars.playerGhostRoute.GetComponent<LineRenderer>().SetPosition(journey.routeLog.Count - 1, journey.routeLog[journey.routeLog.Count - 1].theRoute[1] - new Vector3(0, transform.position.y, 0));//we always use the destination coordinate of the route, because the origin point was already added the last time so [1] position		

			//if it isn't a loaded game then do the original code
		}
		else {
			GameVars.playerGhostRoute.GetComponent<LineRenderer>().positionCount = journey.routeLog.Count + 1;//we add one here because the route list never includes the origin position--so we add it manually for a new game
																										 //TODO This is a quick fix--we use a 0,0,0 to designate the settlement as a stopping points rather than a normal one. This ruins the ghost trail however so we will just use position [0] instead --which just makes no visual diference in the trail

			if (journey.routeLog[journey.routeLog.Count - 1].theRoute[1].x < 1) {
				GameVars.playerGhostRoute.GetComponent<LineRenderer>().SetPosition(journey.routeLog.Count, journey.routeLog[journey.routeLog.Count - 1].theRoute[0] - new Vector3(0, transform.position.y, 0));//we always use the destination coordinate of the route, because the origin point was already added the last time so [1] position		

			}
			else {
				GameVars.playerGhostRoute.GetComponent<LineRenderer>().SetPosition(journey.routeLog.Count, journey.routeLog[journey.routeLog.Count - 1].theRoute[1] - new Vector3(0, transform.position.y, 0));//we always use the destination coordinate of the route, because the origin point was already added the last time so [1] position		
			}
		}
	}


	void CalculateShipTrajectoryPreview(Vector3 destination) {
		Vector3 shipPosition = transform.position;
		bool isCalculatingRoute = true;
		float shipSpeedMagnitude = 0;
		Vector3 travelDirection = Vector3.zero;
		float distance = 0f;
		Vector3 windAndWaterVector = Vector3.zero;
		Vector3 lastPlayerPosition = Vector3.zero;
		List<Vector3> trajectoryToDraw = new List<Vector3>();

		int infiniteLoopBreak = 0;

		//Add the first point on the route--the player's current position
		trajectoryToDraw.Add(shipPosition - new Vector3(0, shipPosition.y, 0));

		//Now we need to figure out the actual trajectory along the chosen path 
		while (isCalculatingRoute) {
			//Get direction to settlement from ship
			travelDirection = Vector3.Normalize(destination - shipPosition);
			distance = Vector3.Distance(destination, shipPosition);
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
				playerMovementVector = ((travelDirection * shipSpeed_Actual) + windAndWaterVector) * shipSpeedModifiers.Game / 20; //This / 20  value is to set the resolution of the line--e.g. the slower the ship, the more segments to the line
																																   //Debug.Log (Vector3.Angle(travelDirection, playerMovementVector));
				if (Vector3.Angle(travelDirection, playerMovementVector) < 160f) {
					//playerMovementMagnitude = playerMovementVector.magnitude;
					//Move the ship
					shipPosition += playerMovementVector;
					trajectoryToDraw.Add(shipPosition - new Vector3(0, shipPosition.y, 0));
				}
				else {
					isCalculatingRoute = false;
				}
			}
			else {
				isCalculatingRoute = false;
			}
			//Make sure we break the while loop if it iterates over 100 times so we add the counter here
			infiniteLoopBreak++;
			if (infiniteLoopBreak > 250) { isCalculatingRoute = false; }

		}

		//Now draw the actual trajectory to the screen
		LineRenderer line = GameVars.playerTrajectory.GetComponent<LineRenderer>();
		line.positionCount = trajectoryToDraw.Count;
		for (int i = 0; i < trajectoryToDraw.Count; i++) {
			line.SetPosition(i, trajectoryToDraw[i]);
		}


	}

	public void UpdateNavigatorBeaconAppearenceBasedOnDistance(GameObject beacon) {
		//Get position of player and beacon
		Vector3 playerPos = transform.position;
		Vector3 beaconPos = beacon.transform.position;
		//Get Distance
		float distance = Vector3.Distance(playerPos, beaconPos);

		//If the distance is less than 100, start fading the beacon to transparent, and fading it's size and reverse
		//	that if it is 100. We'll be using the same formula to fade the celestial sphere colors.

		//Update size
		float calculatedWidth = Utils.GetRange(distance, 0, 100f, .1f, 5f);
		beacon.GetComponent<LineRenderer>().startWidth = calculatedWidth;
		beacon.GetComponent<LineRenderer>().endWidth = calculatedWidth;

		Color colorEnd = new Color(6f / 255f, 167f / 255f, 1f, 0);

		//Update transparency
		float alpha = Utils.GetRange(distance, 0, 100f, 0, 1f);
		beacon.GetComponent<LineRenderer>().startColor = new Color(88f / 255f, 1f, 211 / 255f, alpha);
		beacon.GetComponent<LineRenderer>().endColor = colorEnd;
	}


	public Vector3 GetCurrentWindWaterForceVector(Vector3 travelDirection) {
		Vector3 windAndWaterVector = Vector3.zero;
		Vector3 windVector = Vector3.zero;
		Vector3 waterVector = Vector3.zero;

		if (!rayCheck_stopCurrents) waterVector = currentWaterDirectionVector;
		if (GameVars.sailsAreUnfurled) windVector = currentWindDirectionVector;

		windAndWaterVector = (waterVector * currentWaterDirectionMagnitude) + (windVector * currentWindDirectionMagnitude);

		//This sets the ship's actual speed once the vectors have been taken into account. The value is stored in a public variable accessible by all functions
		current_shipSpeed_Magnitude = ((travelDirection * shipSpeed_Actual) + windAndWaterVector).magnitude;

		return windAndWaterVector;
	}

	public void AnimateCursorRing() {

		//First determine if enough time has passed to animate the sprite
		if (Time.time > cursorRingAnimationClock) {
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

		//Make sure the cursor's z axis is always facing the player's camera
		cursorRing.transform.LookAt(GameVars.FPVCamera.transform.position);
		//cursorRing.transform.eulerAngles = -cursorRing.transform.eulerAngles; 
		cursorRing.transform.eulerAngles = new Vector3(105f, cursorRing.transform.eulerAngles.y, -1 * cursorRing.transform.eulerAngles.z);
	}


	public void CheckIfPlayerFoundKnownSettlementAndTurnGhostTrailBackOn(int ID) {
		//Check if the current settlement approach area is part of the player's knowledge network--if it is, then turn the memory
		//ghost route back on and alert the player to finding their way back to known locations
		foreach (int id in ship.playerJournal.knownSettlements) {
			Debug.Log(id + " =? " + ID);
			//if we find a match then turn the memory ghost route back on
			if (id == ID) {
				Debug.Log("Found match in memory lookup");
				string settlementName = GameVars.GetSettlementFromID(id).name;
				GameVars.playerGhostRoute.SetActive(true);
				GameVars.notificationMessage = "After a long and difficult journey, you and your crew finally found your bearings in the great sea!" +
										  " You and your crew recognize the waters surrounding " + settlementName + " and remember the sea routes," +
										  " you are all familiar with!";
				GameVars.showNotification = true;
				break;
			}
		}
	}


	public void DetectCoastLinesWithRayCasts() {
		//This function Detects coast lines from 3 separate distances:
		//	--long range: roughly 15km detects coast lines to determine if seagull sound occurs
		//	--medium range: if the player is within 4km of the coast line they will no longer be affected by sea currents--this represents the benefit of sticking close to coastlines vs going asea
		//	--short range : detects if coast line is super close and then stops the player from moving if it's the case(this is to prevent the player clipping into terrain or land)
		//It shoots 8 rays from the center of the ship at equal 45 deg angles--more than 8 is probably not necessary, but more can be added if necessary

		//We'll shoot starting from the 12 o clock position and work counter clock wise
		//Every time the function is called the bools are reset to false--they will turn true if ANY ray cast fits the criteria
		float maxDistance = 25f;
		float distCoastLine = .1f;
		float distStopCurrent = 4f;
		bool stopShip = false;
		bool stopCurrents = false;
		bool playBirdSong = false;
		//set the layer mask to only check for collisions on layer 10 ("terrain")
		int terrainLayerMask = 1 << 10;
		RaycastHit hitInfo;
		Vector3 rayOrigin = transform.position + new Vector3(0, -.23f, 0);
		//12 O clock
		//Debug.DrawRay(rayOrigin, transform.forward*maxDistance, Color.yellow);
		if (Physics.Raycast(rayOrigin, transform.forward, out hitInfo, maxDistance, terrainLayerMask)) {
			//Debug.Log ("Hit coastline");
			if (hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine) {
				//If less than 1km~ stop ship movement
				stopShip = true;// Debug.Log ("Stopping Ship on Coast Line");
			}
			if (hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent) {
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if (hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance) {
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//1.5 o Clock
		}
		if (Physics.Raycast(rayOrigin, transform.forward + transform.right, out hitInfo, maxDistance, terrainLayerMask)) {
			//Debug.Log ("Hit coastline");
			if (hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine) {
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if (hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent) {
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if (hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance) {
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//3 o clock
		}
		if (Physics.Raycast(rayOrigin, transform.right, out hitInfo, maxDistance, terrainLayerMask)) {
			//Debug.Log ("Hit coastline");
			if (hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine) {
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if (hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent) {
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if (hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance) {
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//4.5 o clock	
		}
		if (Physics.Raycast(rayOrigin, -transform.forward + transform.right, out hitInfo, maxDistance, terrainLayerMask)) {
			//Debug.Log ("Hit coastline");
			if (hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine) {
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if (hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent) {
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if (hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance) {
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//6 o clock	
		}
		if (Physics.Raycast(rayOrigin, -transform.forward, out hitInfo, maxDistance, terrainLayerMask)) {
			//Debug.Log ("Hit coastline");
			if (hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine) {
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if (hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent) {
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if (hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance) {
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//7.5 o clock		
		}
		if (Physics.Raycast(rayOrigin, -transform.forward - transform.right, out hitInfo, maxDistance, terrainLayerMask)) {
			//Debug.Log ("Hit coastline");
			if (hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine) {
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if (hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent) {
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if (hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance) {
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//9 o clock	
		}
		if (Physics.Raycast(rayOrigin, -transform.right, out hitInfo, maxDistance, terrainLayerMask)) {
			//Debug.Log ("Hit coastline");
			if (hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine) {
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if (hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent) {
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if (hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance) {
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
			//10.5 o clock		
		}
		if (Physics.Raycast(rayOrigin, transform.forward - transform.right, out hitInfo, maxDistance, terrainLayerMask)) {
			//Debug.Log ("Hit coastline");
			if (hitInfo.distance >= 0 && hitInfo.distance <= distCoastLine) {
				//If less than 1km~ stop ship movement
				stopShip = true;//Debug.Log ("Stopping Ship on Coast Line");
			}
			if (hitInfo.distance > distCoastLine && hitInfo.distance <= distStopCurrent) {
				//If within 4km~ turn off currents
				stopCurrents = true;//Debug.Log ("Stopping Ocean Currents");
			}
			if (hitInfo.distance > distStopCurrent && hitInfo.distance <= maxDistance) {
				//if within 15km~ turn on seagulls
				playBirdSong = true;
			}
		}

		//now set the main bool flags to the temp flags
		rayCheck_stopShip = stopShip;
		rayCheck_stopCurrents = stopCurrents;
		rayCheck_playBirdSong = playBirdSong;
	}

	public void KeepNearestWindZonesOn(string playerZoneName) {
		//This function turns on the surrounding wind/water zones of the current wind/water zone
		//It assumes the zones have a x,y coordinate system for their object 'name'
		//If the zone name containing the player ship is  6_8  (x_y), then we turn on the surrounding cells using simple math
		//Clear the current list
		windZoneNamesToTurnOn.Clear();

		char[] lineDelimiter = new char[] { '_' };
		int x = 0;
		int y = 0;
		string[] xy = playerZoneName.Split(lineDelimiter);
		x = int.Parse(xy[0]);
		y = int.Parse(xy[1]);

		//Add the new matrix of named objects that surround the player
		windZoneNamesToTurnOn.Add((x - 1) + "_" + (y - 1));   windZoneNamesToTurnOn.Add((x) + "_" + (y - 1));    windZoneNamesToTurnOn.Add((x + 1) + "_" + (y - 1));
		windZoneNamesToTurnOn.Add((x - 1) + "_" + (y));       windZoneNamesToTurnOn.Add((x) + "_" + (y));        windZoneNamesToTurnOn.Add((x + 1) + "_" + (y));
		windZoneNamesToTurnOn.Add((x - 1) + "_" + (y + 1));   windZoneNamesToTurnOn.Add((x) + "_" + (y + 1));    windZoneNamesToTurnOn.Add((x + 1) + "_" + (y + 1));


		foreach (string zoneName in windZoneNamesToTurnOn) {
			//TODO:This is a quick and dirty way to make sure we don't get errors when on game world edge trying to turn zones on/off that don't exist
			try {
				Debug.Log("WIND" + zoneName);
				GameVars.windZoneParent.transform.Find(zoneName).transform.GetChild(0).gameObject.SetActive(true);
			}
			catch { }
		}
	}


	public void KeepNearestWaterZonesOn(string playerZoneName) {
		//This function turns on the surrounding wind/water zones of the current wind/water zone
		//It assumes the zones have a x,y coordinate system for their object 'name'
		//If the zone name containing the player ship is  6_8  (x_y), then we turn on the surrounding cells using simple math
		//Clear the current list
		currentZoneNamesToTurnOn.Clear();

		char[] lineDelimiter = new char[] { '_' };
		int x = 0;
		int y = 0;
		string[] xy = playerZoneName.Split(lineDelimiter);
		x = int.Parse(xy[0]);
		y = int.Parse(xy[1]);

		//Add the new matrix of named objects that surround the player
		currentZoneNamesToTurnOn.Add((x - 1) + "_" + (y - 1));    currentZoneNamesToTurnOn.Add((x) + "_" + (y - 1));    currentZoneNamesToTurnOn.Add((x + 1) + "_" + (y - 1));
		currentZoneNamesToTurnOn.Add((x - 1) + "_" + (y));        currentZoneNamesToTurnOn.Add((x) + "_" + (y));        currentZoneNamesToTurnOn.Add((x + 1) + "_" + (y));
		currentZoneNamesToTurnOn.Add((x - 1) + "_" + (y + 1));    currentZoneNamesToTurnOn.Add((x) + "_" + (y + 1));    currentZoneNamesToTurnOn.Add((x + 1) + "_" + (y + 1));


		foreach (string zoneName in currentZoneNamesToTurnOn) {
			//TODO:This is a quick and dirty way to make sure we don't get errors when on game world edge trying to turn zones on/off that don't exist
			try {
				GameVars.currentZoneParent.transform.Find(zoneName).transform.GetChild(0).gameObject.SetActive(true);
				//Debug.Log ("WATER " + zoneName + " : " + GameObject.Find(zoneName).transform.GetChild(0).name + "  -->ON<--");
			}
			catch { }
		}
	}

	public void CheckCurrentWaterWindZones() {
		//This function only needs to run once to initialize
		int windLayerMask = 1 << 20;
		int waterLayerMask = 1 << 19;
		RaycastHit hitInfo;
		//Fire Water Collider Check
		if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 200, waterLayerMask)) {
			Debug.Log("********" + hitInfo.transform.name);
			hitInfo.transform.GetChild(0).gameObject.SetActive(true);
		}
		//Fire Wind Collider Check
		if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 200, windLayerMask)) {
			Debug.Log("********" + hitInfo.transform.name);
			hitInfo.transform.GetChild(0).gameObject.SetActive(true);
		}

	}

	IEnumerator WindZoneMaintenance() {
		//This loops indefinitely--it should always be running while the game is on
		//Having an infinite loop is dangerous--but I think it should be safe. I'll have to keep an eye
		//on memory leaks etc.
		while (true) {
			for (int i = 0; i < GameVars.windZoneParent.transform.childCount; i++) {
				Transform currentChild = GameVars.windZoneParent.transform.GetChild(i);
				foreach (string zoneID in windZoneNamesToTurnOn) {
					if (currentChild.name == zoneID) {
						currentChild.GetChild(0).gameObject.SetActive(true);
						break;
					}
					else
						currentChild.GetChild(0).gameObject.SetActive(false);
				}
				yield return null;
			}
		}

	}

	IEnumerator waterCurrentZoneMaintenance() {
		//This loops indefinitely--it should always be running while the game is on
		//Having an infinite loop is dangerous--but I think it should be safe. I'll have to keep an eye
		//on memory leaks etc.
		while (true) {
			for (int i = 0; i < GameVars.currentZoneParent.transform.childCount; i++) {
				Transform currentChild = GameVars.currentZoneParent.transform.GetChild(i);
				foreach (string zoneID in currentZoneNamesToTurnOn) {
					if (currentChild.name == zoneID) {
						currentChild.GetChild(0).gameObject.SetActive(true);
						break;
					}
					else {
						currentChild.GetChild(0).gameObject.SetActive(false);
						//Debug.Log ("WATER " + zoneID + " : " + currentChild.name + "  -->OFF<--");
					}
				}
				yield return null;
			}
		}

	}

	public void PassTime(float amountToWait, bool isPort) {
		GameVars.isPassingTime = true;
		StartCoroutine(WaitForTimePassing(.25f, false));
	}

	//This function will run the clock for a given amount of time and adjust the necessary variables
	//--Like Provisions and water consumption, and skyline changes
	IEnumerator WaitForTimePassing(float amountToWait, bool isPort) {
		//Rather than one quick jump of time--let's do a smooth visual passage of time
		//I'm going to chop it to 180 frames--so roughly 3-4 seconds total

		//Additionally, we have a bool for isPort--we don't want the passage of time to affect the Provisions and water supplies when at a port
		//--The assumption is that the crew is on their own at port so the passage of time when leaving a port doe not affect the ship's hp
		//--nor Provisions stores--it only affects the time of day.
		float numOfFrames = 180f;
		float amountPerFrame = amountToWait / numOfFrames;
		for (int i = 0; i < numOfFrames; i++) {
			ship.totalNumOfDaysTraveled += amountPerFrame;
			UpdateDayNightCycle(GameVars.IS_NOT_NEW_GAME);
			if (!isPort) {
				UpdateShipAtrophyAfterTravelTime(amountPerFrame, true);
				CheckIfProvisionsOrWaterIsDepleted(amountPerFrame);
			}
			yield return null;
		}
		GameVars.isPassingTime = false;
		GameVars.controlsLocked = false;
		if (!isPort) {//If this isn't a port--then add a journey log at the end
					  //Add a new route to the player journey log
			journey.AddRoute(new PlayerRoute(transform.position, transform.position, ship.totalNumOfDaysTraveled), gameObject.GetComponent<script_player_controls>(), GameVars.currentCaptainsLog);
			//Update player ghost route
			UpdatePlayerGhostRouteLineRenderer(GameVars.IS_NOT_NEW_GAME);
		}
	}




}
