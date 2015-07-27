using UnityEngine;
using System.Collections;

public class Corner : Object {

	/** Prefab de la flor */
	public static Object florPrefab;
	/** Prefab del papel  */
	public static Object papelPrefab;

	/** Posicion visual de la flor */
	public Vector3 flowerPos;
	/** Posicion visual del papel */
	public Vector3 paperPos;

	/** Referencia visual a la flor en la esquina (se oculta cuando flowers = 0) */
	public Object flowerInstance;
	/** Referencia visual al papel en la esquina (se oculta cuando papers = 0) */
	public Object paperInstance;

	
	/** Numero de flores en la esquina */
	public int flowers = 0;
	/** Numero de papeles en la esquina */
	public int papers = 0;


	/** Constructor */
	public Corner( Vector3 visualPosFlor, Vector3 visalPosPapel) {
		flowerPos = visualPosFlor;
		paperPos = visalPosPapel;
	}


	/** Actualiza el numero de papeles en la esquina. Cuando es 0 no visualiza objeto en pantalla */
	public void setPapers(int count) {
		papers = count;
		if (count > 0 && paperInstance == null) {
			paperInstance = Instantiate (papelPrefab, paperPos, Quaternion.identity);
			return;
		}
		if (count == 0 && paperInstance != null) {
			Destroy(((Transform)paperInstance).gameObject);
			paperInstance = null;
			return;
		}
	}

	/** Actualiza el numero de flores en la esquina. Cuando es 0 no visualiza objeto en pantalla */
	public void setFlowers(int count) {
		flowers = count;
		if (count > 0 && flowerInstance == null) {
			flowerInstance = Instantiate (florPrefab, flowerPos, Quaternion.identity);
			return;
		}
		if (count == 0 && flowerInstance != null) {
			Destroy(((Transform)flowerInstance).gameObject);
			flowerInstance = null;
			return;
		}
	}

	/** Incrementa en uno la cantidad de papeles */
	public void incPapers() {
		setPapers(papers+1);
	}


	/** Decrementa en uno la cantidad de papeles */
	public void decPapers() {
		setPapers(papers-1);
	}

	/** Incrementa en uno la cantidad de flores */
	public void incFlowers() {
		setFlowers(flowers+1);
	}
	
	
	/** Decrementa en uno la cantidad de flores */
	public void decFlowers() {
		setFlowers(flowers-1);
	}


}
