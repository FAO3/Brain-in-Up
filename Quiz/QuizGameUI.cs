using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class QuizGameUI : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private QuizManager _quizManager;               //ref to the QuizManager script
    [SerializeField] private CategoryBtnScript _categoryBtnPrefab;
    [SerializeField] private GameObject _scrollHolder;
    [SerializeField] private Button _playButton, _editorButton, _exitButton, _backButton, _pauseButton;
    [SerializeField] private Text _scoreText, _timerText, _highScoreText;
    [SerializeField] private List<Image> _lifeImageList;
    [SerializeField] private GameObject _gameOverPanel, _mainMenu, _gamePanel, _upHolder, _pauseMenu, _poinLight, _wrongEmotions;
    [SerializeField] private Color _correctColor, _wrongColor, _normalColor; //color of buttons
    [SerializeField] private Image _questionImg;                     //image component to show image
    [SerializeField] private UnityEngine.Video.VideoPlayer _questionVideo;   //to show video
    [SerializeField] private AudioSource _questionAudio;             //audio source for audio clip
    [SerializeField] private Text _questionInfoText;                 //text to show question
    [SerializeField] private List<Button> _options;                  //options button reference
    [SerializeField] private GameObject _spawner;
    [SerializeField] private Animator _moonAnimation;
    [SerializeField] private Animator _liftAnimation;
