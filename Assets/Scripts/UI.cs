using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

	// Posibles estados de la GUI
	public const int STATE_EDITING = 0;
	public const int STATE_RUNNING = 1;
	public const int STATE_CONFIG  = 2;

	// Tamaño default de los botones 
	public static int buttonWidth = 60;
	public static int buttonHeight = 15;
	public static int margin = 10;

	// Estado actual de la GUI. Inicia en EDITING logicamente
	public static int currentState = STATE_EDITING;

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
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Pause")) {
			currentState = STATE_EDITING;
			return;
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Resume")) {
			currentState = STATE_EDITING;
			return;
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Stop")) {
			currentState = STATE_EDITING;
			return;
		}
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
