using UnityEngine;
using System.Collections;
using System.Reflection;
using System; 

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
	// Demora en segundos
	protected float waitDelaySeconds = .5f;

	// Codigo fuente
	protected string sourceCode = "mover;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;";
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
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Save")) { 
			// TODO: Implementar
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Run")) {
			parseCode();
			currentState = STATE_RUNNING;
			run = true;
			step = false;
			ended = false;
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Reset")) { 
			Application.LoadLevel(Application.loadedLevel);
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
		if (!ended && GUI.Button (new Rect (margin + i * buttonWidth, margin, buttonWidth, margin + buttonHeight), run ? "Pause" : "Resume")) {
			run = !run;
		}
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Stop")) {
			currentState = STATE_EDITING;
		}
		// Separador
		i++;
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
		// Velocidad
		if (GUI.Button (new Rect (margin + i * buttonWidth, margin, buttonWidth / 2, margin + buttonHeight), "-")) {
			currentRunningSpeed -= .1f;;
			if (currentRunningSpeed < 0)
				currentRunningSpeed = 0;
		}
		GUI.TextArea (new Rect (margin + i++ * buttonWidth + buttonWidth / 2, margin, buttonWidth, margin + buttonHeight), "Vel: " + Mathf.RoundToInt(currentRunningSpeed * 10));
		if (GUI.Button (new Rect (margin + i++ * buttonWidth + buttonWidth / 2, margin, buttonWidth / 2, margin + buttonHeight), "+")) {
			currentRunningSpeed += .1f;
			if (currentRunningSpeed > 1)
				currentRunningSpeed = 1;
		}
		// Linea de estado
		GUI.TextArea (new Rect (margin, Screen.height - 2 * margin - buttonHeight, Screen.width - 2 * margin, margin + buttonHeight), statusText);

		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();
	}


	/** Renders the Config Menu */
	void renderConfig() {
		// Botonera principal
		int i = 0;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Accept")) {
			currentState = STATE_EDITING;
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), "Cancel")) {
			currentState = STATE_EDITING;
		}
		GUI.Box (new Rect (margin + i * buttonWidth, margin, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight), ".:: CONFIGURATION ::.");
	}


	/** Actualiza la camara actual */
	void setCurrentCamera(int cameraNo) {
		for (int i = 0; i < cameras.Count; i++)
			((Camera)cameras[i]).enabled = false;
		((Camera)cameras[cameraNo]).enabled = true;
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
			statusText = "Finished. ";
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

		// Invocar ejecucion visual via reflection
		try {
			object result = null;
			// Recuperar el BigBang, y a partir de alli el Robot que se tenga configurado
			Transform theRobot = (Transform)Init.robotInstance;
			RobotBehaviour behaviour = (RobotBehaviour)theRobot.GetComponent<RobotBehaviour>();
			Type type = behaviour.GetType();
			MethodInfo methodInfo = type.GetMethod((string)sentences[lineNo]);
			ParameterInfo[] parameters = methodInfo.GetParameters();
			//object classInstance = Activator.CreateInstance(type, null);
			if (parameters.Length == 0)
			{
				// Caso general de visualizacion: mover, derecha, etc.  LIMITACION: NO puede recibir argumentos adicionales
				behaviour.StartCoroutine(methodInfo.Name, 0);
			}
			else
			{
				// Caso INFORMAR, en donde se requeiere un parametro adicional. 
				object[] parametersArray = new object[] { "Hola!!" };  // TODO: deshardcode
				result = methodInfo.Invoke(behaviour, parametersArray);
			}
		} catch (Exception) {
			statusText = "Unknown instruction at line " + (currentLine+1) + ": " + sentences[currentLine];
			run = false;
		}
	}

	/** Retorna la referencia al "creador" */
	public static GameObject getBigBang() {
		if (bigBang == null)
			bigBang = GameObject.Find("BigBang");
		return bigBang;
	}
	

}
