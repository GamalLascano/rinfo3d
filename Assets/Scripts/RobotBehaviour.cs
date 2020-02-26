using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

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
				I18N.getValue("heading") + getHeading(theRobot) + "\n " +
				I18N.getValue("flowers_corner") + Init.city[posAv-1, posCa-1].flowers + ", " + 
				I18N.getValue("papers_corner") + Init.city[posAv-1, posCa-1].papers + " - " +
				I18N.getValue("flowers_bag") + flores + ", " +
				I18N.getValue("papers_bag") + papeles + " ";

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


		int heading  = Mathf.RoundToInt(theRobot.transform.rotation.eulerAngles.y);
		int deltaX = 0;
		int deltaZ = 0;
		if (heading == 0)
			deltaZ = 1;
		if (heading == 180)
			deltaZ = -1;
		if (heading == 90)
			deltaX = 1;
		if (heading == 270)
			deltaX = -1;


		// Destino
		startPos = theRobot.position;
		Vector3 endPos = new Vector3 (	Mathf.RoundToInt ( theRobot.position.x + deltaX),
		                      			Mathf.RoundToInt ( theRobot.position.y ),
		                              	Mathf.RoundToInt ( theRobot.position.z + deltaZ) 
		                      		);
		
		// Moverlo un poco
		float journeyLength = Vector3.Distance(startPos, endPos);
		float startTime = Time.time;
		float speed = UI.currentRunningSpeed * 10f;
		while (Vector3.Distance(endPos, theRobot.position) > 0) { 
			theRobot.Translate (Vector3.forward * Time.deltaTime * (UI.currentRunningSpeed * 10f));
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			theRobot.position = Vector3.Lerp(startPos, endPos, fracJourney);
			yield return new WaitForSeconds(0);
		}
		
		// Fijarlo en enteros al final
		theRobot.position = new Vector3 (Mathf.RoundToInt ( theRobot.position.x ),
		                                 Mathf.RoundToInt ( theRobot.position.y ),
		                                 Mathf.RoundToInt ( theRobot.position.z ) 
		                                 );
		yield return new WaitForSeconds(0);

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
		Transform throttle = theRobot.Find("Throttle");
		ParticleSystem ps = (ParticleSystem)throttle.GetComponent<ParticleSystem>();
        var em = ps.emission;
        em.enabled = true;
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
			yield return new WaitForSeconds(0);
		}

		// Fijarlo en enteros al final
		theRobot.position = new Vector3 (Mathf.RoundToInt ( int.Parse((string)arguments[0]) ),
		                                 Mathf.RoundToInt ( theRobot.position.y ),
		                                 Mathf.RoundToInt ( int.Parse((string)arguments[1]) ) 
		                                );
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
			yield return new WaitForSeconds(0);
		}

		em.enabled = false;
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

		yield return new WaitForSeconds(0);

		float scale = 0f;
		while (scale < 1) { 
			theRobot.localScale = new Vector3(scale, 2-scale, scale);
			scale = scale + (UI.currentRunningSpeed * 2) * Time.deltaTime;
			yield return new WaitForSeconds(0);
		}
		
		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}

	/**
	 * El robot toma una flor
	 */ 
	public virtual IEnumerator tomarFlor() {

		// Tomar flor de la esquina. 
		Vector3 pos = getRobotPosition();
		int floresEnEsquina = Init.city[(int)pos.x-1, (int)pos.z-1].flowers;
		if (floresEnEsquina == 0) {
			UI.runtimeErrorMsg = I18N.getValue("no_flowers_corner");
		}
		else {
			Init.city[(int)pos.x-1, (int)pos.z-1].decFlowers();
			flores++;
		}

		// ======== Movimiento brazos robot. TODO: modularizar (ver problema de uso con Coroutines) ========
		// Recuperar el robot
		Transform theRobot = (Transform)Init.robotInstance;
		Transform cuerpo = theRobot.Find("CuerpoRobot");
		Transform brazoIzq = cuerpo.Find("BrazoIzq");
		Transform brazoDer = cuerpo.Find("BrazoDer");
		Vector3 defPosIzq = brazoIzq.position;
		Vector3 defPosDer = brazoDer.position;
		
		// Brazos abajo!
		Vector3 startPosIzq = brazoIzq.position;
		Vector3 startPosDer = brazoDer.position;
		Vector3 endPosIzq = new Vector3(brazoIzq.position.x, brazoIzq.position.y - .15f, brazoIzq.position.z);
		Vector3 endPosDer = new Vector3(brazoDer.position.x, brazoDer.position.y - .15f, brazoDer.position.z);
		float journeyLengthIzq = Vector3.Distance(startPosIzq, endPosIzq);
		float startTimeIzq = Time.time;
		float speedIzq = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPosIzq, brazoIzq.position) > 0.001f) { 
			float distCoveredIzq = (Time.time - startTimeIzq) * speedIzq;
			float fracJourneyIzq = distCoveredIzq / journeyLengthIzq;
			brazoIzq.position = Vector3.Lerp(startPosIzq, endPosIzq, fracJourneyIzq);
			brazoDer.position = Vector3.Lerp(startPosDer, endPosDer, fracJourneyIzq);
			yield return new WaitForSeconds(0);
		}

		// Dust!
		Transform throttle = theRobot.Find("Dust");
		ParticleSystem ps = (ParticleSystem)throttle.GetComponent<ParticleSystem>();
		ps.Emit(10);
		yield return new WaitForSeconds(0);

		// Brazos arriba!
		endPosIzq = startPosIzq;
		endPosDer = startPosDer;
		startPosIzq = brazoIzq.position;
		startPosDer = brazoDer.position;
		journeyLengthIzq = Vector3.Distance(startPosIzq, endPosIzq);
		startTimeIzq = Time.time;
		speedIzq = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPosIzq, brazoIzq.position) > 0.001f) { 
			float distCoveredIzq = (Time.time - startTimeIzq) * speedIzq;
			float fracJourneyIzq = distCoveredIzq / journeyLengthIzq;
			brazoIzq.position = Vector3.Lerp(startPosIzq, endPosIzq, fracJourneyIzq);
			brazoDer.position = Vector3.Lerp(startPosDer, endPosDer, fracJourneyIzq);
			yield return new WaitForSeconds(0);
		}

		// Dejar posicion de brazos tal cual estaba
		brazoIzq.position = defPosIzq;
		brazoDer.position = defPosDer;

		// ======== FIN Movimiento brazos robot. ========


		yield return new WaitForSeconds(0);
		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}

	/**
	 * El robot deposita una flor
	 */ 
	public virtual IEnumerator depositarFlor() {

		// Depositar flor en la esquina. 
		Vector3 pos = getRobotPosition();
		if (flores == 0)
			UI.runtimeErrorMsg = I18N.getValue("no_flowers_bag");
		else { 
			flores--;
			Init.city[(int)pos.x-1, (int)pos.z-1].incFlowers();
		}


		// ======== Movimiento brazos robot. TODO: modularizar (ver problema de uso con Coroutines) ========
		// Recuperar el robot
		Transform theRobot = (Transform)Init.robotInstance;
		Transform cuerpo = theRobot.Find("CuerpoRobot");
		Transform brazoIzq = cuerpo.Find("BrazoIzq");
		Transform brazoDer = cuerpo.Find("BrazoDer");
		Vector3 defPosIzq = brazoIzq.position;
		Vector3 defPosDer = brazoDer.position;
		
		// Brazos abajo!
		Vector3 startPosIzq = brazoIzq.position;
		Vector3 startPosDer = brazoDer.position;
		Vector3 endPosIzq = new Vector3(brazoIzq.position.x, brazoIzq.position.y - .15f, brazoIzq.position.z);
		Vector3 endPosDer = new Vector3(brazoDer.position.x, brazoDer.position.y - .15f, brazoDer.position.z);
		float journeyLengthIzq = Vector3.Distance(startPosIzq, endPosIzq);
		float startTimeIzq = Time.time;
		float speedIzq = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPosIzq, brazoIzq.position) > 0.001f) { 
			float distCoveredIzq = (Time.time - startTimeIzq) * speedIzq;
			float fracJourneyIzq = distCoveredIzq / journeyLengthIzq;
			brazoIzq.position = Vector3.Lerp(startPosIzq, endPosIzq, fracJourneyIzq);
			brazoDer.position = Vector3.Lerp(startPosDer, endPosDer, fracJourneyIzq);
			yield return new WaitForSeconds(0);
		}

		// Dust!
		Transform throttle = theRobot.Find("Dust");
		ParticleSystem ps = (ParticleSystem)throttle.GetComponent<ParticleSystem>();
		ps.Emit(10);
		yield return new WaitForSeconds(0);
				
		// Brazos arriba!
		endPosIzq = startPosIzq;
		endPosDer = startPosDer;
		startPosIzq = brazoIzq.position;
		startPosDer = brazoDer.position;
		journeyLengthIzq = Vector3.Distance(startPosIzq, endPosIzq);
		startTimeIzq = Time.time;
		speedIzq = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPosIzq, brazoIzq.position) > 0.001f) { 
			float distCoveredIzq = (Time.time - startTimeIzq) * speedIzq;
			float fracJourneyIzq = distCoveredIzq / journeyLengthIzq;
			brazoIzq.position = Vector3.Lerp(startPosIzq, endPosIzq, fracJourneyIzq);
			brazoDer.position = Vector3.Lerp(startPosDer, endPosDer, fracJourneyIzq);
			yield return new WaitForSeconds(0);
		}
		
		// Dejar posicion de brazos tal cual estaba
		brazoIzq.position = defPosIzq;
		brazoDer.position = defPosDer;
		
		// ======== FIN Movimiento brazos robot. ========


		yield return new WaitForSeconds(0);
		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}

	/**
	 * El robot toma un papel
	 */ 
	public virtual IEnumerator tomarPapel() {

		// Tomar papel de la esquina. 
		Vector3 pos = getRobotPosition();
		int papelesEnEsquina = Init.city[(int)pos.x-1, (int)pos.z-1].papers;
		if (papelesEnEsquina == 0) {
			UI.runtimeErrorMsg = I18N.getValue("no_papers_corner");
		}
		else {
			Init.city[(int)pos.x-1, (int)pos.z-1].decPapers();
			papeles++;
		}

		// ======== Movimiento brazos robot. TODO: modularizar (ver problema de uso con Coroutines) ========
		// Recuperar el robot
		Transform theRobot = (Transform)Init.robotInstance;
		Transform cuerpo = theRobot.Find("CuerpoRobot");
		Transform brazoIzq = cuerpo.Find("BrazoIzq");
		Transform brazoDer = cuerpo.Find("BrazoDer");
		Vector3 defPosIzq = brazoIzq.position;
		Vector3 defPosDer = brazoDer.position;
		
		// Brazos abajo!
		Vector3 startPosIzq = brazoIzq.position;
		Vector3 startPosDer = brazoDer.position;
		Vector3 endPosIzq = new Vector3(brazoIzq.position.x, brazoIzq.position.y - .15f, brazoIzq.position.z);
		Vector3 endPosDer = new Vector3(brazoDer.position.x, brazoDer.position.y - .15f, brazoDer.position.z);
		float journeyLengthIzq = Vector3.Distance(startPosIzq, endPosIzq);
		float startTimeIzq = Time.time;
		float speedIzq = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPosIzq, brazoIzq.position) > 0.001f) { 
			float distCoveredIzq = (Time.time - startTimeIzq) * speedIzq;
			float fracJourneyIzq = distCoveredIzq / journeyLengthIzq;
			brazoIzq.position = Vector3.Lerp(startPosIzq, endPosIzq, fracJourneyIzq);
			brazoDer.position = Vector3.Lerp(startPosDer, endPosDer, fracJourneyIzq);
			yield return new WaitForSeconds(0);
		}

		// Dust!
		Transform throttle = theRobot.Find("Dust");
		ParticleSystem ps = (ParticleSystem)throttle.GetComponent<ParticleSystem>();
		ps.Emit(10);
		yield return new WaitForSeconds(0);

		// Brazos arriba!
		endPosIzq = startPosIzq;
		endPosDer = startPosDer;
		startPosIzq = brazoIzq.position;
		startPosDer = brazoDer.position;
		journeyLengthIzq = Vector3.Distance(startPosIzq, endPosIzq);
		startTimeIzq = Time.time;
		speedIzq = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPosIzq, brazoIzq.position) > 0.001f) { 
			float distCoveredIzq = (Time.time - startTimeIzq) * speedIzq;
			float fracJourneyIzq = distCoveredIzq / journeyLengthIzq;
			brazoIzq.position = Vector3.Lerp(startPosIzq, endPosIzq, fracJourneyIzq);
			brazoDer.position = Vector3.Lerp(startPosDer, endPosDer, fracJourneyIzq);
			yield return new WaitForSeconds(0);
		}
		
		// Dejar posicion de brazos tal cual estaba
		brazoIzq.position = defPosIzq;
		brazoDer.position = defPosDer;
		
		// ======== FIN Movimiento brazos robot. ========


		yield return new WaitForSeconds(0);
		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}
	
	/**
	 * El robot deposita un papel
	 */ 
	public virtual IEnumerator depositarPapel() {

		// Depositar papel en la esquina. 
		Vector3 pos = getRobotPosition();
		if (papeles == 0)
			UI.runtimeErrorMsg = I18N.getValue("no_papers_bag");
		else { 
			papeles--;
			Init.city[(int)pos.x-1, (int)pos.z-1].incPapers();
		}

		// ======== Movimiento brazos robot. TODO: modularizar (ver problema de uso con Coroutines) ========
		// Recuperar el robot
		Transform theRobot = (Transform)Init.robotInstance;
		Transform cuerpo = theRobot.Find("CuerpoRobot");
		Transform brazoIzq = cuerpo.Find("BrazoIzq");
		Transform brazoDer = cuerpo.Find("BrazoDer");
		Vector3 defPosIzq = brazoIzq.position;
		Vector3 defPosDer = brazoDer.position;
		
		// Brazos abajo!
		Vector3 startPosIzq = brazoIzq.position;
		Vector3 startPosDer = brazoDer.position;
		Vector3 endPosIzq = new Vector3(brazoIzq.position.x, brazoIzq.position.y - .15f, brazoIzq.position.z);
		Vector3 endPosDer = new Vector3(brazoDer.position.x, brazoDer.position.y - .15f, brazoDer.position.z);
		float journeyLengthIzq = Vector3.Distance(startPosIzq, endPosIzq);
		float startTimeIzq = Time.time;
		float speedIzq = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPosIzq, brazoIzq.position) > 0.001f) { 
			float distCoveredIzq = (Time.time - startTimeIzq) * speedIzq;
			float fracJourneyIzq = distCoveredIzq / journeyLengthIzq;
			brazoIzq.position = Vector3.Lerp(startPosIzq, endPosIzq, fracJourneyIzq);
			brazoDer.position = Vector3.Lerp(startPosDer, endPosDer, fracJourneyIzq);
			yield return new WaitForSeconds(0);
		}

		// Dust!
		Transform throttle = theRobot.Find("Dust");
		ParticleSystem ps = (ParticleSystem)throttle.GetComponent<ParticleSystem>();
		ps.Emit(10);
		yield return new WaitForSeconds(0);

		// Brazos arriba!
		endPosIzq = startPosIzq;
		endPosDer = startPosDer;
		startPosIzq = brazoIzq.position;
		startPosDer = brazoDer.position;
		journeyLengthIzq = Vector3.Distance(startPosIzq, endPosIzq);
		startTimeIzq = Time.time;
		speedIzq = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPosIzq, brazoIzq.position) > 0.001f) { 
			float distCoveredIzq = (Time.time - startTimeIzq) * speedIzq;
			float fracJourneyIzq = distCoveredIzq / journeyLengthIzq;
			brazoIzq.position = Vector3.Lerp(startPosIzq, endPosIzq, fracJourneyIzq);
			brazoDer.position = Vector3.Lerp(startPosDer, endPosDer, fracJourneyIzq);
			yield return new WaitForSeconds(0);
		}
		
		// Dejar posicion de brazos tal cual estaba
		brazoIzq.position = defPosIzq;
		brazoDer.position = defPosDer;
		
		// ======== FIN Movimiento brazos robot. ========


		yield return new WaitForSeconds(0);
		// Fin de ejecucion
		UI.executingCurrentLine = false;

	}
    public virtual IEnumerator programa()
    {
        UI.programName = (string)arguments[0];
        yield return new WaitForSeconds(0);
        UI.executingCurrentLine = false;
    }
    //No hace nada, se va a usar para comprobar sintaxis
    public virtual IEnumerator comenzar()
    {

        yield return new WaitForSeconds(0);
        UI.executingCurrentLine = false;
    }
    /**
 * El robot finaliza la ejecucion realizando un super-festejo
 */
    public virtual IEnumerator finalizar() {

		// ======== Movimiento brazos robot. TODO: modularizar (ver problema de uso con Coroutines) ========
		// Recuperar el robot
		Transform theRobot = (Transform)Init.robotInstance;
		Transform cuerpo = theRobot.Find("CuerpoRobot");
		Transform brazoIzq = cuerpo.Find("BrazoIzq");
		Transform brazoDer = cuerpo.Find("BrazoDer");
		Vector3 defPosIzq = brazoIzq.position;
		Vector3 defPosDer = brazoDer.position;

		// Brazos arriba!
		Vector3 startPosIzq = brazoIzq.position;
		Vector3 startPosDer = brazoDer.position;
		Vector3 endPosIzq = new Vector3(brazoIzq.position.x, brazoIzq.position.y + .15f, brazoIzq.position.z);
		Vector3 endPosDer = new Vector3(brazoDer.position.x, brazoDer.position.y + .15f, brazoDer.position.z);
		float journeyLengthIzq = Vector3.Distance(startPosIzq, endPosIzq);
		float startTimeIzq = Time.time;
		float speedIzq = UI.currentRunningSpeed * 5;
		while (Vector3.Distance(endPosIzq, brazoIzq.position) > 0.001f) { 
			float distCoveredIzq = (Time.time - startTimeIzq) * speedIzq;
			float fracJourneyIzq = distCoveredIzq / journeyLengthIzq;
			brazoIzq.position = Vector3.Lerp(startPosIzq, endPosIzq, fracJourneyIzq);
			brazoDer.position = Vector3.Lerp(startPosDer, endPosDer, fracJourneyIzq);
			yield return new WaitForSeconds(0);
		}

		yield return new WaitForSeconds(0);
		

		// Robot arriba!
		Vector3 startPos = theRobot.position;	
		int deltaY = 1;		
		startPos = theRobot.position;
		Vector3 endPos = new Vector3 (Mathf.RoundToInt ( theRobot.position.x),
		                              Mathf.RoundToInt ( theRobot.position.y + deltaY ),
		                              Mathf.RoundToInt ( theRobot.position.z));
		// Moverlo un poco
		float journeyLength = Vector3.Distance(startPos, endPos);
		float startTime = Time.time;
		float speed = UI.currentRunningSpeed * 10f;
		while (Vector3.Distance(endPos, theRobot.position) > 0) { 
			theRobot.Translate (Vector3.forward * Time.deltaTime * (UI.currentRunningSpeed * 10f));
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			theRobot.position = Vector3.Lerp(startPos, endPos, fracJourney);
			yield return new WaitForSeconds(0);
		}		
		// Fijarlo en enteros al final
		theRobot.position = new Vector3 (Mathf.RoundToInt ( theRobot.position.x ),
		                                 Mathf.RoundToInt ( theRobot.position.y ),
		                                 Mathf.RoundToInt ( theRobot.position.z ) 
		                                 );		
		
		yield return new WaitForSeconds(0);

		// Robot abajo!
		endPos = startPos;
		startPos = theRobot.position;	

		// Moverlo un poco
		journeyLength = Vector3.Distance(startPos, endPos);
		startTime = Time.time;
		speed = UI.currentRunningSpeed * 10f;
		while (Vector3.Distance(endPos, theRobot.position) > 0) { 
			theRobot.Translate (Vector3.forward * Time.deltaTime * (UI.currentRunningSpeed * 10f));
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			theRobot.position = Vector3.Lerp(startPos, endPos, fracJourney);
			yield return new WaitForSeconds(0);
		}		
		// Fijarlo en enteros al final
		theRobot.position = new Vector3 (Mathf.RoundToInt ( theRobot.position.x ),
		                                 Mathf.RoundToInt ( theRobot.position.y ),
		                                 Mathf.RoundToInt ( theRobot.position.z ) 
		                                 );		
		
		yield return new WaitForSeconds(0);

		// Brazos abajo!
		endPosIzq = startPosIzq;
		endPosDer = startPosDer;
		startPosIzq = brazoIzq.position;
		startPosDer = brazoDer.position;
		journeyLengthIzq = Vector3.Distance(startPosIzq, endPosIzq);
		startTimeIzq = Time.time;
		while (Vector3.Distance(endPosIzq, brazoIzq.position) > 0.001f) { 
			float distCoveredIzq = (Time.time - startTimeIzq) * speedIzq;
			float fracJourneyIzq = distCoveredIzq / journeyLengthIzq;
			brazoIzq.position = Vector3.Lerp(startPosIzq, endPosIzq, fracJourneyIzq);
			brazoDer.position = Vector3.Lerp(startPosDer, endPosDer, fracJourneyIzq);
			yield return new WaitForSeconds(0);
		}

		yield return new WaitForSeconds(0);

		// Fin de ejecucion
		UI.executingCurrentLine = false;
	}
	
	//Estructuras de control

    	public virtual IEnumerator repetir()
    {
        Transform theRobot = (Transform)Init.robotInstance;
        RobotBehaviour behaviour = (RobotBehaviour)theRobot.GetComponent<RobotBehaviour>();
        int i = 0;
        Type type = behaviour.GetType();
        int n = int.Parse((string)arguments[0]);
        string sentenceName = (string)arguments[1];
        if (n > i)
        {
            for (i = 0; i < n; i++)
            {
               
                MethodInfo methodInfo = type.GetMethod(sentenceName);
                behaviour.StartCoroutine(methodInfo.Name, 0);
            }
            
        }
        else
        {
            UI.runtimeErrorMsg = I18N.getValue("the_number_of_repetitions_was_not_specified");
        }
        yield return new WaitForSeconds(0);
            //Fin de ejecucion
            UI.executingCurrentLine = false;
        
    }
	
}
