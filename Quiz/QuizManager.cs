using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
#pragma warning disable 649
    //ref to the QuizGameUI script
    [SerializeField] private QuizGameUI _quizGameUI;
    //ref to the scriptableobject file
    [SerializeField] public List<QuizDataScriptable> _quizDataList;
    [SerializeField] private float _timeInSeconds;
#pragma warning restore 649

    private string _currentCategory = "";
    private string _currentCategoryHighScore = "";
    private int _correctAnswerCount = 0;
    //questions data
    private List<Question> _questions;
    //current question data
    private Question _selectedQuetion = new Question();
    private int _gameScore;
    private int _gameHighScore;
    private int _lifesRemaining;
    private float _currentTime;
    private QuizDataScriptable _dataScriptable;

    private GameStatus _gameStatus = GameStatus.NEXT;
    public GameStatus GameStatus { get { return _gameStatus; }}

    [SerializeField] public List<QuizDataScriptable> QuizData { get => _quizDataList; }

    public void StartGame(int categoryIndex, string category)
    {
        _currentCategory = category;
        _currentCategoryHighScore = category + categoryIndex;
        _correctAnswerCount = 0;
        _gameScore = 0;
        if (PlayerPrefs.HasKey(_currentCategoryHighScore))
        {
            _gameHighScore = PlayerPrefs.GetInt(_currentCategoryHighScore);
            _quizGameUI.HighScoreText.text = "High Score:" + _gameHighScore;
        }
        _lifesRemaining = 3;
        //set the questions data
        _questions = new List<Question>();
        _dataScriptable = _quizDataList[categoryIndex];
        _questions.AddRange(_dataScriptable.questions);
        //select the question
        SelectQuestion();
        
        //gameStatus = GameStatus.PLAYING;

        QuizGameUI.AwakeTimer.AddListener(GameStatusPLAYING);
        QuizGameUI.ButtonClick.AddListener(GameStatusNEXT);
    }

    /// <summary>
    /// Method used to randomly select the question form questions data
    /// </summary>
    private void SelectQuestion()
    {
        //get the random number
        int val = UnityEngine.Random.Range(0, _questions.Count);
        //set the selectedQuetion
        _selectedQuetion = _questions[val];
        //send the question to quizGameUI
        _quizGameUI.SetQuestion(_selectedQuetion);

        _questions.RemoveAt(val);
    }

    private void GameStatusNEXT()
    {
        _gameStatus = GameStatus.NEXT;
    }
    private void GameStatusPLAYING()
    {
        _currentTime = _timeInSeconds;
        _gameStatus = GameStatus.PLAYING;
    }

    private void Update()
    {

        if (_gameStatus == GameStatus.PLAYING)
        {
            _currentTime -= Time.deltaTime;
            SetTime(_currentTime);
        }
    }

    void SetTime(float value)
    {
        TimeSpan time = TimeSpan.FromSeconds(_currentTime);      //set the time value
        _quizGameUI.TimerText.text = time.ToString("mm':'ss");   //convert time to Time format

        if (_currentTime <= 0)
        {
            //Game Over
            GameEnd();
        }
    }

    /// <summary>
    /// Method called to check the answer is correct or not
    /// </summary>
    /// <param name="selectedOption">answer string</param>
    /// <returns></returns>
    public bool Answer(string selectedOption)
    {
        //set default to false
        bool correct = false;
        //if selected answer is similar to the correctAns
        if (_selectedQuetion.correctAns == selectedOption)
        {
            //Yes, Ans is correct
            _correctAnswerCount++;
            correct = true;
            _gameScore += 50;
            _quizGameUI.ScoreText.text = "Score:" + _gameScore;
            if(_gameScore > _gameHighScore)
            {
                _gameHighScore = _gameScore;
                PlayerPrefs.SetInt(_currentCategoryHighScore, _gameHighScore);
                _quizGameUI.HighScoreText.text = "High Score:" + _gameHighScore;
            }
        }
        else
        {
            //No, Ans is wrong
            //Reduce Life
            _lifesRemaining--;
            if (_gameScore != 0)
            {
                _gameScore -= 25;
                _quizGameUI.ScoreText.text = "Score:" + _gameScore;
            }   
            _quizGameUI.ReduceLife(_lifesRemaining);

            if (_lifesRemaining == 0)
            {
                GameEnd();
            }
        }

        if (_gameStatus == GameStatus.PLAYING)
        {
            if (_questions.Count > 0)
            {
                //call SelectQuestion method again after 1s
                Invoke("SelectQuestion", 0.4f);
            }
            else
            {
                GameEnd();
            }
        }
        //return the value of correct bool
        return correct;
    }

    private void GameEnd()
    {
        _gameStatus = GameStatus.NEXT;
        _quizGameUI.GameOverPanel.SetActive(true);
        Time.timeScale = 0;

        //fi you want to save only the highest score then compare the current score with saved score and if more save the new score
        //eg:- if correctAnswerCount > PlayerPrefs.GetInt(currentCategory) then call below line
        if (_correctAnswerCount > PlayerPrefs.GetInt(_currentCategory))
        {
            PlayerPrefs.SetInt(_currentCategory, _correctAnswerCount); //save the score for this category
        }

        //Save the score
            
    }
}

[System.Serializable]
public class Question
{
    public string questionInfo;    
    public QuestionType questionType;
    public Sprite questionImage;     
    public AudioClip audioClip;      
    public UnityEngine.Video.VideoClip videoClip;
    public List<string> options = new List<string>(4);     
    public string correctAns;
}

[System.Serializable]
public enum QuestionType
{
    TEXT,
    IMAGE,
    AUDIO,
    VIDEO
}

[SerializeField]
public enum GameStatus
{
    PLAYING,
    NEXT,
    INPROCESS
}