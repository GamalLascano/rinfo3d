using UnityEngine;
using System.Collections;

public class SimpleRobotBehaviour : RobotBehaviour {

	/**
	 * Metodo a implementar
	 */
	public override IEnumerator mover()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando mover!");

		Transform theRobot = (Transform)Init.robotInstance;

		// TODO: Esto logicamente hay que deshardcodearlo
		for (int i = 0; i < 20; i++) {
			theRobot.Translate (Vector3.forward * .05f);
			yield return new WaitForSeconds(0);
		}

		// Llevar a la posicion justa
		theRobot.position = new Vector3 (Mathf.RoundToInt (theRobot.position.x),
		                                 Mathf.RoundToInt (theRobot.position.y),
		                                 Mathf.RoundToInt (theRobot.position.z));

		UI.executingCurrentLine = false;
	}
	
	/**
	 * Metodo a implementar
	 */
	public override IEnumerator derecha()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando derecha!");
		Transform theRobot = (Transform)Init.robotInstance;

		// TODO: Esto logicamente hay que deshardcodearlo
		for (var r = 0; r < 20; r++) {
			theRobot.localRotation = Quaternion.Euler(theRobot.localRotation.eulerAngles.x,
			                                          theRobot.localRotation.eulerAngles.y + (90f / 20f),
			                                          theRobot.localRotation.eulerAngles.z);
			yield return new WaitForSeconds(0);
		}

		// Llevar a la posicion justa
		theRobot.localRotation = Quaternion.Euler(Mathf.RoundToInt(theRobot.localRotation.eulerAngles.x),
		                                          Mathf.RoundToInt(theRobot.localRotation.eulerAngles.y),
		                                          Mathf.RoundToInt(theRobot.localRotation.eulerAngles.z));

		UI.executingCurrentLine = false; 
	}	


}
