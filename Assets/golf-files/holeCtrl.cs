using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class holeCtrl : MonoBehaviour {

	private float[] pos_1 = { 0.0f, -0.624f, 1.08f, 5.33f, -5.94f, -2.23f};  // Position, Rotation (3 val each)
	private float[] pos_2 = { 0.0f, -0.715f, 3.49f, -3.0f, -5.5f, -2.24f}; 
	private float[] pos_3 = { 1.52f, -0.29f, -2.32f, 1.63f, -5.5f, -2.24f}; 
	private float[] pos;

	// Use this for initialization
	void Start () {

		// slect random hole position (between 1 and 3)
		int nb = Random.Range (1, 4);

		//select 
		if (nb == 1) {
			pos = pos_1;
		} else if (nb == 2) {
			pos = pos_2;
		} else {
			pos = pos_3;
		}

		// for testing purposes only
		pos = pos_2;

		// position the hole
		Vector3 hole_pos = new Vector3(pos[0], pos[1], pos[2]);
		transform.position = hole_pos;
		Vector3 hole_rot = new Vector3(pos[3], pos[4], pos[5]);
		transform.rotation = Quaternion.Euler(hole_rot);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
