using UnityEngine;
using System.Collections;

public class SimpleRobotBehaviour : RobotBehaviour {

	/**
	 * Metodo a implementar
	 */
	public override IEnumerator mover()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando mover!");
		return base.mover();
	}


	/**
	 * Metodo a implementar
	 */
	public override IEnumerator Derecha()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando derecha!");
		return base.Derecha();
	}	


	/**
	 * Metodo a implementar
	 */
	public override IEnumerator Informar() {
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando informar!");
		return base.Informar ();
	}


	/**
	 * Metodo a implementar
	 */
	public override IEnumerator Pos() {
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando pos!");
		return base.Pos ();
	}

	/**
	 * Metodo a implementar
	 */
	public override IEnumerator Iniciar() {
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando iniciar!");
		return base.Iniciar ();
	}

	/**
	 * Metodo a implementar
	 */ 
	public override IEnumerator tomarFlor() {
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando tomarFlor!");
		return base.tomarFlor ();		
	}
	
	/**
	 * Metodo a implementar
	 */ 
	public override IEnumerator depositarFlor() {
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando depositarFlor!");
		return base.depositarFlor ();		
	}
	
	/**
	 * Metodo a implementar
	 */ 
	public override IEnumerator tomarPapel() {
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando tomarPapel!");
		return base.tomarPapel ();		
	}
	
	/**
	 * Metodo a implementar
	 */ 
	public override IEnumerator depositarPapel() {
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando depositarPapel!");
		return base.depositarPapel ();		
	}
	
	/**
 	* Metodo de estructura de datos a implementar
 	*/
    	public override IEnumerator repetir() {
        	Debug.Log("Soy SimpleRobotBehaviour y estoy ejecutando la estructura de datos repetir!");
        	return base.repetir();
    	}

	/**
	 * Metodo a implementar
	 */ 
    public override IEnumerator comenzar()
    {
        Debug.Log("Soy SimpleRobotBehaviour y estoy ejecutando comenzar!");
        return base.comenzar();
    }
    public override IEnumerator programa()
    {
        Debug.Log("Soy SimpleRobotBehaviour y estoy ejecutando programa!");
        return base.programa();
    }
	public override IEnumerator finalizar() {
		//Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando finalizar!");
		return base.finalizar ();		
	}
}
