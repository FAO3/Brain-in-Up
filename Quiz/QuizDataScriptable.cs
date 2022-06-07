using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "QuestionsData", menuName = "QuestionsData", order = 1)]
public class QuizDataScriptable : ScriptableObject
{
    private QuizManager _quizManager;
    public string categoryName;
    public List<Question> questions;

    [ContextMenu("Save")]
    public void SaveState()
    {
        var json = JsonUtility.ToJson(this, true);

        File.WriteAllText(GetFilePath(), json);
        //Debug.Log(GetFilePath());
    }

    [ContextMenu("Load")]
    public void LoadState()
    {
        var json = File.ReadAllText(GetFilePath());
        JsonUtility.FromJsonOverwrite(json, this);
    }
    [ContextMenu("Transport")]
    public void Transport()
    {
        _quizManager = GameObject.FindObjectOfType<QuizManager>();
        _quizManager.QuizData.Add(this);
    }

    private string GetFilePath()
    {
        return Application.persistentDataPath + $"/{categoryName}.so";
    }
}
