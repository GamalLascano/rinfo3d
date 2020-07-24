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
    public static List<robotCode> listOfRobotCodes=new List<robotCode>();
    public static void parseRobotCode(int currentline, ArrayList sentences)
    {
        string aux = (string) sentences[currentline];
        int index = currentline;
        while (aux.Contains("robot "))
        {
            string name = aux.Substring("robot ".Length);
            index = index + 2;
            int start = index;
            while(!((string) sentences[index] == "finalizar")||(index<sentences.Count))
            {
                index++;
            }
            robotCode newRob = new robotCode(name, sentences.GetRange(start, index - 1));
            listOfRobotCodes.Add(newRob);
            if((index+1)< sentences.Count)
            {
                aux = (string)sentences[index + 1];
            }
        }
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
