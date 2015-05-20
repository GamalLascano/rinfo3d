using UnityEngine;
using System.Collections;

public class SimpleRobotBehaviour : RobotBehaviour {

	/**
	 * Metodo a implementar
	 */
	public override IEnumerator mover()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando mover!");

		// Posicion inicial
		Transform theRobot = (Transform)Init.robotInstance;
		Vector3 startPos = theRobot.position;

		// Moverlo un poquito
		while (Vector3.Distance(startPos, theRobot.position) < 1) { 
			theRobot.Translate (Vector3.forward * Time.deltaTime * (UI.currentRunningSpeed * 10));
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
	public override IEnumerator Derecha()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando derecha!");
		Transform theRobot = (Transform)Init.robotInstance;
		Quaternion initialRotation = theRobot.localRotation;


		float angle = 0;
		while (angle < 90) {
			float step = 90 * Time.deltaTime * (UI.currentRunningSpeed * 10);
			theRobot.localRotation = Quaternion.Euler(theRobot.localRotation.eulerAngles.x,
			                                          theRobot.localRotation.eulerAngles.y + step,
			                                          theRobot.localRotation.eulerAngles.z);
			angle += step;
			yield return new WaitForSeconds(0);
		}

		// Llevar a la posicion justa
		theRobot.localRotation = Quaternion.Euler(Mathf.RoundToInt(initialRotation.eulerAngles.x),
		                                          Mathf.RoundToInt(initialRotation.eulerAngles.y) + 90,
		                                          Mathf.RoundToInt(initialRotation.eulerAngles.z));

		UI.executingCurrentLine = false; 
	}	


	/**
	 * Metodo a implementar
	 */
	public override IEnumerator Informar() {
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando informar!");

		UnityEditor.EditorUtility.DisplayDialog("RInfo3D", (string)arguments[0], "OK");	
	
		yield return new WaitForSeconds(0);

		UI.executingCurrentLine = false; 
	}

}