#pragma warning restore 649

    private float _audioLength;          //store audio length
    private Question _question;          //store current question data
    private bool _answered = false;      //bool to keep track if answered or not

    public Text TimerText { get => _timerText; }                     
    public Text ScoreText { get => _scoreText; }   
    public Text HighScoreText { get => _highScoreText; }
    public GameObject GameOverPanel { get => _gameOverPanel; }           

    public static UnityEvent ButtonClick = new UnityEvent();
    public static UnityEvent AwakeTimer = new UnityEvent();
    private void Start()
    {
        //add the listner to all the buttons
        for (int i = 0; i < _options.Count; i++)
        {
            Button localBtn = _options[i];
            localBtn.onClick.AddListener(() => OnClick(localBtn));
        }

        CreateCategoryButtons();

    }
    public void SetQuestion(Question question)
    {
        //set the question
        this._question = question;
        //check for questionType
        switch (question.questionType)
        {
            case QuestionType.TEXT:
                _questionImg.transform.parent.gameObject.SetActive(false);   //deactivate image holder
                break;
            case QuestionType.IMAGE:
                _questionImg.transform.parent.gameObject.SetActive(true);    //activate image holder
                _questionVideo.transform.gameObject.SetActive(false);        //deactivate questionVideo
                _questionImg.transform.gameObject.SetActive(true);           //activate questionImg
                _questionAudio.transform.gameObject.SetActive(false);        //deactivate questionAudio

                _questionImg.sprite = question.questionImage;                //set the image sprite
                break;
            case QuestionType.AUDIO:
                _questionVideo.transform.parent.gameObject.SetActive(true);  //activate image holder
                _questionVideo.transform.gameObject.SetActive(false);        //deactivate questionVideo
                _questionImg.transform.gameObject.SetActive(false);          //deactivate questionImg
                _questionAudio.transform.gameObject.SetActive(true);         //activate questionAudio
                
                _audioLength = question.audioClip.length;                    //set audio clip
                StartCoroutine(PlayAudio());                                //start Coroutine
                break;
            case QuestionType.VIDEO:
                _questionVideo.transform.parent.gameObject.SetActive(true);  //activate image holder
                _questionVideo.transform.gameObject.SetActive(true);         //activate questionVideo
                _questionImg.transform.gameObject.SetActive(false);          //deactivate questionImg
                _questionAudio.transform.gameObject.SetActive(false);        //deactivate questionAudio

                _questionVideo.clip = question.videoClip;                    //set video clip
                _questionVideo.Play();                                       //play video
                break;
        }

        _questionInfoText.text = question.questionInfo;                     //set the question text

        //suffle the list of options
        List<string> ansOptions = ShuffleList.ShuffleListItems<string>(question.options);

        //assign options to respective option buttons
        for (int i = 0; i < _options.Count; i++)
        {
            //set the child text
            _options[i].GetComponentInChildren<Text>().text = ansOptions[i];
            _options[i].name = ansOptions[i];    //set the name of button
            _options[i].image.color = _normalColor; //set color of button to normal
        }

        _answered = false;                       

    }

    public void OnClickPlay()
    {
        _playButton.gameObject.SetActive(false);
        _editorButton.gameObject.SetActive(false);
        _exitButton.gameObject.SetActive(false);
        _scrollHolder.gameObject.SetActive(true);
        _backButton.gameObject.SetActive(true);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnClickBack()
    {
        _playButton.gameObject.SetActive(true);
        _editorButton.gameObject.SetActive(true);
        _exitButton.gameObject.SetActive(true);
        _scrollHolder.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);
    }

    public void OnClickPause()
    {
        _scoreText.gameObject.SetActive(false);
        _highScoreText.gameObject.SetActive(false);
        _pauseMenu.SetActive(true);
        Time.timeScale = 0; 
    }
    public void OnClickReturn()
    {
        _scoreText.gameObject.SetActive(true);
        _highScoreText.gameObject.SetActive(true);
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnClickMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ReduceLife(int remainingLife)
    {
        _lifeImageList[remainingLife].color = Color.red;
    }

    IEnumerator PlayAudio()
    {
        //if questionType is audio
        if (_question.questionType == QuestionType.AUDIO)
        {
            //PlayOneShot
            _questionAudio.PlayOneShot(_question.audioClip);
            //wait for few seconds
            yield return new WaitForSeconds(_audioLength + 0.5f);
            //play again
            StartCoroutine(PlayAudio());
        }
        else //if questionType is not audio
        {
            //stop the Coroutine
            StopCoroutine(PlayAudio());
            //return null
            yield return null;
        }
    }

    /// <summary>
    /// Method assigned to the buttons
    /// </summary>
    /// <param name="btn">ref to the button object</param>
    void OnClick(Button btn)
    {
        if (_quizManager.GameStatus == GameStatus.PLAYING)
        {
            //if answered is false
            if (!_answered)
            {
                //set answered true
                _answered = true;
                //get the bool value
                bool val = _quizManager.Answer(btn.name);

                //if its true
                if (val)
                {
                    //set color to correct
                    //btn.image.color = correctCol;
                    StartCoroutine(BlinkImg(btn.image));
                    StartCoroutine(waiter());

                    ButtonClick?.Invoke();
                    //_spawner.gameObject.SetActive(true);

                    //QuizManager.GameStatus = GameStatus.INPROCESS;

                    //Time.timeScale = 1;
                }
                else
                {
                    //else set it to wrong color
                    btn.image.color = _wrongColor;
                    _liftAnimation.SetBool("isCollision", true);
                    StartCoroutine(waiter());
                    _wrongEmotions.gameObject.SetActive(true);

                    //_spawner.gameObject.SetActive(true);
                    ButtonClick?.Invoke();
                    
                    //Time.timeScale = 1;
                }
            }
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(0.3f);
        _liftAnimation.SetBool("isCollision", false);
        _wrongEmotions.gameObject.SetActive(false);
        _pauseButton.gameObject.SetActive(true);
        _gamePanel.SetActive(false);
        _moonAnimation.StopPlayback();
        _liftAnimation.StopPlayback();
    }

    void CreateCategoryButtons()
    {
        //we loop through all the available catgories in our QuizManager
        for (int i = 0; i < _quizManager.QuizData.Count; i++)
        {
            //Create new CategoryBtn
            CategoryBtnScript categoryBtn = Instantiate(_categoryBtnPrefab, _scrollHolder.transform);
            //Set the button default values
            categoryBtn.SetButton(_quizManager.QuizData[i].categoryName, _quizManager.QuizData[i].questions.Count);
            int index = i;
            //Add listner to button which calls CategoryBtn method
            categoryBtn.Btn.onClick.AddListener(() => CategoryBtn(index, _quizManager.QuizData[index].categoryName));
        }
    }

    //Method called by Category Button
    private void CategoryBtn(int index, string category)
    {
        _quizManager.StartGame(index, category); //start the game
        _mainMenu.SetActive(false);              //deactivate mainMenu
        _upHolder.SetActive(true);
        _spawner.SetActive(true);    // activate spawner
        _poinLight.SetActive(true);
        _pauseButton.gameObject.SetActive(true);
        LiftCollision.QuestEvent.AddListener(switcher);              
    }

    private void switcher()
    {
        AwakeTimer?.Invoke();
        //_spawner.gameObject.SetActive(false);
        _moonAnimation.StartPlayback();
        _liftAnimation.StartPlayback();
        _gamePanel.SetActive(true);
        _pauseButton.gameObject.SetActive(false);
        //Time.timeScale = 0;

    }

    IEnumerator BlinkImg(Image img)
    {
        for (int i = 0; i < 2; i++)
        {
            img.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            img.color = _correctColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RestryButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
