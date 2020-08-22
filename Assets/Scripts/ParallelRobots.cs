using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
    public class robotBools
    {
        public int robotIndexRef { get; set; } = -1;
        public int currentLine { get; set; } = -1;
        public bool step { get; set; } = false;
        public bool run { get; set; } = true;
        public bool ended { get; set; } = false;
        public bool executingCurrentLine { get; set; } = true;
    }
    public static List<robotCode> listOfRobotCodes=new List<robotCode>();
    public static List<robotBools> listOfRobotBools = new List<robotBools>();
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
    public IEnumerator codeExecution(int robotIndex)
    {
        robotBools aux = new robotBools
        {
            robotIndexRef = robotIndex
        };
        listOfRobotBools.Add(aux);
        int indexLRB = listOfRobotBools.IndexOf(aux);
        while (listOfRobotBools[indexLRB].ended==false)
        {
            listOfRobotBools[indexLRB].step = true;
            listOfRobotBools[indexLRB].currentLine++;

            if (listOfRobotBools[indexLRB].currentLine == listOfRobotCodes[robotIndex].code.Count - 1)
            {
                //statusText = I18N.getValue("finished");
                listOfRobotBools[indexLRB].currentLine = -1;
                listOfRobotBools[indexLRB].run = false;
                listOfRobotBools[indexLRB].ended = true;
                if (!listOfRobotCodes[robotIndex].code.Contains("finalizar"))
                {
                    // Invocar a la corutina encargada de ejecutar la visualizacion
                    Transform theRobot = (Transform)Init.getRobotInstance(robotIndex).robInstance;
                    RobotBehaviour behaviour = (RobotBehaviour)theRobot.GetComponent<RobotBehaviour>();
                    behaviour.StartCoroutine("finalizar", 0);
                }
            }
            else if (listOfRobotCodes[robotIndex].code[listOfRobotBools[indexLRB].currentLine] != null && listOfRobotCodes[robotIndex].code[listOfRobotBools[indexLRB].currentLine].ToString().Length > 0)
            {
                listOfRobotBools[indexLRB].executingCurrentLine = true;
                executeParallelLine(listOfRobotBools[indexLRB].currentLine,indexLRB);
               // if (ListOfRepeatBools.Count > 0)
               // {
               //     if (currentLine == ListOfRepeatBools[ListOfRepeatBools.Count - 1].instructionStopValue)
               //     {
               //         if (ListOfRepeatBools[ListOfRepeatBools.Count - 1].currentLoop < ListOfRepeatBools[ListOfRepeatBools.Count - 1].loops)
               //         {
               //             Debug.Log(ListOfRepeatBools.Count);
               //             ListOfRepeatBools[ListOfRepeatBools.Count - 1].currentLoop++;
               //             currentLine = ListOfRepeatBools[ListOfRepeatBools.Count - 1].instructionStartValue - 1;
               //         }
               //         else
               //         {
               //             ListOfRepeatBools.RemoveAt(ListOfRepeatBools.Count - 1);
               //         }
               //     }
               // }
               // if (ListOfControlBools.Count > 0)
               // {
               //     if (currentLine == ListOfControlBools[ListOfControlBools.Count - 1].instructionStopValue)
               //     {
               //         currentLine = ListOfControlBools[ListOfControlBools.Count - 1].instructionStartValue - 1;
               //     }
               // }
            }

            // Mientras que este ejecutando, esperar 1 - currentRunningSpeed
            while (listOfRobotBools[indexLRB].executingCurrentLine)
            {
                yield return new WaitForSeconds(0);
            }

            listOfRobotBools[indexLRB].step = false;

            // Hubo un error?
            //if (runtimeErrorMsg != null)
            //{
            //    angry = true;
            //    statusText = "Error ejecutando linea " + (currentLine + 1) + ": " + sentences[currentLine] + ". " + runtimeErrorMsg;
            //    run = false;
            //    ended = true;
            //    currentLine = -1;
            //}
        }
        yield return new WaitForSeconds(0);
    }
    public IEnumerator executeParallelLine(int currentLine, int indexLRB)
    {
        // FIXME: Aqui deberia delegarse al robot a fin de que realice la animacion
        int robotIndex = listOfRobotBools[indexLRB].robotIndexRef;
        string status = I18N.getValue("exec_line") + (currentLine + 1) + ": " + listOfRobotCodes[robotIndex].code[currentLine];
       // statusText = status;

        // Invocar ejecucion visual via reflection
        try
        {
            object result = null;
            // Recuperar el BigBang, y a partir de alli el Robot que se tenga configurado
            Transform theRobot = (Transform)Init.getRobotInstance(robotIndex).robInstance;
            RobotBehaviour behaviour = (RobotBehaviour)theRobot.GetComponent<RobotBehaviour>();
            Type type = behaviour.GetType();
            // Pruebas para argumentos.  Esto igualmente se recibe desde libreria
            string sentence = (string)listOfRobotCodes[robotIndex].code[currentLine];
            string sentenceName = "";
            switch (currentLine)
            {
                case 0:
                    sentenceName = sentence.Substring(0, "programa".Length);
                    behaviour.resetArguments();
                    string arg;
                    arg = sentence.Substring("programa".Length, sentence.Length - "programa".Length);
                    behaviour.addArgument(arg);
                    break;
                case 2:
                    sentenceName = sentence.Replace(" ", "").Substring(sentence.IndexOf(':') + 1, "AreaC".Length);
                    break;
                default:
                    sentenceName = sentence.Substring(0, sentence.Contains("(") ? sentence.IndexOf("(") : sentence.Length);
                    break;
            }
            // Cargar los parametros segun la instruccion que sea.  FIXME: Deshardcode
            if (sentence.Contains("("))
            {
                behaviour.resetArguments();
                string sentenceArgs = sentence.Substring(sentence.IndexOf("("), sentence.Length - sentence.IndexOf("("));
                sentenceArgs = sentenceArgs.Replace("(", "").Replace(")", "").Replace(" ", "");
                string[] args = sentenceArgs.Split(","[0]);
                for (int i = 0; i < args.Length; i++)
                    behaviour.addArgument(args[i]);
            }

            MethodInfo methodInfo = type.GetMethod(sentenceName);
            // ParameterInfo[] parameters = methodInfo.GetParameters();

            // Invocar a la corutina encargada de ejecutar la visualizacion
            behaviour.StartCoroutine(methodInfo.Name, 0);

        }
        catch (Exception e)
        {
            Debug.Log("Exception!! " + e.ToString());
         //   statusText = I18N.getValue("unknown_line") + (currentLine + 1) + ": " + sentences[currentLine];
            listOfRobotBools[indexLRB].run = false;
        }
        yield return new WaitForSeconds(0);
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
