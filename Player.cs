using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


	//---------------------Player variables---------------------//

	public Camera mainCamera = null;
	public int maxHealth = 100;
	public int currentHealth;
	public int playerMode = 0; // 0 is Movement Mode, 1 is Camera Mode

	public int currentZoomLevel = 3;
	public int maxZoomLevel = 1;
	public int minZoomLevel = 3;

	public float rotSpeed = 50f; // Player rotation speed, also used for camera rotation
	public float speed = 10f; // Player movement speed
	public float zoomAmount = 20f; // Camera Mode zoom amount

	public GameObject currentWeapon = null; // Currently Equipped weapon, if any
	public GameObject interractableObject = null; // Object in range, that can be interacted with.
	public GameObject cameraObject = null; // Object currently viewed in Camera Mode
	public GameObject attachPoint = null;  // The point which equipped items are attached to

	// For checking if moving, mode change is only allowed while not moving

	public bool playerMoving = false;

	//----------------------------------------------------------//


	void ChangeMode(){
		if (!playerMoving) {
			switch (playerMode) {
			case 0:
				playerMode = 1;
				break;
			case 1:
				playerMode = 0;
				cameraObject = null;
				break;
			}
		}
	}

	void Interact(){
		if (interractableObject.GetComponent<WeaponScript> ()) {
			EquipWeapon (interractableObject);
			interractableObject = null;
		} 
		else
			interractableObject.GetComponent<InteractScript> ().Interact (); // Results of specific interactions are handled by objects themselves.
	}

	void EquipWeapon(GameObject weapon){
		if (currentWeapon != null) { // If player currently has a weapon, it is swapped with the new weapon
			currentWeapon.transform.SetParent (null);
			currentWeapon.transform.position = weapon.transform.position;
			currentWeapon = weapon;
			currentWeapon.transform.position = attachPoint.transform.position;
			currentWeapon.transform.SetParent(transform);
		} 
		else {
			currentWeapon = interractableObject;
			currentWeapon.transform.position = attachPoint.transform.position;
			currentWeapon.transform.SetParent(transform);
			interractableObject = null;
		}
	}

	void TakeDamage (int damage){
		currentHealth -= damage;
	}
		
	void Start () {
		mainCamera = FindObjectOfType<Camera> ();
		attachPoint = GameObject.Find ("AttachPoint");
		currentHealth = maxHealth;
	}

	void Update () {

		//---------------------Movement Mode controls---------------------//

		if (playerMode == 0) {
			mainCamera.transform.rotation = transform.rotation;

			float translation = Input.GetAxis ("Vertical") * speed * Time.deltaTime;
			float rotation = Input.GetAxis ("Horizontal") * rotSpeed * Time.deltaTime;
			transform.Translate (0, 0, translation);
			transform.Rotate (0, rotation, 0);

			if (Input.GetButtonDown ("Attack")) {
				if (currentWeapon != null) 
					currentWeapon.GetComponent<WeaponScript> ().Attack ();	
			}

			if (Input.GetButtonDown ("Block")) {
				if (currentWeapon != null) 
					currentWeapon.GetComponent<WeaponScript> ().Block ();	
			}

			if (Input.GetButtonDown ("Interact")) {
				if (interractableObject != null) {
					Interact ();
				}
			}
		}

		//---------------------Camera Mode controls---------------------//

		if (playerMode == 1) {

			Ray cameraRay = mainCamera.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
			RaycastHit rcHit;
			if (Physics.Raycast (cameraRay, out rcHit) && rcHit.transform.parent != null) {
				//Debug.Log ("Camera looking at " + rcHit.transform.name);
				cameraObject = rcHit.transform.parent.gameObject;
			} 
			else
				cameraObject = null;

			float upDownRotation = -Input.GetAxis ("Vertical") * rotSpeed * Time.deltaTime;
			float leftRightRotation = Input.GetAxis ("Horizontal") * rotSpeed * Time.deltaTime;
	
			mainCamera.transform.Rotate (upDownRotation, 0, 0);
			transform.Rotate (0, leftRightRotation, 0);

			if (mainCamera.transform.localPosition.z < cameraInitialPosition.z) {
				mainCamera.transform.localPosition = cameraInitialPosition;
			}

			if (Input.GetButtonDown ("ZoomIn")) {
				if (currentZoomLevel > maxZoomLevel) {
					currentZoomLevel--;
					mainCamera.fieldOfView = currentZoomLevel * zoomAmount;
				}
			}
			if (Input.GetButtonDown ("ZoomOut")) {
				if (currentZoomLevel < minZoomLevel) {
					currentZoomLevel++;
					mainCamera.fieldOfView = currentZoomLevel * zoomAmount;
				}
			}
		}


		//--------------------General input--------------------//

		if (Input.GetButtonDown ("Menu")) {

		}
		if (Input.GetButtonDown ("ChangeMode")) {
			ChangeMode ();
		}

		if (Input.GetAxisRaw ("Vertical") != 0 || Input.GetAxisRaw ("Horizontal") != 0)
			playerMoving = true;
		else
			playerMoving = false;


		//-----------------------------------------------------//
	}
}
