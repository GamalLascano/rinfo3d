using UnityEngine;
using System.Collections;
using System;

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

		int posAv = Mathf.RoundToInt(theRobot.position.x);
		int posCa = Mathf.RoundToInt(theRobot.position.z);

		return 	I18N.getValue("posavenue") + posAv + ", " +
				I18N.getValue("posstreet") + posCa + ", " +
				I18N.getValue("heading") + getHeading(theRobot) + ", " +
				I18N.getValue("flowers") + flores + ", " + 
				I18N.getValue("papers")  + papeles + "\n" +
				I18N.getValue("flowers").Replace(":", "("+I18N.getValue("corner")+"): ") + Init.city[posAv-1, posCa-1].flowers + ", " +
				I18N.getValue("papers").Replace(":", "("+I18N.getValue("corner")+"): ") + Init.city[posAv-1, posCa-1].papers;
	}

	/** Retorna el heading del robot */
	public string getHeading(Transform theRobot) {
		int heading  = Mathf.RoundToInt(theRobot.transform.rotation.eulerAngles.y);
		switch (heading) {
			case 0:
				return I18N.getValue("north");
			case 90:
				return I18N.getValue("east");
			case 180:
				return I18N.getValue("south");
			case 270:
				return I18N.getValue("west");
		}
		return "-";
	}

	/**
	 * Retorna la posicion del robot redondeando a la ubicacion entera mas proxima
	 */
	public Vector3 getRobotPosition() {
		// Recuperar el robot
		Transform theRobot = (Transform)Init.robotInstance;
		return new Vector3 (Mathf.RoundToInt (theRobot.position.x),
							Mathf.RoundToInt (theRobot.position.y),
							Mathf.RoundToInt (theRobot.position.z));
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
			UI.robotMoved();
			yield return new WaitForSeconds(0);
		}
		
		// Llevar a la posicion justa
		theRobot.position = new Vector3 (Mathf.RoundToInt (theRobot.position.x),
		                                 Mathf.RoundToInt (theRobot.position.y),
		                                 Mathf.RoundToInt (theRobot.position.z));
		UI.robotMoved();

		// Fin de ejecucion
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

		// Fin de ejecucion
		UI.executingCurrentLine = false; 

	}

	/**
	 * Informa texto en pantalla
	 */
	public virtual IEnumerator Informar() { 

		string message = (string)arguments[0];
		UI.informarMessage = message;

		while (UI.informarMessage != null)
			yield return new WaitForSeconds(0);

		// Fin de ejecucion
		UI.executingCurrentLine = false; 
	}

	/**
	 * Ubica al robot en la posicion dada segun los argumentos
	 */ 
	public virtual IEnumerator Pos() {

		// Recuperar el robot
		Transform theRobot = (Transform)Init.robotInstance;

		// Throttle ON!
		Transform throttle = theRobot.FindChild("Throttle");
		ParticleSystem ps = (ParticleSystem)throttle.GetComponent<ParticleSystem>();
		ps.enableEmission = true;
		yield return new WaitForSeconds(0);

		// Arriba!
		Vector3 startPos = theRobot.position;
		Vector3 endPos = new Vector3 (Mathf.RoundToInt ( theRobot.position.x ),
		                              Mathf.RoundToInt ( theRobot.position.y + 1 ),
		                              Mathf.RoundToInt ( theRobot.position.z ) 
		                             );
		float journeyLength = Vector3.Distance(startPos, endPos);
		float startTime = Time.time;
		float speed = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPos, theRobot.position) > 0) { 
			theRobot.Translate (Vector3.forward * Time.deltaTime * (UI.currentRunningSpeed * 10));
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			theRobot.position = Vector3.Lerp(startPos, endPos, fracJourney);
			UI.robotMoved();
			yield return new WaitForSeconds(0);
		}

		// Destino
		startPos = theRobot.position;
		endPos = new Vector3 (Mathf.RoundToInt ( int.Parse((string)arguments[0]) ),
		                      Mathf.RoundToInt ( theRobot.position.y ),
		                      Mathf.RoundToInt ( int.Parse((string)arguments[1]) ) 
		                     );

		// Moverlo un poco
		journeyLength = Vector3.Distance(startPos, endPos);
		startTime = Time.time;
		speed = UI.currentRunningSpeed * 10;
		while (Vector3.Distance(endPos, theRobot.position) > 0) { 
			theRobot.Translate (Vector3.forward * Time.deltaTime * (UI.currentRunningSpeed * 10));
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			theRobot.position = Vector3.Lerp(startPos, endPos, fracJourney);
			UI.robotMoved();
			yield return new WaitForSeconds(0);
		}

		// Fijarlo en enteros al final
		theRobot.position = new Vector3 (Mathf.RoundToInt ( int.Parse((string)arguments[0]) ),
		                                 Mathf.RoundToInt ( theRobot.position.y ),
		                                 Mathf.RoundToInt ( int.Parse((string)arguments[1]) ) 
		                                );
		UI.robotMoved();
		yield return new WaitForSeconds(0);

		// Abajo!
		startPos = theRobot.position;
		endPos = new Vector3 (Mathf.RoundToInt ( theRobot.position.x ),
		                      Mathf.RoundToInt ( theRobot.position.y - 1 ),
		                      Mathf.RoundToInt ( theRobot.position.z ) 
		                     );
		journeyLength = Vector3.Distance(startPos, endPos);
		startTime = Time.time;
		speed = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPos, theRobot.position) > 0) { 
			theRobot.Translate (Vector3.forward * Time.deltaTime * (UI.currentRunningSpeed * 10));
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			theRobot.position = Vector3.Lerp(startPos, endPos, fracJourney);
			UI.robotMoved();
			yield return new WaitForSeconds(0);
		}

		ps.enableEmission = false;
		yield return new WaitForSeconds(0);

		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}


	/**
	 * Inicializa al robot en la posicion dada segun los argumentos
	 */ 
	public virtual IEnumerator Iniciar() {
		// Ubicacion inicial segun parametros, apuntando al norte
		Transform theRobot = (Transform)Init.robotInstance;

		theRobot.localRotation = Quaternion.Euler(Mathf.RoundToInt (theRobot.localRotation.eulerAngles.x),
		                                          0,
		                                          Mathf.RoundToInt (theRobot.localRotation.eulerAngles.z));

		theRobot.position = new Vector3 (Mathf.RoundToInt ( int.Parse((string)arguments[0]) ),
		                                 Mathf.RoundToInt ( 0 ),
		                                 Mathf.RoundToInt ( int.Parse((string)arguments[1]) ) 
		                                 );
		UI.robotMoved();
		yield return new WaitForSeconds(0);

		float scale = 0f;
		while (scale < 1) { 
			theRobot.localScale = new Vector3(scale, 2-scale, scale);
			scale = scale + (UI.currentRunningSpeed * 2) * Time.deltaTime;
			UI.robotMoved();
			yield return new WaitForSeconds(0);
		}
		
		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}

	/**
	 * El robot toma una flor
	 */ 
	public virtual IEnumerator tomarFlor() {

		// Tomar flor de la esquina. TODO: Validar existencia en la esquina
		Vector3 pos = getRobotPosition();
		int floresEnEsquina = Init.city[(int)pos.x-1, (int)pos.z-1].flowers;
		if (floresEnEsquina == 0) {
			UI.runtimeErrorMsg = I18N.getValue("no_flowers_corner");
		}
		else {
			Init.city[(int)pos.x-1, (int)pos.z-1].flowers--;
			flores++;
		}

		yield return new WaitForSeconds(0);
		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}

	/**
	 * El robot deposita una flor
	 */ 
	public virtual IEnumerator depositarFlor() {

		// Depositar flor en la esquina. TODO: Validar existencia en la bolsa
		Vector3 pos = getRobotPosition();
		if (flores == 0)
			UI.runtimeErrorMsg = I18N.getValue("no_flowers_bag");
		else { 
			flores--;
			Init.city[(int)pos.x-1, (int)pos.z-1].flowers++;
		}
		yield return new WaitForSeconds(0);
		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}

	/**
	 * El robot toma un papel
	 */ 
	public virtual IEnumerator tomarPapel() {

		// Tomar papel de la esquina. TODO: Validar existencia en la esquina
		Vector3 pos = getRobotPosition();
		int papelesEnEsquina = Init.city[(int)pos.x-1, (int)pos.z-1].papers;
		if (papelesEnEsquina == 0) {
			UI.runtimeErrorMsg = I18N.getValue("no_papers_corner");
		}
		else {
			Init.city[(int)pos.x-1, (int)pos.z-1].papers--;
			papeles++;
		}

		yield return new WaitForSeconds(0);
		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}
	
	/**
	 * El robot deposita un papel
	 */ 
	public virtual IEnumerator depositarPapel() {

		// Depositar papel en la esquina. TODO: Validar existencia en la bolsa
		Vector3 pos = getRobotPosition();
		if (papeles == 0)
			UI.runtimeErrorMsg = I18N.getValue("no_papers_bag");
		else { 
			papeles--;
			Init.city[(int)pos.x-1, (int)pos.z-1].papers++;
		}
		
		yield return new WaitForSeconds(0);
		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}

}
