using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DialogPetteia : MonoBehaviour
{
	public List<string> CaptureLines, getCapturedLines;
	public GameObject EnemyCanvas;
	public Text enemyName;
	public Text dialog;
	public Image enemyFaces;
	private const string ResourcePath = "crew_portraits";
	private const string DefaultPortrait = "crew_portraits/phoenician_sailor";

	public bool isWin;
    // Start is called before the first frame update
    void Start()
    {
		isWin = false;
		EnemyCanvas.SetActive(false);
		//enemyName.text = Globals.GameVars.currentSettlement.availableCrew.RandomElement<CrewMember>().name;
		//enemyName.text = Globals.GameVars.GetSettlementFromID(0).availableCrew.RandomElement<CrewMember>().name;
		//enemyFaces = Globals.GameVars.newGameAvailableCrew.RandomElement<CrewMember>()
		//Debug.Log(Globals.GameVars.newGameAvailableCrew.RandomElement<CrewMember>().name);
		
		CrewMember c = Globals.GameVars.currentSettlement.availableCrew.RandomElement<CrewMember>();
		enemyName.text = c.name;
		enemyFaces.sprite = Resources.Load<Sprite>(ResourcePath + "/" + c.ID) ?? Resources.Load<Sprite>(DefaultPortrait);



	}

    // Update is called once per frame
    void Update()
    {
		if (isWin) {

		}



		if (Input.GetKeyDown(KeyCode.Alpha1)){
			StartCoroutine(EnemyCapturesFunction());
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			StartCoroutine(PlayerCapturesFunction());
		}
	}

	public void EnemyCaptures() {
		if (Random.Range(0, 101) > 50) {
			StartCoroutine(EnemyCapturesFunction());
		}
	}
	public void PlayerCaptures() {
		if (Random.Range(0, 101) > 50) {
			StartCoroutine(PlayerCapturesFunction());
		}
	}

	IEnumerator EnemyCapturesFunction() {
		yield return new WaitForSeconds(1f);
		EnemyCanvas.SetActive(true);
		dialog.text = CaptureLines.RandomElement();
		//yield return new WaitForSeconds(8f);
		
		//EnemyCanvas.SetActive(false);
	}

	IEnumerator PlayerCapturesFunction() {
		yield return new WaitForSeconds(1f);
		EnemyCanvas.SetActive(true);
		dialog.text = getCapturedLines.RandomElement();
		//yield return new WaitForSeconds(8f);
		
		//EnemyCanvas.SetActive(false);
	}
	public void ResetBoard() {
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
	}
}
