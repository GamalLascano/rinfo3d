using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelRobots : MonoBehaviour
{
    public class robotCode{
        public string name { get; set; }
        public ArrayList code { get; set; }
        public robotCode(string n,ArrayList ar)
        {
            name = n;
            code = ar;
        }
    }
    public List<robotCode> listOfRobotCodes=new List<robotCode>();
    public void parseRobotCode(int currentline, ArrayList sentences)
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
