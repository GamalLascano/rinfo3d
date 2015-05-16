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

		theRobot.position = new Vector3 (Mathf.RoundToInt (theRobot.position.x),
		                                 Mathf.RoundToInt (theRobot.position.y),
		                                 Mathf.RoundToInt (theRobot.position.z));
	}
	
	/**
	 * Metodo a implementar
	 */
	public override IEnumerator derecha()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando derecha!");
		yield return new WaitForSeconds(0);
	}	


}
