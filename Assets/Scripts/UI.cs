using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
// using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
public class UI : MonoBehaviour {

	// Referencia a object de creacion
	static GameObject bigBang = null;

	// Posibles estados de la GUI
	public const int STATE_EDITING = 0;
	public const int STATE_RUNNING = 1;
	public const int STATE_CONFIG  = 2;
    public const int STATE_VR = 3;
    public const int STATE_RUNNING_VR = 4;
    //  Booleano para iniciar VR
    public static bool vrmod = false;
    // Tamaño de pantalla del dispositivo actual
    private static int deviceWidth =  Screen.width;
	private static int deviceHeight = Screen.height;
	
	// Tamaño default de los botones 
	public static int buttonWidth = deviceWidth/8; //100
	public static int buttonHeight = deviceHeight/20; //20
	public static int margin = deviceHeight/40; //10

	// Estilo de los componentes
	GUIStyle styleButton, styleTextArea;
	Color textButtonColor, textTextAreaColor;
	
	// Estado actual de la GUI. Inicia en EDITING logicamente
	public static int currentState = STATE_EDITING;
	// Velocidad de ejecucion
	public static float currentRunningSpeed = .5f;
	// Zoom
	public static float zoom = 3f;
	// Pan
	public static float pan = 0f;
	// Idioma seleccionado por default
	private static string langSelected = I18N.getValue("lang_es");

	// Informar Message
	public static string informarMessage = null;

	// Skin default a utilizar 
	public GUISkin customSkin;

   // private int newrun=1;
	// Codigo fuente
	protected string sourceCode = "Iniciar(1,1);\ntomarFlor;\ntomarPapel;\nmover;\ndepositarFlor;\ndepositarPapel;\nmover;\nmover;\nmover;\nmover;\nDerecha;\nmover;\nmover;\nmover;\nmover;\nPos(20,30);\nDerecha;\nDerecha;\nInformar(\"Listo!\");"; //"Iniciar(1,1);\nmover;\nmover;\nDerecha;\nmover;\nmover;\nInformar(\"Hola\");Pos(20,20);\ntomarFlor;\ntomarPapel;\nPos(11,11);\ndepositarFlor;\ndepositarPapel;\nmover;\ndepositarPapel;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;\nDerecha;\nmover;";
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
    // Camara VR
    public Camera cameraVR = null;
	// Camara actual
	public static int currentCamera = 2;

	// Configurador: Flores
	protected string config_flower_av = "1";
	protected string config_flower_st =	"1";
	protected string config_flower_no = "0";

	// Configurador: Papeles
	protected string config_paper_av = "1";
	protected string config_paper_st = "1";
	protected string config_paper_no = "0";

	// Configurador: Bolsa
	protected string config_bag_flowers = "0";
	protected string config_bag_papers =  "0";

	// Estilo de mensaje (ejecucion correcta o con error)
	static GUIStyle styleKO = null; 
	static GUIStyle styleOK = null;

	public static int CAMERA_TOP = 0;
	public static int CAMERA_ONBOARD = 1;
	public static int CAMERA_3D = 2;
    public static int CAMERA_VR = 3;

    //Menus en VR
    public static UnityEngine.Object menuEndInstance = null;
    public Transform menuEndPrefab;
    private bool flagMenu = false;
    // Carga las camaras
    void loadCameras() {
		if (cameras == null) {
			cameras = new ArrayList();
			cameraOnBoard = (Camera)((Transform)Init.robotInstance).GetComponentInChildren<Camera>();
			cameras.Add(cameraTop);
			cameras.Add(cameraOnBoard);
			cameras.Add(cameraAngle);
            cameras.Add(cameraVR);
			setCurrentCamera(currentCamera);

		}
	}

    public static void desactivarVR()
    {
        GameObject.FindGameObjectWithTag("MenuEnd").SetActive(false);
        vrmod = false;
        currentState = STATE_EDITING;
        XRSettings.enabled = false;
        
    }

