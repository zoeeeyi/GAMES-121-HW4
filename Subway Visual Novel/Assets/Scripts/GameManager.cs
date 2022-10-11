using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Canvas m_uiCanvas;
    [SerializeField] List<TextMeshProUGUI> m_textBoxList = new List<TextMeshProUGUI>();
    //[SerializeField] List<TextMeshProUGUI> m_textBoxLeft = new List<TextMeshProUGUI>();
    //[SerializeField] List<TextMeshProUGUI> m_textBoxRight = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> m_textWaitLeft = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> m_textWaitRight = new List<TextMeshProUGUI>();
    public TextMeshProUGUI[] m_textStageLeft;
    public TextMeshProUGUI[] m_textStageRight;

    [SerializeField] Vector2 m_leftFirstPos;
    [SerializeField] Vector2 m_rightFirstPos;
    Vector3 m_leftTopPos;
    Vector3 m_rightTopPos;
    List<Vector3> m_leftPositionList;
    List<Vector3> m_rightPositionList;

    [SerializeField] float m_lineSpace;
    public float textBoxMoveSmoothTime;
    public float textColorTransitionTime;

    void Start()
    {
        //Reset all text boxes
        for (int i = 0; i < m_textBoxList.Count; i++)
        {
            //Reset the text box
            ResetTextBox(m_textBoxList[i]);
        }

        //Set up on stage arrays
        m_textStageLeft = new TextMeshProUGUI[m_textWaitLeft.Count];
        m_textStageRight = new TextMeshProUGUI[m_textWaitRight.Count];

        /*for (int i = 0; i < m_textStageLeft.Length; i++)
        {
            m_textStageLeft[i] = null;
        }

        for (int i = 0; i < m_textStageRight.Length; i++)
        {
            m_textStageRight[i] = null;
        }*/

    }

    void Update()
    {
        //Update top position
        /*if (m_textStageLeft[m_textStageLeft.Length - 1] != null)
        {
            m_leftTopPos = m_textStageLeft[m_textStageLeft.Length - 1].rectTransform.anchoredPosition;
        }

        if (m_textStageRight[m_textStageRight.Length - 1] != null)
        {
            m_rightTopPos = m_textStageRight[m_textStageRight.Length - 1].rectTransform.anchoredPosition;
        }*/

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NewLine(m_textStageRight, m_textWaitRight, true, "THIS IS A VERY LONG TEXT!");
        }
    }

    void UpdateTextBox(bool _isLeft, bool _isSelection, string _text)
    {
        //List<TextMeshProUGUI> _updatingGroup = (_isLeft) ? m_textBoxLeft : m_textBoxRight;
        int _horizontalMoveDir = _isLeft ? 1 : -1;


    }

    public void DeleteSelection(TextMeshProUGUI _textBox, TextMeshProUGUI[] _targetStage)
    {
        TextBox _textBoxScript = _textBox.GetComponent<TextBox>();
        int _upperIndex = _textBoxScript.currentIndexOnStage + 1;
        int _lowerIndex = _textBoxScript.currentIndexOnStage - 1;

        if (_upperIndex < _targetStage.Length)
        {
            if (_targetStage[_upperIndex] != null)
            {
                if (_targetStage[_upperIndex].GetComponent<TextBox>().isSelection)
                {
                    ResetTextBox(_targetStage[_upperIndex]);
                    _targetStage[_upperIndex] = null;
                    for (int i = _textBoxScript.currentIndexOnStage; i >= 0; i--)
                    {
                        if (_targetStage[i] != null)
                        {
                            PushUpLine(_targetStage[i], _targetStage);
                        }
                    }
                }
            }
        }

        if (_lowerIndex >= 0)
        {
            if (_targetStage[_lowerIndex] != null)
            {
                if (_targetStage[_lowerIndex].GetComponent<TextBox>().isSelection)
                {
                    ResetTextBox(_targetStage[_lowerIndex]);
                    _targetStage[_lowerIndex] = null;
                    for (int i = _lowerIndex - 1; i >= 0; i--)
                    {
                        if (_targetStage[i] != null)
                        {
                            PushUpLine(_targetStage[i], _targetStage);
                        }
                    }
                }
            }
        }

        AdjustBoxPositions(_targetStage, _textBoxScript.isLeft);
    }

    void PushUpLine(TextMeshProUGUI _textBox, TextMeshProUGUI[] _targetStage)
    {
        TextBox _targetTextBoxScript = _textBox.gameObject.GetComponent<TextBox>();
        int _targetIndex = _targetTextBoxScript.currentIndexOnStage + 1;

        if (_targetIndex >= _targetStage.Length)
        {
            //Generate a fake text and move it up
            GameObject _fakeText = Instantiate(_textBox.gameObject, _textBox.rectTransform.anchoredPosition, Quaternion.identity);
            _fakeText.GetComponent<TextBox>().targetPosition = Vector2.up * 100 + _textBox.rectTransform.anchoredPosition;
            _fakeText.GetComponent<TextBox>().targetTransparency = 0;
            StartCoroutine(DestroyFakeText(_fakeText));
            ResetTextBox(_textBox);
            _targetStage[_targetIndex - 1] = null;
            return;
        }

        _targetStage[_targetIndex] = _textBox;
        _targetTextBoxScript.currentIndexOnStage = _targetIndex;

        if (_targetIndex == _targetStage.Length - 1)
        {
            _targetTextBoxScript.targetPosition = Vector2.up * 50 + _textBox.rectTransform.anchoredPosition;
        } else
        {
            _targetTextBoxScript.targetPosition = (_targetStage[_targetIndex + 1] != null)
                ? _targetStage[_targetIndex + 1].rectTransform.anchoredPosition + Vector2.down * (_targetStage[_targetIndex + 1].rectTransform.sizeDelta.y - m_lineSpace)
                : Vector2.up * 50 + _textBox.rectTransform.anchoredPosition;
        }

        _targetStage[_targetIndex - 1] = null;
    }

    //Creating new line, only do this after 
    //_textBox that's being pushed into should be the first on the waiting line
    void NewLine(TextMeshProUGUI[] _targetStage, List<TextMeshProUGUI> _fromWaitLine, bool _isSelection, string _text)
    {
        for (int i = _targetStage.Length - 1; i >= 0; i--)
        {
            if (_targetStage[i] != null)
            {
                PushUpLine(_targetStage[i], _targetStage);
            }
        }

        //Add the text box to the stage, and delete it from waiting line
        TextMeshProUGUI _textBox = _fromWaitLine[0];
        _targetStage[0] = _textBox;
        _fromWaitLine.Remove(_textBox);

        //Set text
        _textBox.text = _text;

        //Set target position
        TextBox _targetTextBoxScript = _textBox.gameObject.GetComponent<TextBox>();

        //Set if this is a selection
        if (_isSelection)
        {
            _textBox.gameObject.GetComponent<Button>().enabled = true;
            _targetTextBoxScript.isSelection = true;
        }

        _targetTextBoxScript.targetPosition = (_targetTextBoxScript.isLeft) ? m_leftFirstPos : m_rightFirstPos;

        //Update stage index for text box
        _targetTextBoxScript.currentIndexOnStage = 0;

        //Re adjust the other boxes positions
        AdjustBoxPositions(_targetStage, _targetTextBoxScript.isLeft);
    }

    void AdjustBoxPositions(TextMeshProUGUI[] _targetStage, bool _isLeft)
    {
        Vector2 _xPosition = ((_isLeft) ? m_leftFirstPos : m_rightFirstPos).x * Vector2.right;

        for (int i = 1; i < _targetStage.Length; i++)
        {
            if (_targetStage[i] != null && (_targetStage[i-1] != null)){
                _targetStage[i].GetComponent<TextBox>().targetPosition = (Vector2) _xPosition
                    + Vector2.up * (_targetStage[i - 1].GetComponent<TextBox>().targetPosition.y +  
                    + _targetStage[i].rectTransform.sizeDelta.y + m_lineSpace);
                if (i == 2)
                {
                    Debug.Log(_targetStage[i].GetComponent<TextBox>().targetPosition);
                }

            }
        }
    }

    void ResetTextBox(TextMeshProUGUI _textBox)
    {
        //Add the text box to corresponding wait list
        if (_textBox.GetComponent<TextBox>().isLeft)
        {
            m_textWaitLeft.Add(_textBox);
        }
        else
        {
            m_textWaitRight.Add(_textBox);
        }

        _textBox.text = "Placeholder";
        _textBox.GetComponent<Button>().enabled = false;

        TextBox _textBoxScript = _textBox.GetComponent<TextBox>();
        _textBoxScript.isSelection = false;
        _textBoxScript.clicked = false;
        _textBoxScript.currentIndexOnStage = -1;
        _textBox.rectTransform.anchoredPosition = (_textBoxScript.isLeft) ? (m_leftFirstPos + Vector2.left * 300): (m_rightFirstPos + Vector2.right * 300);
        _textBoxScript.targetPosition = _textBox.rectTransform.anchoredPosition;
    }

    IEnumerator DestroyFakeText(GameObject _toDestroy)
    {
        yield return new WaitForSeconds(textColorTransitionTime);
        Destroy(_toDestroy);
    }
}
