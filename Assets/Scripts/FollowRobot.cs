using UnityEngine;
using System.Collections;

public class FollowRobot : MonoBehaviour {
    Transform theRobot;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Init.robotInstance == null)
			return;
        if (Init.robotInstance.Count > 1)
        {
            if (Init.robotInstance[1].robInstance != null)
            {
                theRobot = (Transform)Init.robotInstance[1].robInstance;
            } else theRobot = (Transform)Init.robotInstance[0].robInstance;

        }
        else
        {
            theRobot = (Transform)Init.robotInstance[0].robInstance;
        }
        transform.position = new Vector3(theRobot.position.x, Mathf.RoundToInt(transform.position.y), theRobot.position.z - UI.pan);
		transform.LookAt(theRobot);
	}
}
