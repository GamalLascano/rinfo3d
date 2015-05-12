using UnityEngine;
using System.Collections;

public class SimpleRobotBehaviour : RobotBehaviour {

	/**
	 * Metodo a implementar
	 */
	public override void mover()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando mover!");
	}
	
	/**
	 * Metodo a implementar
	 */
	public override void derecha()
	{
		Debug.Log ("Soy SimpleRobotBehaviour y estoy ejecutando derecha!");
	}	


}
