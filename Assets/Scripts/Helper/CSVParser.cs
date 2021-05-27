using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class CSVParser
{
    public string pathCSV;


    private char lineSeperater = '\n'; // It defines line seperate character
    private char fieldSeperator = ','; // It defines field seperate chracter

    public CSVParser(List<string> data)
    {
#if UNITY_EDITOR
        pathCSV = getPath() + "/Data/" + SceneManager.GetActiveScene().name + "_" +
                  DateTime.Now.ToString("yyyy_dd_M__HH_mm_ss") + ".csv";
        File.Create(pathCSV).Close();
        File.AppendAllText(pathCSV, String.Join(fieldSeperator.ToString(), data) + lineSeperater);
#endif
    }

    private void readData(List<string> data)
    {
        string[] records = data.ToArray();
        foreach (string record in records)
        {
            string[] fields = record.Split(fieldSeperator);
            foreach (string field in fields)
            {
                Debug.Log(field + "\t");
            }

            Debug.Log('\n');
        }
    }
    
    private void readData(string[] records)
    {
        foreach (string record in records)
        {
            string[] fields = record.Split(fieldSeperator);
            foreach (string field in fields)
            {
                Debug.Log(field + "\t");
            }

            Debug.Log('\n');
        }
    }

    public void addData(List<string> data)
    {
        File.AppendAllText(pathCSV,
            DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + fieldSeperator +
            String.Join(fieldSeperator.ToString(), data) + lineSeperater);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        readData(data);
    }
    
    public void addData(string[] data)
    {
        File.AppendAllText(pathCSV,
            DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + fieldSeperator +
            String.Join(fieldSeperator.ToString(), data) + lineSeperater);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        readData(data);
    }

    private static string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath;
#elif UNITY_ANDROID
return Application.persistentDataPath;// +fileName;
#elif UNITY_IPHONE
return GetiPhoneDocumentsPath();// +"/"+fileName;
#else
return Application.dataPath;// +"/"+ fileName;
#endif
    }
}