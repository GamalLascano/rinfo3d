using UnityEngine;
using System.Collections;

public abstract class RobotBehaviour : MonoBehaviour {

	// Posicion y cantidad de papeles y flores
	public int papeles = 0;
	public int flores  = 0;

	/** Argumentos a recibir en una operacion dada */
	protected ArrayList arguments = new ArrayList();

	/** Reinicia los argumentos para la siguiente instruccion */
	public void resetArguments() {
		arguments.Clear();
	}

	/** Incorpora un nuevo argumento a la nomina de argumentos */
	public void addArgument(object anArgument) {
		arguments.Add(anArgument);
	}

	/** Retorna el estado del robot */
	public string getRobotStatus() {
		Transform theRobot = (Transform)Init.robotInstance;

		return 	"PosAv: "   + Mathf.RoundToInt(theRobot.position.x) + ", " +
				"PosCa: "   + Mathf.RoundToInt(theRobot.position.z) + ", " +
				"Heading: " + getHeading(theRobot) + ", " +
				"Flowers: " + flores + ", " + 
				"Papers: "  + papeles;
	}

	/** Retorna el heading del robot */
	public string getHeading(Transform theRobot) {
		int heading  = Mathf.RoundToInt(theRobot.transform.rotation.eulerAngles.y);
		switch (heading) {
			case 0:
				return "N";
			case 90:
				return "E";
			case 180:
				return "S";
			case 270:
				return "O";
		}
		return "-";
	}

	/**
	 * Desplaza el robot en una posicion hacia adelante segun su heading
	 */
	public virtual IEnumerator mover() {
	
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
	 * Realiza una rotacion del robot de 90 grados hacia la derecha sobre su eje
	 */
	public virtual IEnumerator Derecha() { 
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
	 * Informa texto en pantalla
	 */
	public virtual IEnumerator Informar() { 
		
		UnityEditor.EditorUtility.DisplayDialog("RInfo3D", (string)arguments[0], "OK");	
		
		yield return new WaitForSeconds(0);
		
		UI.executingCurrentLine = false; 
	}
	
}
