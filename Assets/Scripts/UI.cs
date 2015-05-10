﻿using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

	// Posibles estados de la GUI
	public const int STATE_EDITING = 0;
	public const int STATE_RUNNING = 1;
	public const int STATE_CONFIG  = 2;

	// Camaras 
	public const int CAMARA_3DCAM   = 0;
	public const int CAMARA_HEADCAM = 1;
	public const int CAMARA_TOPCAM  = 2;
	public const int CAMERA_END_OF 	= 3;

	// Tamaño default de los botones 
	public static int buttonWidth = 60;
	public static int buttonHeight = 15;
	public static int margin = 10;

	// Estado actual de la GUI. Inicia en EDITING logicamente
	public static int currentState = STATE_EDITING;
	// Velocidad de ejecucion
	public static float currentRunningSpeed = .5f;
	// Camara actual
	public static int currentCamera = CAMARA_3DCAM;
	// Demora en segundos
	protected float waitDelaySeconds = .5f;

	// Codigo fuente
	protected string sourceCode = "Mover;\nMover;\nDerecha;\nMover;";
	// Contenido de la linea de estado
	protected string statusText = "Ready.";
	// Liena actual
	protected int currentLine = -1;
	// Conjunto de instrucciones
	protected ArrayList sentences = new ArrayList();
	// Codigo parseado
	protected bool codeParsed = false;
	// Error al interpretar linea
	protected bool angry = false;
	// Estado actual de la GUI bajo ejecucion STATE_RUNNING (pude estar activo o pausado).  Inicia en false logicamente.
	protected bool run = false;
	// Periodo durante la ejecucion de una instruccion
	protected bool step = false;
	// Animando la instruccion actual
	protected bool executingCurrentLine = false;
	// Robot fuera de los limites de la ciudad?
	protected bool alive = true;
	// Ya se ejecutaron todas las instrucciones del programa?
	protected bool ended = false;



	void OnGUI() { 
		switch (currentState) { 	
			case STATE_EDITING: {
				renderEditing();
				break;
			}
			case STATE_RUNNING: {
				renderRunning();
				break;
			}
			case STATE_CONFIG: {
				renderConfig();
				break;
			}
		}
	}

	/** Renders the Editing Menu */
	void renderEditing() {
		// Botonera principal
		int i = 0;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Open")) { 
			// TODO: Implementar
		}
		GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Save");	
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Run")) {
			parseCode();
			currentState = STATE_RUNNING;
			run = true;
			step = false;
			return;
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Settings")) {
			currentState = STATE_CONFIG;
			return;
		}
		GUI.Box (new Rect (margin + i * buttonWidth, margin, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight), ".:: EDITING ::.");

		// Visualizacion de codigo fuente
		sourceCode = GUI.TextArea(new Rect(margin, buttonHeight + 3 * margin, Screen.width - 2 * margin, Screen.height - 4 * margin - buttonHeight), sourceCode);
	}


	/** Renders the Running Menu */
	void renderRunning() {
		// Botonera principal
		int i = 0;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), run ? "Pause" : "Resume")) {
			run = !run;
			return;
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Stop")) {
			currentState = STATE_EDITING;
			return;
		}
		// Separador
		i++;
		// Camara
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Cam: " + currentCamera )) {
			currentCamera++;
			if (currentCamera >= CAMERA_END_OF)
				currentCamera = 0;
		}
		// Separador
		i++;
		// Velocidad
		if (GUI.Button (new Rect (margin + i * buttonWidth, margin, buttonWidth / 2, margin + buttonHeight), "-")) {
			currentRunningSpeed -= .1f;;
			if (currentRunningSpeed < 0)
				currentRunningSpeed = 0;
			return;
		}
		GUI.TextArea (new Rect (margin + i++ * buttonWidth + buttonWidth / 2, margin, buttonWidth, margin + buttonHeight), "Vel: " + Mathf.RoundToInt(currentRunningSpeed * 10));
		if (GUI.Button (new Rect (margin + i++ * buttonWidth + buttonWidth / 2, margin, buttonWidth / 2, margin + buttonHeight), "+")) {
			currentRunningSpeed += .1f;
			if (currentRunningSpeed > 1)
				currentRunningSpeed = 1;
			return;
		}
		// Linea de estado
		GUI.TextArea (new Rect (margin, Screen.height - 2 * margin - buttonHeight, Screen.width - 2 * margin, margin + buttonHeight), statusText);
	}


	/** Renders the Config Menu */
	void renderConfig() {
		// Botonera principal
		int i = 0;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Accept")) {
			currentState = STATE_EDITING;
			return;
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Cancel")) {
			currentState = STATE_EDITING;
			return;
		}
		GUI.Box (new Rect (margin + i * buttonWidth, margin, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight), ".:: CONFIGURATION ::.");
	}


	void parseCode() {
		// FIXME: Esto en realidad se debe delegar a la libreria
		sentences = new ArrayList ();
		string[] insts = sourceCode.Replace("\n", "").Replace(" ", "").Split(";"[0]);
		for (int i=0; i<insts.Length; i++)
			sentences.Add(insts[i].Trim());
		codeParsed = true;
		angry = false;
		currentLine = -1;
	}



	// Update is called once per frame
	void Update () {
		// Si no esta ejecutando, no hay nada mas que hacer
		if (currentState != STATE_RUNNING)
			return;

		// Si el codigo no fue parseado no puede continuar
		if (!codeParsed)
			return;

		// Ejecutar una instruccion unicamente si: 1) estamos en ejecucion, 2) si no se esta animando una instruccion previa, 3) si el robot sigue vivo
		if (run && !step && alive) {
			StartCoroutine(executeStep());
		}

	}

	/** Ejecucion de una instruccion */
	IEnumerator executeStep(){

		step = true;
		currentLine++;

		if (currentLine == sentences.Count-1) {
			statusText = "Finalizado. ";
			currentLine = -1;
			run = false;
			ended = true;
		}
		else if (sentences[currentLine] != null && sentences[currentLine].ToString().Length > 0) {
			executingCurrentLine = true;
			executeLine(currentLine);
		}

		// Mientras que este ejecutando, esperar
		while (executingCurrentLine) {
			yield return new WaitForSeconds(1 - waitDelaySeconds);
		}

		step = false;

	}

	/** Efectiviza la animacion de la instruccion */
	void executeLine(int lineNo) {

		// FIXME: Aqui deberia delegarse al robot a fin de que realice la animacion


		string status = "Executing line: " + (currentLine + 1) + ": " + sentences [lineNo];
		statusText = status;
		Debug.Log (status);


		executingCurrentLine = false;

	}


}
