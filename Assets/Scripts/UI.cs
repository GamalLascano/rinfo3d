﻿using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
// using UnityEditor;
using System.IO;

public class UI : MonoBehaviour {

	// Referencia a object de creacion
	static GameObject bigBang = null;

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
	// Velocidad de ejecucion
	public static float currentRunningSpeed = .5f;
	// Zoom
	public static float zoom = 3f;
	// Idioma seleccionado por default
	private static string langSelected = I18N.getValue("lang_es");



	// Codigo fuente
	protected string sourceCode = "Iniciar(1,1);\nmover;\nDerecha;\nmover;\ntomarFlor;\ntomarPapel;\ndepositarFlor;\ndepositarFlor;\ndepositarPapel;\nPos(10,7);\nmover;\nDerecha;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;";
	// Contenido de la linea de estado del robot
	protected string statusRobot = "";
	// Contenido de la linea de estado de instruccion
	protected string statusText = I18N.getValue("ready");
	// Liena actual
	protected int currentLine = -1;
	// Conjunto de instrucciones
	protected ArrayList sentences = new ArrayList();
	// Codigo parseado
	protected bool codeParsed = false;
	// Error al interpretar linea
	protected bool angry = false;
	// Error en tiempo de ejecucion (null si no hay error)
	public static string runtimeErrorMsg = null;
	// Estado actual de la GUI bajo ejecucion STATE_RUNNING (pude estar activo o pausado).  Inicia en false logicamente.
	protected bool run = false;
	// Periodo durante la ejecucion de una instruccion
	protected bool step = false;
	// Animando la instruccion actual
	public static bool executingCurrentLine = false;
	// Robot fuera de los limites de la ciudad?
	protected bool alive = true;
	// Ya se ejecutaron todas las instrucciones del programa?
	protected bool ended = false;

	// Listado de Camaras
	protected ArrayList cameras  = null;
	// Camara superior
	public Camera cameraTop 	 = null;
	// Camara desde la esquina
	public Camera cameraAngle   = null;
	// Camara on-board
	Camera cameraOnBoard  = null;
	// Camara actual
	public static int currentCamera = 0;

	// Configurador: Flores
	protected string config_flower_av = "1";
	protected string config_flower_st =	"1";
	protected string config_flower_no = "0";

	// Configurador: Papeles
	protected string config_paper_av = "1";
	protected string config_paper_st = "1";
	protected string config_paper_no = "0";


	// Carga las camaras
	void loadCameras() {
		if (cameras == null) {
			cameras = new ArrayList();
			cameraOnBoard = (Camera)((Transform)Init.robotInstance).GetComponentInChildren<Camera>();
			cameras.Add(cameraTop);
			cameras.Add(cameraOnBoard);
			cameras.Add(cameraAngle);
			setCurrentCamera(currentCamera);
		}
	}