    void OnGUI() { 
		GUI.skin = customSkin;
		GUI.skin.verticalScrollbar.fixedWidth = deviceWidth/45;
		GUI.skin.verticalScrollbarThumb.fixedWidth = deviceWidth/45;
		GUI.skin.horizontalScrollbar.fixedHeight = GUI.skin.verticalScrollbar.fixedWidth;
		GUI.skin.horizontalScrollbarThumb.fixedHeight = GUI.skin.verticalScrollbarThumb.fixedWidth;

		// Estilo de los botones
		textButtonColor = new Color(0.75F, 0.75F, 1.0F, 1);
		textTextAreaColor = new Color(0.75F, 1.0F, 0.75F, 1);

		styleButton = new GUIStyle("button");
		styleButton.normal.textColor = textButtonColor;
		styleButton.fontSize = deviceHeight/25;

		styleTextArea = new GUIStyle("textArea");
		styleTextArea.normal.textColor = textTextAreaColor;
		styleTextArea.fontSize = deviceHeight/30;

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
            case STATE_VR: {
                renderVR();
                break;
            }
            case STATE_RUNNING_VR: {
                    renderVRRUN();
                    break;
                }
		}
	}

	/** Renders the Editing Menu */
	void renderEditing() {
        if (vrmod == false) {
            XRSettings.enabled = false;
            vrmod = true;
            setCurrentCamera(2);
            GvrPointerInputModule.Pointer.overridePointerCamera = ((Camera)cameras[2]);
        }
        // Botonera principal
        int i = 0;

		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("reset"), styleButton)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("run"), styleButton)) {
			parseCode();
			currentState = STATE_RUNNING;
			run = true;
			step = false;
			ended = false;
		}
        if (GUI.Button(new Rect(margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("VR"), styleButton)) {
            vrmod = false;
            GameObject.FindGameObjectWithTag("Menu").SetActive(true);
            currentState = STATE_VR;
        }

        //		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("open"), styleButton)) {
        //			var path = EditorUtility.OpenFilePanel(I18N.getValue("open_file"), "", "txt");
        //			if (path.Length != 0) {
        //				Debug.Log ("Reading data from: " + path);
        //				readCode(path);
        //			}

        //		}
        //		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("save"), styleButton)) {
        //			var path = EditorUtility.SaveFilePanel (I18N.getValue("save_file"), "", I18N.getValue("filename"), "txt");
        //			if (path.Length != 0) {
        //				Debug.Log ("Writing data to: " + path);
        //				writeCode (path);
        //			}
        //		}
        if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("settings"), styleButton)) {
			currentState = STATE_CONFIG;
			return;
		}

		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue ("quit"), styleButton)) {
			Application.Quit();
		}

		GUI.Box (new Rect (margin + i * buttonWidth, margin, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight), I18N.getValue("edit_title"), styleButton);

		// Visualizacion de codigo fuente
		sourceCode = GUI.TextArea(new Rect(margin, buttonHeight + 3 * margin, Screen.width - 2 * margin, Screen.height - 4 * margin - buttonHeight), sourceCode, styleTextArea);
	}
    public void runInVR()
    {
        GameObject.FindGameObjectWithTag("Menu").SetActive(false);
        parseCode();
        currentState = STATE_RUNNING_VR;
        run = true;
        step = false;
        ended = false;
        cameraVR.transform.parent.gameObject.transform.position = cameraOnBoard.transform.position + new Vector3(0f,0.5f,-0.5f);
        //Transform robot = (Transform)Init.robotInstance;
        //cameraVR.transform.parent.gameObject.transform.position = new Vector3(robot.position.x, robot.position.y + 2f, robot.position.z - 2f);
        // Camera.current.transform.LookAt((Transform)Init.robotInstance);

    }
    public void desactivarVR2()
    {
        //setCurrentCamera(2);
        //GvrPointerInputModule.Pointer.overridePointerCamera = ((Camera)cameras[1]);
        vrmod = false;
        currentState = STATE_EDITING;
    }
    void renderVRRUN()
    {
        cameraVR.transform.parent.gameObject.transform.position = cameraOnBoard.transform.position + new Vector3(0f,0.5f,-0.5f);
        if (ended == true)
        {
            if (flagMenu == false)
            {
                VrGazebo.RestartClock();
                Debug.Log("Entre aca una vez");
                flagMenu = true;
                if (menuEndInstance == null)
                {
                    menuEndInstance = Instantiate(menuEndPrefab, cameraOnBoard.transform.position + new Vector3(0f, 0.5f, 4f), Quaternion.identity);
                }
                else
                {
                    GameObject.FindGameObjectWithTag("MenuEnd").SetActive(true);
                    Transform cambiarPos = (Transform)menuEndInstance;
                    cambiarPos.position = cameraOnBoard.transform.position + new Vector3(0f, 0.5f, -0.5f);
                }
            }
        }
        //cameraVR.transform.parent.gameObject.transform.rotation = cameraOnBoard.transform.rotation;
        //    //if (newrun == 1)
        //    //{
        //    //    parseCode();
        //    //    run = true;
        //    //    step = false;
        //    //    ended = false;
        //    //    Camera.current.transform.LookAt((Transform)Init.robotInstance);
        //    //    newrun = 0;
        //    //}

    }
    void renderVR()
    {
        if (vrmod == false)
        {
            setCurrentCamera(3);
            GvrPointerInputModule.Pointer.overridePointerCamera = ((Camera)cameras[3]);
            
            XRSettings.enabled = true;
            
            vrmod = true;
        }

    }
	/** Renders the Running Menu */
	void renderRunning() {
		// Botonera principal
		int i = 0;
		if (!ended && GUI.Button (new Rect (margin + i * buttonWidth, margin, buttonWidth, margin + buttonHeight), run ? I18N.getValue("pause") : I18N.getValue("resume"), styleButton) && informarMessage == null) {
			run = !run;
		}
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth, margin + buttonHeight), I18N.getValue("stop"), styleButton) && informarMessage == null) {
			currentState = STATE_EDITING;
		}
		// Camara
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin, buttonWidth , margin + buttonHeight), ((Camera)cameras[currentCamera]).name, styleButton)) {
			currentCamera++;
			if (currentCamera >= cameras.Count)
				currentCamera = 0;
			setCurrentCamera(currentCamera);
		}
		// Linea de estado del robot
		GUI.TextArea (new Rect (margin + i++ * buttonWidth, margin, Screen.width - (2 * margin + (i-1) * buttonWidth), margin + buttonHeight * 2), Init.getRobotBehaviour().getRobotStatus(), styleTextArea);
		// Velocidad
		GUI.Label(new Rect (margin, Screen.height / 2 + buttonHeight * 4, buttonWidth * 2, buttonHeight + margin), I18N.getValue("speed"));
		currentRunningSpeed = GUI.VerticalScrollbar( new Rect(margin, Screen.height / 2 - buttonHeight * 4, margin, buttonHeight*8), currentRunningSpeed, .1f, 1f, 0f);
		// Zoom
		GUI.Label(new Rect (Screen.width - buttonWidth / 2 - margin, Screen.height / 2 + buttonHeight * 4, buttonWidth, buttonHeight + margin), I18N.getValue("zoom"));
		zoom = GUI.VerticalScrollbar( new Rect(Screen.width - margin * 2, Screen.height / 2 - buttonHeight * 4, margin, buttonHeight*8), zoom, 1f, .5f, 10f);
		// Paneo
		GUI.Label(new Rect (Screen.width / 2 - buttonWidth - margin * 6, Screen.height - margin * 4 - buttonHeight * 2, buttonWidth, buttonHeight + margin), I18N.getValue("pan"));
		pan = GUI.HorizontalScrollbar( new Rect(Screen.width / 2 - buttonWidth, Screen.height - margin * 3 - buttonHeight * 2, buttonWidth * 2, margin), pan, 3f, -15f, 15f);


		// Linea de ejecucion
		if (styleOK == null) {
			// Textura para error
			Texture2D aTexture = new Texture2D(1, 1);
			aTexture.SetPixel(0, 0, Color.white);
			aTexture.wrapMode = TextureWrapMode.Repeat;
			aTexture.Apply();

			// Mensajes OK y ERROR
			styleOK = new GUIStyle(GUI.skin.textArea);
			styleKO = new GUIStyle(GUI.skin.textArea);

			// Colores de Estilos OK y ERROR
			styleOK.normal.textColor = Color.white;
			styleOK.fontSize = styleTextArea.fontSize;
			styleKO.normal.textColor = Color.red;
			styleKO.fontSize = styleTextArea.fontSize;
			styleKO.normal.background = aTexture;
		}
		GUI.TextArea (new Rect (margin, Screen.height - 2 * margin - buttonHeight, Screen.width - 2 * margin, margin + buttonHeight), statusText, getStatusStyle());

		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();
		
		// Informar dialog
		if (informarMessage != null) {
			GUI.TextArea(new Rect (margin * 4, Screen.height / 2 - buttonHeight * 2, Screen.width - 8 * margin, buttonHeight * 4), informarMessage, styleTextArea);
			if (GUI.Button (new Rect (margin * 4, Screen.height / 2 + buttonHeight * 2, Screen.width - 8 * margin, buttonHeight * 2), "OK", styleTextArea)) {
				informarMessage = null;
			}
		}
	}


	/** Renders the Config Menu */
	void renderConfig() {
		float rowSpace = 1.5f;
		float row = 0f;
		// Botonera principal
		int i = 0;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("accept"), styleButton)) {
			currentState = STATE_EDITING;
		}
		GUI.Box (new Rect (margin + i * buttonWidth, margin + buttonHeight * row, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight), I18N.getValue ("set_title"), styleButton);

		// Nueva fila
		row += rowSpace;

		// ========================================= FLOWERS! =======================================
		i = 0;
		GUI.Box (new Rect (margin + i * buttonWidth, margin + buttonHeight * row, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight * (rowSpace * 2)), I18N.getValue ("set_flowers") + I18N.getValue("wildcard"));
		
		// Nueva fila
		row += rowSpace;
		
		// Configuracion de flores en esquina
		i = 1;
		GUI.Label (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("avenue"));
		config_flower_av = GUI.TextField (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth / 2, margin + buttonHeight), config_flower_av);
		i++;
		GUI.Label (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("street"));
		config_flower_st = GUI.TextField (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth / 2, margin + buttonHeight), config_flower_st);
		i++;
		GUI.Label (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("count"));
		config_flower_no = GUI.TextField (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth / 2, margin + buttonHeight), config_flower_no);
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("set"))) {
			Corner.setCorner(config_flower_av, config_flower_st, config_flower_no, false);
		}

		// Nueva fila
		row += rowSpace;
		row += rowSpace;

		// ========================================= PAPERS! =======================================
		i = 0;
		GUI.Box (new Rect (margin + i * buttonWidth, margin + buttonHeight * row, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight * (rowSpace * 2)), I18N.getValue ("set_papers")  + I18N.getValue("wildcard"));
		
		// Nueva fila
		row += rowSpace;
	
		// Configuracion de papeles en esquina
		i = 1;
		GUI.Label (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("avenue"));
		config_paper_av = GUI.TextField (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth / 2, margin + buttonHeight), config_paper_av);
		i++;
		GUI.Label (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("street"));
		config_paper_st = GUI.TextField (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth / 2, margin + buttonHeight), config_paper_st);
		i++;
		GUI.Label (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("count"));
		config_paper_no = GUI.TextField (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth / 2, margin + buttonHeight), config_paper_no);
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("set"))) {
			Corner.setCorner(config_paper_av, config_paper_st, config_paper_no, true);
		}

		// Nueva fila
		row += rowSpace;
		row += rowSpace;

		// ========================================= BAG! =======================================
		i = 0;
		GUI.Box (new Rect (margin + i * buttonWidth, margin + buttonHeight * row, Screen.width - (2 * margin + i++ * buttonWidth), margin + buttonHeight * (rowSpace * 2)), I18N.getValue ("inthebag"));
		
		// Nueva fila
		row += rowSpace;
		
		// Configuracion de papeles en esquina
		i = 1;
		GUI.Label (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("flowers_bag"));
		config_bag_flowers = GUI.TextField (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth / 2, margin + buttonHeight), config_bag_flowers);
		i++;
		GUI.Label (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("papers_bag"));
		config_bag_papers = GUI.TextField (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth / 2, margin + buttonHeight), config_bag_papers);
		i++;
		i++;
		i++;
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("set"))) {
			Init.getRobotBehaviour().flores = int.Parse(config_bag_flowers);
			Init.getRobotBehaviour().papeles = int.Parse(config_bag_papers); 
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
		GUI.Label (new Rect (margin + i++ * buttonWidth / 2, margin + buttonHeight * row, buttonWidth * 2, margin + buttonHeight), I18N.getValue ("lang_selected"));
		langSelected = GUI.TextField (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), langSelected);
		i++;
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("lang_en"))) {
			langSelected = I18N.getValue ("lang_en");
			I18N.setLang("en_US");
		}
		if (GUI.Button (new Rect (margin + i++ * buttonWidth, margin + buttonHeight * row, buttonWidth, margin + buttonHeight), I18N.getValue ("lang_es"))) {
			langSelected = I18N.getValue ("lang_es");
			I18N.setLang("es_AR");
		}
	}


	/** Retorna el estilo para el mensaje de status en funcion de la presencia o ausencia de un error */
	public static GUIStyle getStatusStyle() { 
		return (runtimeErrorMsg == null ? styleOK : styleKO);
	}


	/** Actualiza la camara actual */
	void setCurrentCamera(int cameraNo) {
		pan = 0;
		for (int i = 0; i < cameras.Count; i++) { 
			((Camera)cameras[i]).enabled = false;
			((AudioListener)((Camera)cameras[i]).GetComponent<AudioListener>()).enabled = false;
		}
		((Camera)cameras[cameraNo]).enabled = true;
		((AudioListener)((Camera)cameras[cameraNo]).GetComponent<AudioListener>()).enabled = true;
		setZoom();
	}

	/** Actualiza el zoom de la camara */
	void setZoom() {
		if (((Camera)cameras [currentCamera]).orthographic) {
			((Camera)cameras [currentCamera]).orthographicSize = zoom;
		} else {
			((Camera)cameras [currentCamera]).fieldOfView = zoom * 5;
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
        {
            if (currentState != STATE_RUNNING_VR)
            {
                return;
            }
        }

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
			// Invocar a la corutina encargada de ejecutar la visualizacion
			Transform theRobot = (Transform)Init.robotInstance;
			RobotBehaviour behaviour = (RobotBehaviour)theRobot.GetComponent<RobotBehaviour>();
			behaviour.StartCoroutine("finalizar", 0);
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
