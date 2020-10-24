﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ParallelRobots : MonoBehaviour
{
    public class robotCode{
        public string name { get; set; }
        public ArrayList code { get; set; }
        public int offset { get; set; }
        public robotCode(string n,ArrayList ar,int of)
        {
            name = n;
            code = ar;
            offset = of;
        }
        public robotCode()
        {
            name = "null";
            code = new ArrayList();
            offset = 0;
        }
    }
    public class robotBools
    {
        public int robotIndexRef { get; set; } = -1;
        public int currentLine { get; set; } = -1;
        public List<UI.ControlBools> ListOfControlBools { get; set; } = new List<UI.ControlBools>();
        public bool step { get; set; } = false;
        public bool run { get; set; } = true;
        public bool ended { get; set; } = false;
        public bool executingCurrentLine { get; set; } = true;
    }
    public static List<robotCode> listOfRobotCodes=new List<robotCode>();
    public static List<robotBools> listOfRobotBools = new List<robotBools>();
    private static List<UI.RepeatBools> ListOfRepeatBools = new List<UI.RepeatBools>();
    public static void setRepeat(int loop, int value, int value2)
    {
        ListOfRepeatBools.Add(new UI.RepeatBools(loop, value, value2));
    }
    public static int parseRobotCode(int currentline, ArrayList sentences)
    {
        int index = currentline+1;
        string aux = (string) sentences[index];
        
        while (aux.Contains("robot"))
        {
            //Tengo que verificar si tiene comenzar tambien
            string name = aux.Substring("robot".Length);
            index = index + 2;
            int start = index;
            
            while(!String.Equals((string)sentences[index], "finalizar") && (index<sentences.Count))
            {
                Debug.Log((string)sentences[index] + " finalizar");
                index++;
            }
            int ayudapls = index - start;
            robotCode newRob = new robotCode(name, sentences.GetRange(start, ayudapls),start);
            if (Init.robotInstance.Count > 1) listOfRobotCodes.Add(new robotCode());
            listOfRobotCodes.Add(newRob);
            if((index+1)< sentences.Count)
            {
                aux = (string)sentences[index + 1];
            }
        }
        for(int i=1; i < listOfRobotCodes.Count; i++)
        {
            Debug.Log("---RobotCode " + i + "---");
            for(int j = 0; j < listOfRobotCodes[i].code.Count; j++)
            {
                Debug.Log(listOfRobotCodes[i].code[j]);
            }
        }
        return index;
    }
    public IEnumerator codeExecution(int robotIndex)
    {

        robotBools aux = new robotBools
        {
            robotIndexRef = robotIndex
        };
        
        listOfRobotBools.Add(aux);
        int indexLRB = listOfRobotBools.IndexOf(aux);
        Transform theRobot = (Transform)Init.getRobotInstance(robotIndex).robInstance;
        RobotBehaviour behaviour = (RobotBehaviour)theRobot.GetComponent<RobotBehaviour>();
        behaviour.robotOffset = listOfRobotCodes[robotIndex].offset;
        while (listOfRobotBools[indexLRB].ended==false)
        {
            listOfRobotBools[indexLRB].step = true;
            listOfRobotBools[indexLRB].currentLine++;
            if (listOfRobotBools[indexLRB].currentLine == listOfRobotCodes[robotIndex].code.Count)
            {
                //statusText = I18N.getValue("finished");
                listOfRobotBools[indexLRB].currentLine = -1;
                listOfRobotBools[indexLRB].run = false;
                listOfRobotBools[indexLRB].ended = true;
                if (!listOfRobotCodes[robotIndex].code.Contains("finalizar"))
                {
                    // Invocar a la corutina encargada de ejecutar la visualizacion
                    theRobot = (Transform)Init.getRobotInstance(robotIndex).robInstance;
                    behaviour = (RobotBehaviour)theRobot.GetComponent<RobotBehaviour>();
                    behaviour.StartCoroutine("finalizar", 0);
                }
            }
            else if (listOfRobotCodes[robotIndex].code[listOfRobotBools[indexLRB].currentLine] != null && listOfRobotCodes[robotIndex].code[listOfRobotBools[indexLRB].currentLine].ToString().Length > 0)
            {
                listOfRobotBools[indexLRB].executingCurrentLine = true;
                object[] paramos = new object[2] { listOfRobotBools[indexLRB].currentLine, indexLRB };
                UI.getBigBang().GetComponent<ParallelRobots>().StartCoroutine("executeParallelLine", paramos);
                if (ListOfRepeatBools.Count > 0)
                {
                    if (listOfRobotBools[indexLRB].currentLine == ListOfRepeatBools[ListOfRepeatBools.Count - 1].instructionStopValue)
                    {
                        if (ListOfRepeatBools[ListOfRepeatBools.Count - 1].currentLoop < ListOfRepeatBools[ListOfRepeatBools.Count - 1].loops)
                        {
                            ListOfRepeatBools[ListOfRepeatBools.Count - 1].currentLoop++;
                            listOfRobotBools[indexLRB].currentLine = ListOfRepeatBools[ListOfRepeatBools.Count - 1].instructionStartValue - 1;
                        }
                        else
                        {
                            ListOfRepeatBools.RemoveAt(ListOfRepeatBools.Count - 1);
                        }
                    }
                }
                 if (listOfRobotBools[indexLRB].ListOfControlBools.Count > 0)
                 {
                     if (listOfRobotBools[indexLRB].currentLine == listOfRobotBools[indexLRB].ListOfControlBools[listOfRobotBools[indexLRB].ListOfControlBools.Count - 1].instructionStopValue)
                     {
                        listOfRobotBools[indexLRB].currentLine = listOfRobotBools[indexLRB].ListOfControlBools[listOfRobotBools[indexLRB].ListOfControlBools.Count - 1].instructionStartValue - 1;
                     }
                 }
            }

            // Mientras que este ejecutando, esperar 1 - currentRunningSpeed
            while (listOfRobotBools[indexLRB].executingCurrentLine)
            {
                yield return new WaitForSeconds(1- UI.currentRunningSpeed);
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
        Debug.Log("Termino la ejecucion de parallelRobots");
        yield return new WaitForSeconds(0);
    }
    public IEnumerator executeParallelLine(object[] paramos)
    {
        int currentLine=(int)paramos[0]; 
        int indexLRB = (int)paramos[1];
        // FIXME: Aqui deberia delegarse al robot a fin de que realice la animacion
        int robotIndex = listOfRobotBools[indexLRB].robotIndexRef;
        string status = I18N.getValue("exec_line") + (currentLine + 1) + ": " + listOfRobotCodes[robotIndex].code[currentLine];
       // statusText = status;

        // Invocar ejecucion visual via reflection
        try
        {
            // Recuperar el BigBang, y a partir de alli el Robot que se tenga configurado
            Transform theRobot = (Transform)Init.getRobotInstance(robotIndex).robInstance;
            RobotBehaviour behaviour = (RobotBehaviour)theRobot.GetComponent<RobotBehaviour>();
            Type type = behaviour.GetType();
            // Pruebas para argumentos.  Esto igualmente se recibe desde libreria
            string sentence = (string)listOfRobotCodes[robotIndex].code[currentLine];
            string sentenceName = "";
            sentenceName = sentence.Substring(0, sentence.Contains("(") ? sentence.IndexOf("(") : sentence.Length);
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
            Debug.Log(sentenceName);
            // Invocar a la corutina encargada de ejecutar la visualizacion
            behaviour.StartCoroutine(methodInfo.Name, 0);

        }
        catch (Exception e)
        {
            Debug.Log("Exception!! " + e.ToString());
         //   statusText = I18N.getValue("unknown_line") + (currentLine + 1) + ": " + sentences[currentLine];
            listOfRobotBools[indexLRB].run = false;
        }
        Debug.Log("Salgo en linea " + listOfRobotBools[indexLRB].currentLine);
        yield return new WaitForSeconds(0);
    }
    
    public static void setLoop(int value, bool state, int value2,int indexLRB)
    {
        if (state == true)
        {
            if (listOfRobotBools[indexLRB].ListOfControlBools.Count > 0)
            {
                if (listOfRobotBools[indexLRB].ListOfControlBools[listOfRobotBools[indexLRB].ListOfControlBools.Count - 1].inLoop == true)
                {
                    if (listOfRobotBools[indexLRB].ListOfControlBools[listOfRobotBools[indexLRB].ListOfControlBools.Count - 1].instructionStartValue != value2)
                    {
                        listOfRobotBools[indexLRB].ListOfControlBools.Add(new UI.ControlBools(state, value, value2));
                    }
                }
            }
            else
            {
                listOfRobotBools[indexLRB].ListOfControlBools.Add(new UI.ControlBools(state, value, value2));
            }
        }
        else
        {
            if (listOfRobotBools[indexLRB].ListOfControlBools.Count > 0)
            {
                if ((listOfRobotBools[indexLRB].ListOfControlBools[listOfRobotBools[indexLRB].ListOfControlBools.Count - 1].inLoop == true) && (listOfRobotBools[indexLRB].ListOfControlBools[listOfRobotBools[indexLRB].ListOfControlBools.Count - 1].instructionStartValue == value2))
                {
                    listOfRobotBools[indexLRB].ListOfControlBools.RemoveAt(listOfRobotBools[indexLRB].ListOfControlBools.Count - 1);
                }
            }
            listOfRobotBools[indexLRB].currentLine = value;
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
