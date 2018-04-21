using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerp : MonoBehaviour {

	public Transform holeObj;

	//Use to switch between Force Modes
	enum ModeSwitching { Start, Force};
	ModeSwitching m_ModeSwitching;

	private float lerpTime =2.0f;           // original: 2.0f
	private float accelFactor = 27.0f;     // original: 25.0f
	private float dragFactor = 0.1f;
	private float dragTrigger = 0.99f;

	float currentLerpTime;
	Vector3 startPos;
	float m_Angle = 5.0f;
	Rigidbody m_Rigidbody;
	Vector3 vForce;
	private float m_InitialDistance, m_InitialAngle;
	private float drag = 0.0f;

	protected void Start() {
		m_Rigidbody = GetComponent<Rigidbody>();
		startPos = transform.position;

		// compute initial distance
		m_InitialDistance = Vector3.Distance (transform.position, holeObj.transform.position);
		Debug.Log ("Initial Distance: " + m_InitialDistance);

		// compute angle to the hole
		// sin(a) = holeObj.transform.position.z - transform.position.z / m_InitialDistance;
		m_InitialAngle = Mathf.Rad2Deg * Mathf.Asin((holeObj.transform.position.z - transform.position.z) / m_InitialDistance);
		//Debug.Log ("Angle: " + m_InitialAngle);
	}

	void FixedUpdate () {

		//Here, switching modes depend on button presses in the Game mode
		switch (m_ModeSwitching) {
		//This is the starting mode which resets the GameObject
		case ModeSwitching.Start:
			currentLerpTime = 0f;
			// replace the ball
			transform.position = startPos;
			// reset the drag
			drag = 0.0f;
			m_Rigidbody.drag = 0f;
			break;

		case ModeSwitching.Force:
			// remove constraints
			m_Rigidbody.constraints = RigidbodyConstraints.None;
			// compute force to apply based on the angle
			vForce = Quaternion.AngleAxis (m_InitialAngle + m_Angle, Vector3.up) * -Vector3.right;
			//Debug.Log ("vForce: " + vForce);
			//increment timer once per frame
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime > lerpTime) {
				currentLerpTime = lerpTime;
			}
			// acceleration/deceleration function (sigmoid)
			float t = currentLerpTime / lerpTime;
			t = t * t * (3f - 2f * t);
			// Debug.Log ("t: " + t);
			m_Rigidbody.AddForce (vForce * (1.0f - t) * accelFactor, ForceMode.Force);

			// almost stopped, apply drag
			if (t > dragTrigger) {
				drag += dragFactor;
				m_Rigidbody.drag = drag;
			}

			break;
		}

		//start when we press spacebar
		if (Input.GetKeyDown (KeyCode.Space)) {
			currentLerpTime = 0f;
			m_ModeSwitching = ModeSwitching.Force;
		}

		//reset when we press escape
		if (Input.GetKeyDown (KeyCode.Escape)) {
			currentLerpTime = 0f;
			m_ModeSwitching = ModeSwitching.Start;
		}

		
	}
}
