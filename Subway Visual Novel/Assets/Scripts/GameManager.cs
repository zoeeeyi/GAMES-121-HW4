using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject m_uiCanvas;
    [SerializeField] List<TextMeshProUGUI> m_textBoxList = new List<TextMeshProUGUI>();
    [DisableInEditorMode] public List<TextMeshProUGUI> textWaitLeft = new List<TextMeshProUGUI>();
    [DisableInEditorMode] public List<TextMeshProUGUI> textWaitRight = new List<TextMeshProUGUI>();
    [DisableInEditorMode] public TextMeshProUGUI[] textStageLeft;
    [DisableInEditorMode] public TextMeshProUGUI[] textStageRight;

    [SerializeField] Vector2 m_leftFirstPos;
    [SerializeField] Vector2 m_rightFirstPos;

    [SerializeField] float m_lineSpace;
    public float textBoxMoveSmoothTime;
    public float textDestroyTime;

    void Start()
    {
        //Reset all text boxes
        for (int i = 0; i < m_textBoxList.Count; i++)
        {
            //Reset the text box
            ResetTextBox(m_textBoxList[i]);
        }

        //Set up on stage arrays
        textStageLeft = new TextMeshProUGUI[textWaitLeft.Count];
        textStageRight = new TextMeshProUGUI[textWaitRight.Count];
    }

    void Update()
    {
        AdjustBoxPositions(textStageLeft, true);

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ClearUp();
        }

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

        _textBoxScript.isSelection = false;
        AdjustBoxPositions(_targetStage, _textBoxScript.isLeft);
    }

    public void PushUpLine(TextMeshProUGUI _textBox, TextMeshProUGUI[] _targetStage)
    {
        TextBox _targetTextBoxScript = _textBox.gameObject.GetComponent<TextBox>();
        int _targetIndex = _targetTextBoxScript.currentIndexOnStage + 1;

        if (_targetIndex >= _targetStage.Length)
        {
            //Generate a fake text and move it up
            ClearText(_textBox);
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
    public void NewLine(TextMeshProUGUI[] _targetStage, List<TextMeshProUGUI> _fromWaitLine, bool _isSelection, string _text, int _nextLineIndex)
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

        //Set next line index
        _targetTextBoxScript.nextLineIndex = _nextLineIndex;

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

    public void AdjustBoxPositions(TextMeshProUGUI[] _targetStage, bool _isLeft)
    {
        Vector2 _xPosition = ((_isLeft) ? m_leftFirstPos : m_rightFirstPos).x * Vector2.right;

        for (int i = 1; i < _targetStage.Length; i++)
        {
            if (_targetStage[i] != null)
            {
                int _goDownSteps = 0;

                for (int j = 1; i - j >= 0; j++)
                {
                    if (_targetStage[i - j] != null)
                    {
                        _goDownSteps = j;
                        break;
                    }
                }

                if (_goDownSteps != 0)
                {
                    _targetStage[i].GetComponent<TextBox>().targetPosition = (Vector2)_xPosition
                        + Vector2.up * (_targetStage[i - _goDownSteps].GetComponent<TextBox>().targetPosition.y +
                        +_targetStage[i].rectTransform.sizeDelta.y + m_lineSpace);
                } else
                {
                    _targetStage[i].GetComponent<TextBox>().targetPosition = ((_isLeft) ? m_leftFirstPos : m_rightFirstPos)
                        + Vector2.up * (_targetStage[i].rectTransform.sizeDelta.y + m_lineSpace);
                }
            }
        }
    }

    public void ClearUp()
    {
        for (int i = 0; i<textStageLeft.Length; i++)
        {
            if (textStageLeft[i] != null)
            {
                ClearText(textStageLeft[i]);
                textStageLeft[i] = null;
            }
        }

        for (int i = 0; i < textStageRight.Length; i++)
        {
            if (textStageRight[i] != null)
            {
                ClearText(textStageRight[i]);
                textStageRight[i] = null;
            }
        }
    }

    public void ClearText(TextMeshProUGUI _t)
    {
        //Generate a fake text and move it up
        GameObject _fakeText = Instantiate(_t.gameObject, _t.transform.position, Quaternion.identity, m_uiCanvas.transform);
        _fakeText.GetComponent<TextMeshProUGUI>().rectTransform.anchoredPosition = _t.rectTransform.anchoredPosition;
        _fakeText.GetComponent<TextBox>().targetPosition = Vector2.up * 100 + _t.rectTransform.anchoredPosition;
        _fakeText.GetComponent<TextBox>().thisIsFake = true;
        StartCoroutine(DestroyFakeText(_fakeText));
        ResetTextBox(_t);
        return;
    }

    public void ResetTextBox(TextMeshProUGUI _textBox)
    {
        //Add the text box to corresponding wait list
        if (_textBox.GetComponent<TextBox>().isLeft)
        {
            textWaitLeft.Add(_textBox);
        }
        else
        {
            textWaitRight.Add(_textBox);
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
        yield return new WaitForSeconds(textDestroyTime);
        Destroy(_toDestroy);
    }
}
