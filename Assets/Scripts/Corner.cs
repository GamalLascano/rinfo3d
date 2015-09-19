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
			paperInstance = Instantiate (papelPrefab, paperPos, Quaternion.Euler(90, 0, 0));
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
			flowerInstance = Instantiate (florPrefab, flowerPos, Quaternion.Euler(90, 0, 0));
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

	/** Setea el numero de papeles/flores para una esquina en particular. 
	 * 	Si se utiliza el cuantificador * sobre una avenida, se especificara la cantidad sobre todas las avenidas de esa calle
	 * 	Si se utiliza el cuantificador * sobre una calle, se especificara la cantidad sobre todas las calles de esa avenida
	 * 	Si se utiliza el cuantificador * sobre calle y avenida, se especificara la cantidad sobre todas las esquinas de la ciudad
	 */
	public static void setCorner(string config_av, string config_st, string config_no, bool setPapers) {
		int fromAv = 0;
		int toAv = 99;
		int fromSt = 0;
		int toSt = 99;


		if ("*" != config_av) {
			fromAv = int.Parse(config_av) - 1;
			toAv = int.Parse(config_av) - 1;
		}
		if ("*" != config_st) {
			fromSt = int.Parse(config_st) - 1;
			toSt = int.Parse(config_st) - 1;
		}
		int no = int.Parse(config_no);

		for (int currAv = fromAv; currAv <= toAv; currAv++) {
			for (int currSt = fromSt; currSt <= toSt; currSt++) {
				if (setPapers)
					Init.city[currAv, currSt].setPapers(no);
				else
					Init.city[currAv, currSt].setFlowers(no);
			}
		}
	}
		

}
