using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtrl : MonoBehaviour {

	public Transform holeObj;

	//Use to switch between Force Modes
	enum ModeSwitching { Start, Force};
	ModeSwitching m_ModeSwitching;

	Rigidbody m_Rigidbody;

	private bool isGameOver = false;
	private bool hasWon = false;
	private string m_AccelString, m_AngleString, m_DistanceString;
	private float m_Accel, m_Angle;
	private float m_InitialDistance, m_InitialAngle;
	private Vector3 m_StartPos; 
	private float mass = 2.0f;
	private float speedFactor = 10.0f;
	private int frame = 0;
	private float drag = 0.0f;
	private float dragFactor = 0.05f;
	private string url_data = "file:///tmp/unity_data.txt";
	private string url_results = "file:///tmp/unity_results.txt";
	private string[] strArr;

	Vector3 vForce;

	// Use this for initialization
	void Start () {
		//You get the Rigidbody component you attach to the GameObject
		m_Rigidbody = GetComponent<Rigidbody>();
		//This starts at first mode (nothing happening yet)
		m_ModeSwitching = ModeSwitching.Start;
		//The forces typed in from the text fields (the ones you can manipulate in Game view)
		m_AccelString = "50";
		m_AngleString = "0";
		//The GameObject's starting position and Rigidbody position
		m_StartPos = transform.position;
		// compute initial distance
		m_InitialDistance = Vector3.Distance (transform.position, holeObj.transform.position);
		//Debug.Log ("Initial Distance: " + m_InitialDistance);
		// compute angle to the hole
		// sin(a) = holeObj.transform.position.z - transform.position.z / m_InitialDistance;
		m_InitialAngle = Mathf.Rad2Deg * Mathf.Asin((holeObj.transform.position.z - transform.position.z) / m_InitialDistance);
		//Debug.Log ("Angle: " + m_InitialAngle);

		// freeze the ball
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

		// check input parameters
		// StartCoroutine(CheckInputFile());
		// ReadInputDataFile ();
	}


	// Update is called once per frame
	void FixedUpdate () {
		//If the current mode is not the starting mode (or the GameObject is not reset), the force can change
		if (m_ModeSwitching != ModeSwitching.Start)
		{


		}

		//Here, switching modes depend on button presses in the Game mode
		switch (m_ModeSwitching) {
		//This is the starting mode which resets the GameObject
		case ModeSwitching.Start:
			// reset flag
			hasWon = false;
			// get UI values
			MakeCustomForce ();
			// reset frame
			frame = 0;
			// reset speed factor
			speedFactor = 10.0f;
			// reset drag
			drag = 0.0f;
			// reset the flags and variables
			isGameOver = false;
			// reset the angle
			m_InitialAngle = Mathf.Rad2Deg * Mathf.Asin((holeObj.transform.position.z - transform.position.z) / m_InitialDistance);
			// reset the distance
			m_DistanceString = "-1";
			// reset the mass and the drag
			m_Rigidbody.mass = mass;
			m_Rigidbody.drag = 0.0f;
			// enable the collider after a restart
			this.GetComponent<Collider>().enabled = true;
			//This resets the GameObject and Rigidbody to their starting positions
			transform.position = m_StartPos;
			//This resets the velocity of the Rigidbody
			m_Rigidbody.velocity = new Vector3 (0f, 0f, 0f);
			break;

			//This is Force Mode, using a continuous force on the Rigidbody considering its mass
		case ModeSwitching.Force:
			// remove the contstraint (stop)
			m_Rigidbody.constraints = RigidbodyConstraints.None;
			// compute force to apply based on the angle
			vForce = Quaternion.AngleAxis (m_InitialAngle + m_Angle, Vector3.up) * -Vector3.right;
			Debug.Log ("vForce: " + vForce.ToString("F2"));

			if (frame <= m_Accel) {
				//accelerate
				m_Rigidbody.AddForce (vForce * speedFactor, ForceMode.Acceleration);
				frame++;
			} else {
				// apply force
				m_Rigidbody.AddForce (vForce * speedFactor, ForceMode.Force);
				//increase the drag
				drag += dragFactor;
				//Debug.Log("Drag: " + drag);
				// m_DragString = drag.ToString ("F2");
				m_Rigidbody.drag = drag;
				// Debug.Log ("Velocity: " + m_Rigidbody.velocity);
				if (Mathf.Abs (m_Rigidbody.velocity.x) < 0.2) {
					if (!hasWon) {
						//stop the ball
						m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
						// set the flag
						isGameOver = true;
					}
				}
			}
			//Debug.Log ("Velocity: " + m_Rigidbody.velocity);
			break;
		}

		// test if the ball has fallen 
		if (m_Rigidbody.position.y < -5) {

			if (hasWon == false) {
				//Debug.Log ("Ball has fallen outside the green....");
				m_DistanceString = "-1";
			}
			// restart
			//StartCoroutine(Restart());
		}


		if (isGameOver ) {
			if (!hasWon) {
				// compute the distance to the hole
				float dist = Vector3.Distance (transform.position, holeObj.transform.position);
				m_DistanceString = dist.ToString ("F2");

				// restart
				//StartCoroutine(Restart());
			}
		}

	}

	//The function outputs buttons, text fields, and other interactable UI elements to the Scene in Game view
	void OnGUI()
	{
		//Getting the inputs from each text field and storing them as strings
		GUI.Label(new Rect(15, 75, 50, 20), "Accel");
		GUI.Label(new Rect(15, 105, 50, 20), "Angle");
		GUI.Label(new Rect(15, 135, 50, 20), "Distance");
		m_AccelString = GUI.TextField(new Rect(100, 75, 50, 20), m_AccelString, 25);
		m_AngleString = GUI.TextField(new Rect(100, 105, 50, 20), m_AngleString, 25);
		m_DistanceString = GUI.TextField(new Rect(100, 135, 50, 20), m_DistanceString, 25);

		//Press the button to reset the GameObject and Rigidbody
		if (GUI.Button(new Rect(10, 5, 150, 30), "Reset"))
		{
			//This switches to the start/reset case
			m_ModeSwitching = ModeSwitching.Start;
		}

		//If you press the Start Button, switch to Force state
		if (GUI.Button(new Rect(10, 40, 150, 30), "Start"))
		{
			MakeCustomForce ();
			// remove the contstraint (stop)
			m_Rigidbody.constraints = RigidbodyConstraints.None;
			//Switch to Force (apply force to GameObject)
			m_ModeSwitching = ModeSwitching.Force;
		}
	}

	//Changing strings to floats for the forces
	float ConvertToFloat(string Name)
	{
		//float.TryParse(Name, out m_Result);
		float m_Result = float.Parse(Name);
		return m_Result;
	}

	//Set the converted float from the text fields as the forces to apply to the Rigidbody
	void MakeCustomForce()
	{
		//This converts the strings to floats
		m_Accel = ConvertToFloat(m_AccelString);
		m_Angle = ConvertToFloat(m_AngleString);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.name == "hole") {
			m_DistanceString = "0";
			// stop the object
			//m_Rigidbody.velocity = new Vector3 (0f, 0f, 0f);
			// increase the mass to fall sharply
			m_Rigidbody.mass = 100;
			// fall
			this.GetComponent<Collider>().enabled = false;
			// set flag
			hasWon = true;
			// restart
			//StartCoroutine(Restart());

		}
	}

	IEnumerator Restart() {
		yield return new WaitForSeconds(2.0f);
		m_ModeSwitching = ModeSwitching.Start;
	}

	IEnumerator CheckInputFile() {
		float freq = 0.5f;
		float timer = 0;
		while (true) {
			WWW w = new WWW (url_data);
			// Check for www.error
			if (w.error != null) {
				Debug.Log ("Error reading the file: " + url_data);
			} else {
				// Parse and check for info
				string input_line = w.text;
				Debug.Log ("Input data: " + input_line);
				strArr = input_line.Split (new char[] {','});
				Debug.Log ("strArr[0]: " + strArr[0]);
			}

			while (timer < freq) {
				timer += Time.deltaTime;
				yield return null;
			}
			timer = 0f;
			yield return null;
		}	
	}

	void ReadInputDataFile() {
		WWW w = new WWW (url_data);
		// Check for www.error
		if (w.error != null) {
			Debug.Log ("Error reading the file: " + url_data);
		} else {
			// Parse and check for info
			string input_line = w.text;
			Debug.Log ("Input data: " + input_line);
			strArr = input_line.Split (new char[] {','});
			Debug.Log ("strArr[0]: " + strArr[0]);
		}
	}



}
