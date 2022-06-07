using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class UIEditor : MonoBehaviour
{
    [SerializeField] private GameObject _editorPanel, _mainMenu;
    [SerializeField] private InputField _inputField, _inputField2, _inputField3, _inputField4, _inputField5;
    [SerializeField] private QuizDataScriptable _quizDataScriptable;
    [SerializeField] private QuizManager _quizManager;
    [SerializeField] private Question _question;
    [SerializeField] private Text _questCount;
    [SerializeField] private Text _optionCount, _optionWords;
    [SerializeField] private Text _categoryCount;
    [SerializeField] private Dropdown _savedCategories;
    [SerializeField] private Button _saveButton, _backButton, _removeOption;
    [SerializeField] private CategoryList _categoryList = new CategoryList();

    private string _listCategories = "Category";


    public void ActiveEditor()
    {
        _editorPanel.SetActive(true);
        _mainMenu.SetActive(false);
        if(File.Exists(Application.persistentDataPath + $"/{_listCategories}.so") == false)
        {
            SaveListCategory();
            LoadListCategory();
            ActiveDropdown();
        }
        else
        {
            LoadListCategory();
            ActiveDropdown();
            CategoryCount();
        }
    }

    public void DeactiveEditor()
    {
        SaveListCategory();
        SceneManager.LoadScene(0); 
    }

    public void SetCategory()
    {
        if (_categoryList.Categories.Contains(_inputField.text) == false && _inputField.text != string.Empty)
        {
            _inputField.image.color = Color.white;
            _quizDataScriptable.categoryName = _inputField.text;
            _quizDataScriptable.questions.Clear();
            QuestInList(_quizDataScriptable.questions.Count.ToString());
        }
        else
        {
            _inputField.image.color = Color.red;
            //Debug.Log("reserved");
        }
    }

    public void SetQuestionInfo()
    {
        if (_inputField2.text != "" && _inputField.text != string.Empty)
        {
            _inputField2.image.color = Color.white;
            _question.questionInfo = _inputField2.text;
        }
        else if (_inputField.text == string.Empty)
        {
            _inputField.image.color = Color.red;
        }
        else
        {
            _inputField2.image.color = Color.red;
        }
    }

    public void SetQuestionOption()
    {

        if (_question.options.Count < 4)
        {
            if (_inputField3.text != "")
            {
                _removeOption.gameObject.SetActive(true);
                _question.options.Add(_inputField3.text);
                _inputField3.image.color = Color.white;
                _optionCount.text = "x: " + _question.options.Count.ToString();
                _optionWords.text = _question.options[_question.options.Count - 1].ToString();
                _inputField3.text = "";
            }
            else
            {
                _inputField3.image.color = Color.red;
            }
        }
    }

    public void DeleteQuestionOption()
    {
        if(_question.options.Count > 0)
        {
            int i = _question.options.Count - 1;
            _question.options.RemoveAt(i);
            _optionCount.text = "x: " + _question.options.Count.ToString();
            if (i != 0)
            {
                _optionWords.text = _question.options[i - 1].ToString();
            }
            else
            {
                _optionWords.text = "";
                _removeOption.gameObject.SetActive(false);
            }
        }
    }

    public void SetCorrectAnswer()
    {
        if(_inputField4.text != "" && _question.options.Contains(_inputField4.text) == true)
        {
            _inputField4.image.color = Color.white;
            _question.correctAns = _inputField4.text;
        }
        else
        {
            _inputField4.image.color = Color.red;
        }
    }

    public void SaveUserQuestion()
    {
        if (_categoryList.Categories.Contains(_inputField.text) == false && _inputField.text != string.Empty)
        {
            _categoryList.Categories.Add(_inputField.text);
            SaveListCategory();
            ActiveDropdown();
            CategoryCount();
            _removeOption.gameObject.SetActive(false);
            _optionWords.text = "";
        }
        else
        {
            _inputField.image.color = Color.red;
        }
        _inputField.readOnly = false;
        _quizDataScriptable.SaveState();
        //_quizDataScriptable.Transport();
        _quizDataScriptable.questions.Clear();
        _inputField.text = "";
        QuestInList(_quizDataScriptable.questions.Count.ToString()); //вынести в метод 
    }
    public void ActiveDropdown()
    {
        _savedCategories.ClearOptions();
        _savedCategories.AddOptions(_categoryList.Categories);
    }
    public void OnClickDropdown()
    {
        int value = _savedCategories.value;
        string buffer = _savedCategories.options[value].text;
        if (_categoryList.Categories.Contains(buffer) == true)
        {
            _inputField.image.color = Color.white;
            _quizDataScriptable.categoryName = buffer;
            _quizDataScriptable.LoadState();
            QuestInList(_quizDataScriptable.questions.Count.ToString());
            _inputField.text = buffer;
            _inputField.readOnly = true;
            _saveButton.gameObject.SetActive(false);
            _backButton.gameObject.SetActive(true);
        }
    }

    public void OnClickBack()
    {
        _quizDataScriptable.questions.Clear();
        _inputField.text = "";
        _inputField2.text = "";
        _inputField3.text = "";
        _inputField4.text = "";
        _optionWords.text = "";
        _removeOption.gameObject.SetActive(false);
        QuestInList(_quizDataScriptable.questions.Count.ToString());
        _inputField.readOnly = false;
        _backButton.gameObject.SetActive(false);
        _saveButton.gameObject.SetActive(true);
    }

    public void RemoveQuestion()
    {
        if (_quizDataScriptable.questions.Count != 0)
        {
            int i = _quizDataScriptable.questions.Count - 1;
            _quizDataScriptable.questions.RemoveAt(i);
            _quizDataScriptable.SaveState();
            QuestInList(_quizDataScriptable.questions.Count.ToString());
        }
    }
    public void RemoveCategory()
    {
        if(_categoryList.Categories.Count != 0)
        {
            int i = _categoryList.Categories.Count - 1;
            _categoryList.Categories.RemoveAt(i);
            SaveListCategory();
            ActiveDropdown();
            CategoryCount();
        }
    }

    public void AddNewCategory()
    {
        if (_question.questionInfo != string.Empty && _question.options.Count == 4 && _question.correctAns != string.Empty)
        {
            _quizDataScriptable.questions.Add(_question);
            _quizDataScriptable.SaveState();
            QuestInList(_quizDataScriptable.questions.Count.ToString());
            _removeOption.gameObject.SetActive(false);
            _optionWords.text = "";
            ClearFeilds();
            _question = new Question();
        }
        else if (_question.questionInfo == string.Empty)
        {
            _inputField2.image.color = Color.red;
        }
        else if(_question.options.Count == 0 || _question.options.Count < 4)
        {
            _inputField3.image.color = Color.red;
        }
        else if (_question.correctAns == string.Empty)
        {
            _inputField4.image.color = Color.red;
        }  
    }
    private void ClearFeilds()
    {
        _inputField2.text = "";
        _inputField4.text = "";
        _optionCount.text = "x: ";
    }

    public void SaveListCategory()
    {
        var json = JsonUtility.ToJson(_categoryList, true);

        File.WriteAllText(GetFilePath(), json);
    }

    public void LoadListCategory()
    {
        var json = File.ReadAllText(GetFilePath());
        JsonUtility.FromJsonOverwrite(json, _categoryList);
    }

    private string GetFilePath()
    {
        return Application.persistentDataPath + $"/{_listCategories}.so";
    }

    private void QuestInList(string count)
    {
        _questCount.text = "Quest Count: " + count;
    }

    public void CategoryCount()
    {
        _categoryCount.text = "Choose your category from: " + _categoryList.Categories.Count.ToString();
    }

    [System.Serializable]
    private class CategoryList
    {
        public List<string> Categories = new List<string>();
    }
}
