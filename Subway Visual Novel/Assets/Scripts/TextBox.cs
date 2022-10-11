using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    [HideInInspector] public Vector2 targetPosition;
    [HideInInspector] public float targetTransparency = 1;
    [HideInInspector] public bool isSelection = false;
    public bool isLeft = true;
    public int currentIndexOnStage = -1;

    float m_movePosSmoothTime;
    Vector2 m_movePosSmoothV;

    float m_colorSmoothTime;
    float m_colorSmoothVelocity;
    [HideInInspector] public Color startColor;

    TextMeshProUGUI m_tmpro;
    Button m_button;
    [HideInInspector] public bool clicked = false;

    void Start()
    {
        m_tmpro = GetComponent<TextMeshProUGUI>();
        m_button = GetComponent<Button>();

        m_movePosSmoothTime = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().textBoxMoveSmoothTime;
        m_colorSmoothTime = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().textColorTransitionTime;

        startColor = m_tmpro.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_tmpro.rectTransform.anchoredPosition != targetPosition)
        {
            m_tmpro.rectTransform.anchoredPosition = Vector2.SmoothDamp(m_tmpro.rectTransform.anchoredPosition, targetPosition, ref m_movePosSmoothV, m_movePosSmoothTime);
        }

        /*if (m_tmpro.color.a != targetTransparency)
        {
            m_tempColor.a = Mathf.SmoothDamp(m_tmpro.color.a, targetTransparency, ref m_colorSmoothVelocity, m_colorSmoothTime);
            m_tmpro.color = m_tempColor;
        }*/
    }

    public void OnSelectionClick()
    {
        if (clicked)
        {
            return;
        }

        clicked = true;

        GameManager _gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        if (isLeft)
        {
            _gm.DeleteSelection(m_tmpro, _gm.m_textStageLeft);
        } else
        {
            _gm.DeleteSelection(m_tmpro, _gm.m_textStageRight);
        }
    }
}
