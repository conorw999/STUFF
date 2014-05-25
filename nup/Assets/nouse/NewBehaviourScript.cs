using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	public float movementSpeed = 15.0f;

	public float mouseRot = 15.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//Rotation

		float rotLeftRight = Input.GetAxis ("Mouse X") * mouseRot;
		transform.Rotate (0, rotLeftRight, 0);

		float rotUpDown = Input.GetAxis ("Mouse Y") * -mouseRot;
		Camera.main.transform.Rotate ( rotUpDown, 0, 0 );

		//Movement
		float forwardSpeed = Input.GetAxis ("Vertical") * movementSpeed;
		float sideSpeed = Input.GetAxis ("Horizontal") * movementSpeed;

		Vector3 speed = new Vector3 (sideSpeed, 0, forwardSpeed);

		speed = transform.rotation * speed;

		CharacterController cc = GetComponent<CharacterController> ();

		cc.SimpleMove (speed);
	
	}
}