	// Realiza actividades en funcion del cambio de POSICION del robot (no de heading).  
	// Principalmente utilizado para sincronizar el movimiento de las camaras con el del 
	// robot dentro del mismo frame, a fin de evitar "saltos" mediante updates independientes.
	public static void robotMoved() {
		Camera cameraAngle = ((UI)getBigBang ().GetComponent<UI>()).cameraAngle;
		Camera cameraTop = ((UI)getBigBang().GetComponent<UI>()).cameraTop;
		Transform theRobot = (Transform)Init.robotInstance;

		// Camara 3D: mirar y seguir al robot
		cameraAngle.transform.LookAt (theRobot);
		cameraAngle.transform.position = new Vector3 (theRobot.position.x - 3, cameraAngle.transform.position.y, theRobot.position.z - 3);

		// Camara 2D: seguir al robot
		cameraTop.transform.position = new Vector3 (theRobot.position.x, cameraTop.transform.position.y, theRobot.position.z);
	}

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

		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("reset"))) { 
			Application.LoadLevel(Application.loadedLevel);
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("run"))) {
			parseCode();
			currentState = STATE_RUNNING;
			run = true;
			step = false;
			ended = false;
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("open"))) {
//			var path = EditorUtility.OpenFilePanel(I18N.getValue("open_file"), "", "txt");
//			if (path.Length != 0) {
//				Debug.Log ("Reading data from: " + path);
//				readCode(path);
//			}

		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("save"))) {
//			var path = EditorUtility.SaveFilePanel (I18N.getValue("save_file"), "", I18N.getValue("filename"), "txt");
//			if (path.Length != 0) {
//				Debug.Log ("Writing data to: " + path);
//				writeCode (path);
//			}
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("settings"))) {
			currentState = STATE_CONFIG;
			return;
		}
		GUI.Box (new Rect (margin + i * buttonWidth, margin, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight), I18N.getValue("edit_title"));

		// Visualizacion de codigo fuente
		sourceCode = GUI.TextArea(new Rect(margin, buttonHeight + 3 * margin, Screen.width - 2 * margin, Screen.height - 4 * margin - buttonHeight), sourceCode);
	}
	
	/** Renders the Running Menu */
	void renderRunning() {
		// Botonera principal
		int i = 0;
		if (!ended && GUI.Button (new Rect (margin + i * buttonWidth, margin, buttonWidth, margin + buttonHeight), run ? I18N.getValue("pause") : I18N.getValue("resume"))) {
			run = !run;
		}
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("stop"))) {
			currentState = STATE_EDITING;
		}
		// Camara
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth * 2, margin + buttonHeight), ((Camera)cameras[currentCamera]).name)) {
			currentCamera++;
			if (currentCamera >= cameras.Count)
				currentCamera = 0;
			setCurrentCamera(currentCamera);
		}
		i++;
		// Separador
		i++;
		// Linea de estado del robot
		GUI.TextArea (new Rect (margin + i++ * buttonWidth, margin, Screen.width - (2 * margin + (i-1) * buttonWidth), margin + buttonHeight * 2), Init.getRobotBehaviour().getRobotStatus());
		// Velocidad
		GUI.Label(new Rect (margin, Screen.height / 2 + buttonHeight * 4, buttonWidth, buttonHeight + margin), I18N.getValue("speed"));
		currentRunningSpeed = GUI.VerticalSlider( new Rect(margin, Screen.height / 2 - buttonHeight * 4, margin, buttonHeight*8), currentRunningSpeed, 1f, 0f);
		// Zoom
		GUI.Label(new Rect (Screen.width - buttonWidth / 2 - margin, Screen.height / 2 + buttonHeight * 4, buttonWidth, buttonHeight + margin), I18N.getValue("zoom"));
		zoom = GUI.VerticalSlider( new Rect(Screen.width - margin * 2, Screen.height / 2 - buttonHeight * 4, margin, buttonHeight*8), zoom, .5f, 10f);
		// Linea de ejecucion
		GUI.TextArea (new Rect (margin, Screen.height - 2 * margin - buttonHeight, Screen.width - 2 * margin, margin + buttonHeight), statusText);

		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();
	}


	/** Renders the Config Menu */
	void renderConfig() {
		int rowSpace = 2;
		int row = 0;
		// Botonera principal
		int i = 0;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("accept"))) {
			currentState = STATE_EDITING;
		}
		GUI.Box (new Rect (margin + i * buttonWidth, margin + buttonHeight * row, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight), I18N.getValue ("set_title"));

		// Nueva fila
		row += rowSpace;
		row += rowSpace;

		// ========================================= FLOWERS! =======================================
		i = 0;
		GUI.Box (new Rect (margin + i * buttonWidth, margin + buttonHeight * row, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight * (rowSpace * 2)), I18N.getValue ("set_flowers"));
		
		// Nueva fila
		row += rowSpace;
		
		// Configuracion de flores en esquina
		i = 1;
		GUI.Label (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("avenue"));
		config_flower_av = GUI.TextField (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), config_flower_av);
		i++;
		GUI.Label (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("street"));
		config_flower_st = GUI.TextField (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), config_flower_st);
		i++;
		GUI.Label (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("count"));
		config_flower_no = GUI.TextField (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), config_flower_no);
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth * 2, margin + buttonHeight), I18N.getValue ("set"))) {
			Init.city[int.Parse(config_flower_av)-1, int.Parse(config_flower_st)-1].flowers = int.Parse(config_flower_no);
		}

		// Nueva fila
		row += rowSpace;
		row += rowSpace;
		
		// ========================================= PAPERS! =======================================
		i = 0;
		GUI.Box (new Rect (margin + i * buttonWidth, margin + buttonHeight * row, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight * (rowSpace * 2)), I18N.getValue ("set_papers"));
		
		// Nueva fila
		row += rowSpace;
	
		// Configuracion de papeles en esquina
		i = 1;
		GUI.Label (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("avenue"));
		config_paper_av = GUI.TextField (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), config_paper_av);
		i++;
		GUI.Label (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("street"));
		config_paper_st = GUI.TextField (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), config_paper_st);
		i++;
		GUI.Label (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("count"));
		config_paper_no = GUI.TextField (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), config_paper_no);
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth * 2, margin + buttonHeight), I18N.getValue ("set"))) {
			Init.city[int.Parse(config_paper_av)-1, int.Parse(config_paper_st)-1].papers = int.Parse(config_paper_no);
		}

		// Nueva fila
		row += rowSpace;
		row += rowSpace;
		
		// ========================================= LANGUAGE! =======================================
		i = 0;
		GUI.Box (new Rect (margin + i * buttonWidth, margin + buttonHeight * row, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight * (rowSpace * 2)), I18N.getValue ("language"));
		
		// Nueva fila
		row += rowSpace;
		
		// Configuracion de idioma
		i = 1;
		GUI.Label (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth * 2, margin + buttonHeight), I18N.getValue ("lang_selected"));
		i++;
		langSelected = GUI.TextField (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), langSelected);
		i++;
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth * 2, margin + buttonHeight), I18N.getValue ("lang_en"))) {
			langSelected = I18N.getValue ("lang_en");
			I18N.setLang("en_US");
		}
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth * 2, margin + buttonHeight), I18N.getValue ("lang_es"))) {
			langSelected = I18N.getValue ("lang_es");
			I18N.setLang("es_AR");
		}
	}
	
	/** Actualiza la camara actual */
	void setCurrentCamera(int cameraNo) {
		for (int i = 0; i < cameras.Count; i++)
			((Camera)cameras[i]).enabled = false;
		((Camera)cameras[cameraNo]).enabled = true;
		setZoom();
	}

	/** Actualiza el zoom de la camara */
	void setZoom() {
		if (((Camera)cameras [currentCamera]).orthographic) {
			((Camera)cameras [currentCamera]).orthographicSize = zoom;
		} else {
			((Camera)cameras [currentCamera]).fieldOfView = zoom * 10;
		}
	}

	/** Parse de codigo */
	void parseCode() {
		// FIXME: Esto en realidad se debe delegar a la libreria
		sentences = new ArrayList ();
		string[] insts = sourceCode.Replace("\n", "").Replace(" ", "").Split(";"[0]);
		for (int i=0; i<insts.Length; i++)
			sentences.Add(insts[i].Trim());
		codeParsed = true;
		angry = false;
		runtimeErrorMsg = null;
		currentLine = -1;
	}
	

	// Update is called once per frame
	void Update () {

		// Configurar las camaras
		loadCameras();

		// Si no esta ejecutando, no hay nada mas que hacer
		if (currentState != STATE_RUNNING)
			return;

		// Si el codigo no fue parseado no puede continuar
		if (!codeParsed)
			return;

		// Setear zoom de la camara
		setZoom();

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
			statusText = I18N.getValue("finished");
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
			yield return new WaitForSeconds(1 - currentRunningSpeed);
		}

		step = false;

		// Hubo un error?
		if (runtimeErrorMsg != null) {
			angry = true;
			statusText = "Error ejecutando linea " + (currentLine+1) + ": " + sentences[currentLine] + ". " + runtimeErrorMsg;
			run = false;
			ended = true;
			currentLine = -1;
		}

	}

	/** Efectiviza la animacion de la instruccion */
	void executeLine(int lineNo) {

		// FIXME: Aqui deberia delegarse al robot a fin de que realice la animacion
		string status = I18N.getValue("exec_line") + (currentLine + 1) + ": " + sentences [lineNo];
		statusText = status;

		// Invocar ejecucion visual via reflection
		try {
			object result = null;
			// Recuperar el BigBang, y a partir de alli el Robot que se tenga configurado
			Transform theRobot = (Transform)Init.robotInstance;
			RobotBehaviour behaviour = (RobotBehaviour)theRobot.GetComponent<RobotBehaviour>();
			Type type = behaviour.GetType();

			// Pruebas para argumentos.  Esto igualmente se recibe desde libreria
			string sentence = (string)sentences[lineNo];
			string sentenceName = sentence.Substring(0, sentence.Contains("(") ? sentence.IndexOf("(") : sentence.Length);
			// Cargar los parametros segun la instruccion que sea.  FIXME: Deshardcode
			if (sentence.Contains("(")) { 
				behaviour.resetArguments();
				string sentenceArgs = sentence.Substring(sentence.IndexOf("(") , sentence.Length - sentence.IndexOf("("));
				sentenceArgs = sentenceArgs.Replace("(", "").Replace(")", "").Replace(" ", "");
				string[] args = sentenceArgs.Split(","[0]);
				for (int i=0; i < args.Length; i++)
					behaviour.addArgument(args[i]);
			}

			MethodInfo methodInfo = type.GetMethod(sentenceName);
			// ParameterInfo[] parameters = methodInfo.GetParameters();

			// Invocar a la corutina encargada de ejecutar la visualizacion
			behaviour.StartCoroutine(methodInfo.Name, 0);

		} catch (Exception e) {
			Debug.Log("Exception!! " + e.ToString());
			statusText = I18N.getValue("unknown_line") + (currentLine+1) + ": " + sentences[currentLine];

			run = false;
		}
	}

	/** Retorna la referencia al "creador" */
	public static GameObject getBigBang() {
		if (bigBang == null)
			bigBang = GameObject.Find("BigBang");
		return bigBang;
	}


	/** Lee el codigo (texto) desde un archivo */
	private bool readCode(string fileName)
	{
		try
		{
			string line;
			StreamReader theReader = new StreamReader(fileName);
			
			// "using" statement for potentially memory-intensive objects (instead of relying on garbage collection)
			using (theReader) {
				// Mientras haya lineas de texto
				line = theReader.ReadLine();
				sourceCode = "";
				while (line != null)
				{
					// Agrego la linea al panel de texto de la UI
					sourceCode = sourceCode + line + "\n";
					line = theReader.ReadLine();
				}
								
				theReader.Close();
				return true;
			}
		} catch (Exception e) {
			Debug.LogError("Exception!! " + e.ToString());
			return false;
		}
	}

	/** Escribe el codigo (texto) a un archivo */
	private bool writeCode(string fileName)
	{
		try
		{
			string line;
			StreamWriter theWriter = new StreamWriter(fileName);
			
			// "using" statement for potentially memory-intensive objects (instead of relying on garbage collection)
			using (theWriter) {
				// escribo el texto en el archivo
				string[] lines = sourceCode.Split('\n');
				for(int i=0; i < lines.Length; i++)
					theWriter.WriteLine(lines[i]);

				theWriter.Close();
				return true;
			}
		} catch (Exception e) {
			Debug.LogError("Exception!! " + e.ToString());
			return false;
		}
	}

}
