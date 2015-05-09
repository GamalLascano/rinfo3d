using UnityEngine;
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

	// Estado actual de la GUI bajo ejecucion STATE_RUNNING (bajo RUNNIG pude estar activo o pausado).  Inicia en false logicamente
	public static bool currentRunningState = false;
	// Velocidad de ejecucion
	public static float currentRunningSpeed = .5f;
	// Camara actual
	public static int currentCamera = CAMARA_3DCAM;

	// Codigo a visualizar y ejecutar
	protected string sourceCode = "SOURCE CODE HERE";

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
			currentState = STATE_RUNNING;
			currentRunningState = true;
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
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), currentRunningState ? "Pause" : "Resume")) {
			currentRunningState = !currentRunningState;
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

		GUI.TextArea (new Rect (margin, Screen.height - 2 * margin - buttonHeight, Screen.width - 2 * margin, margin + buttonHeight), "Ready.");
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

}
