using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Init : MonoBehaviour {

	// Valores por defecto
	public int CANT_CALLES = 100;
	public int CANT_AVENIDAS = 100;
	public static float ELEVACION_CIUDAD = -0.01f;
	public static float ELEVACION_CALLEAV = 0.015f;
	public static float ANCHO_CALLEAV = 0.25f;
	public static float DESP_CALLEAV = ANCHO_CALLEAV * 2;
	public static float ELEVACION_PAPEL = 0.02f;
	public static float ELEVACION_FLOR =  0.02f;
	public static float DESP_PAPEL = -0.075f;
	public static float DESP_FLOR =  0.075f;


	// Prefab ciudad, calles y avenidas para inicializar ciudad
	public Transform ciudadPrefab;
	public Transform callePrefab; 
	public Transform avenidaPrefab; 
	public Transform robotPrefab; 
	public Transform papelPrefab;
	public Transform florPrefab;

	// Preferencias
	public bool instanciarFloresRandom = false;
	public bool instanciarPapelesRandom = false;

	/** Referencia al robot */
	public static Object robotInstance = null;

	/** Ciudad modelo (array multidimensional de esquinas) */ 
	public static Corner[,] city;

	/** Retorna el comportamiento del robot */
	public static RobotBehaviour getRobotBehaviour() {
		Transform theRobot = (Transform)Init.robotInstance;
		return (RobotBehaviour)theRobot.GetComponent<RobotBehaviour>();
	}
    // List of Streets Generated
    private static List<Object> streetPrefabsGenerated = new List<Object>();
    // List of Variables the program has
    public class VariableR {
        public string nombre;
        public string tipo;
        public VariableR(string nomVar,string tipoVar)
        {
            nombre = nomVar;
            tipo = tipoVar;
        }
    }
    public static List<VariableR> Variables = new List<VariableR>();
    // Use this for initialization
    void Start () {
        createCity(100,100);
		// Inicializar robot
		robotInstance = Instantiate(robotPrefab, new Vector3(1.0f, ELEVACION_CALLEAV, 1.0f), Quaternion.identity);
        //Inicializar papeles y flores de forma que corra siempre el codigo ejemplo
        Corner.setCorner("1", "1", "1", false);
        Corner.setCorner("1", "1", "1", true);
    }
    public void createCity(int cantAv,int cantCa)
    {
        // Inicializar ciudad (visual)
        CANT_AVENIDAS = cantAv;
        CANT_CALLES = cantCa;
        Object ciudadInstance = Instantiate(
                ciudadPrefab,
                new Vector3(CANT_AVENIDAS / 2 + DESP_CALLEAV, ELEVACION_CIUDAD, CANT_CALLES / 2 + DESP_CALLEAV),
                Quaternion.identity
            );
        streetPrefabsGenerated.Add(((Transform)ciudadInstance).gameObject);
        ((Transform)ciudadInstance).localScale = new Vector3(CANT_AVENIDAS, 0.01f, CANT_CALLES);

        // Inicializar ciudad 
        Corner.florPrefab = florPrefab;
        Corner.papelPrefab = papelPrefab;
        city = new Corner[CANT_AVENIDAS, CANT_CALLES];
        for (int av = 0; av < CANT_AVENIDAS; av++)
            for (int ca = 0; ca < CANT_CALLES; ca++)
            {
                city[av, ca] = new Corner(new Vector3(1 + av + DESP_FLOR, ELEVACION_FLOR, 1 + ca), new Vector3(1 + av + DESP_PAPEL, ELEVACION_PAPEL, 1 + ca));
            }
                

        // Inicializar calles
        for (int z = 1; z <= CANT_CALLES; z++)
        {
            Object calleInstance = Instantiate(callePrefab, new Vector3(CANT_AVENIDAS / 2 + DESP_CALLEAV, ELEVACION_CALLEAV, (float)z), Quaternion.Euler(new Vector3(90, 0, 0)));
            ((Transform)calleInstance).localScale = new Vector3(CANT_AVENIDAS - .75f, ANCHO_CALLEAV, 1);
            streetPrefabsGenerated.Add(((Transform)calleInstance).gameObject);
        }

        // Inicializar avenidas
        for (int x = 1; x <= CANT_AVENIDAS; x++)
        {
            Object avenidaInstance = Instantiate(avenidaPrefab, new Vector3((float)x, ELEVACION_CALLEAV, CANT_CALLES / 2 + DESP_CALLEAV), Quaternion.Euler(new Vector3(90, 0, 0)));
            ((Transform)avenidaInstance).localScale = new Vector3(ANCHO_CALLEAV, CANT_CALLES - .75f, 1);
            streetPrefabsGenerated.Add(((Transform)avenidaInstance).gameObject);
        }

        // Inicializar papeles de manera aleatoria
        if (instanciarPapelesRandom)
        {
            addRandomPrefab(papelPrefab, true, 1, ELEVACION_PAPEL, DESP_PAPEL, city, 3);
        }

        // Inicializar flores de manera aleatoria
        if (instanciarFloresRandom)
        {
            addRandomPrefab(florPrefab, false, 1, ELEVACION_FLOR, DESP_FLOR, city, 3);
        }

    }
    public static void resetCity(int cantAv,int cantCa)
    {
        for (int z = 1; z < UI.getBigBang().GetComponent<Init>().CANT_CALLES; z ++)
        {
            for (int x = 1; x < UI.getBigBang().GetComponent<Init>().CANT_AVENIDAS; x ++)
            {
                    city[x - 1, z - 1].setPapers(0);
                    city[x - 1, z - 1].setFlowers(0);
            }
        }
        foreach (var Obje in city)
        {
            Destroy(Obje);
        }
        city = new Corner[50, 50];
        foreach (var Obj in streetPrefabsGenerated)
        {
            Destroy(Obj);
        }
        UI.getBigBang().GetComponent<Init>().createCity(cantAv, cantCa);
    }
    /**
	 * Crea aleatoriamente a lo largo de la ciudad aPrefab object, 
	 * con una cantidad entre 0 y maxCount de instancias en cada esquina; con elevacionY sobre el nivel de la ciudad
	 */
    protected void addRandomPrefab(Object aPrefab, bool isPapel, int maxCount, float elevacionY, float despX, Corner[,] city, int cornerStep) { 
		for (int z = 1; z < CANT_CALLES; z+=cornerStep) {
			for (int x = 1; x < CANT_AVENIDAS; x+=cornerStep) {

				// Determinar aleatoriamente el numero de flores/papeles
				int count = Mathf.FloorToInt(Random.Range (0, maxCount+1));

				// Asignar el numero de flores/papels a la esquina de la ciudad
				if (isPapel) {
					city[x-1,z-1].setPapers(count);
				} else {
					city[x-1,z-1].setFlowers(count);
				}
			}
		}
	}


}
