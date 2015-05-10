using UnityEngine;
using System.Collections;

public class Init : MonoBehaviour {

	// Valores por defecto
	public int CANT_CALLES = 100;
	public int CANT_AVENIDAS = 100;
	public static float ELEVACION_CIUDAD = -0.01f;
	public static float ELEVACION_CALLEAV = 0.01f;
	public static float ANCHO_CALLEAV = 0.25f;
	public static float DESP_CALLEAV = ANCHO_CALLEAV * 2;
	public static float ELEVACION_PAPEL = 0.015f;
	public static float ELEVACION_FLOR = 0.15f;

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

	// Use this for initialization
	void Start () {
		// Inicializar ciudad
		Object ciudadInstance = Instantiate (
				ciudadPrefab, 
				new Vector3 (CANT_AVENIDAS / 2 + DESP_CALLEAV, ELEVACION_CIUDAD, CANT_CALLES / 2 + DESP_CALLEAV ), 
				Quaternion.identity
			);
		((Transform)ciudadInstance).localScale = new Vector3 (CANT_AVENIDAS, 0.01f, CANT_CALLES);

		// Inicializar calles
		for (int z = 1; z <= CANT_CALLES; z++) {
			Object calleInstance = Instantiate(callePrefab, new Vector3(CANT_AVENIDAS/2 + DESP_CALLEAV, ELEVACION_CALLEAV, (float)z), Quaternion.identity);
			((Transform)calleInstance).localScale = new Vector3 (CANT_AVENIDAS - 1, ELEVACION_CALLEAV, ANCHO_CALLEAV);
		}

		// Inicializar avenidas
		for (int x = 1; x <= CANT_AVENIDAS; x++) {
			Object avenidaInstance = Instantiate(avenidaPrefab, new Vector3((float)x, ELEVACION_CALLEAV, CANT_CALLES/2 + DESP_CALLEAV), Quaternion.identity);
			((Transform)avenidaInstance).localScale = new Vector3 (ANCHO_CALLEAV, ELEVACION_CALLEAV, CANT_CALLES - 1);
		}

		// Inicializar papeles de manera aleatoria
		if (instanciarPapelesRandom) { 
			addRandomPrefab (papelPrefab, 1, ELEVACION_PAPEL);
		}

		// Inicializar flores de manera aleatoria
		if (instanciarFloresRandom) { 
			addRandomPrefab (florPrefab, 1, ELEVACION_FLOR);
		}

		// Inicializar robot
		Object robotInstance = Instantiate(robotPrefab, new Vector3(1.0f, ELEVACION_CALLEAV, 1.0f), Quaternion.identity);
	}

	/**
	 * Crea aleatoriamente a lo largo de la ciudad aPrefab object, 
	 * con una cantidad entre 0 y maxCount de instancias en cada esquina; con elevacionY sobre el nivel de la ciudad
	 */
	protected void addRandomPrefab(Object aPrefab, int maxCount, float elevacionY) { 
		for (int z = 1; z < CANT_CALLES; z++) {
			for (int x = 1; x < CANT_AVENIDAS; x++) {
				int count = Random.Range (0, maxCount+1);
				for (int c = 0; c < count; c++) {
					Instantiate (aPrefab, new Vector3 (x, elevacionY, z), Quaternion.identity);
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
