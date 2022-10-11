using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class SubPhase : MonoBehaviour
{
    public int subPhaseIndex;

    [TableList(ShowIndexLabels = true)]
    public List<SingleLine> m_singleLines = new List<SingleLine> ();

    int m_lineIndex = 0;
    public bool subPhaseStarted = false;

    float m_timer;

    GameManager m_gm;

    // Start is called before the first frame update
    void Start()
    {
        subPhaseStarted = true;
        m_timer = m_singleLines[0].linePushDelay;
        m_gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (subPhaseStarted)
        {
            if (m_timer > 0)
            {
                m_timer -= Time.deltaTime;
                return;
            }

            if (!m_singleLines[m_lineIndex].isSelection)
            {
                PushNewLine(m_singleLines[m_lineIndex]);

                //Push next line
                int _nextLineIndex;
                if (m_singleLines[m_lineIndex].nextLineIndex != -1)
                {
                    _nextLineIndex = m_singleLines[m_lineIndex].nextLineIndex;
                } else
                {
                    _nextLineIndex = m_lineIndex + 1;
                }

                JumpToLine(_nextLineIndex);

            } else // If next line is selection, we give selections all at once
            {
                PushNewLine(m_singleLines[m_lineIndex]);
                m_lineIndex++;

                if (m_lineIndex >= m_singleLines.Count)
                {
                    EndPhase();
                    return;
                }

                if (!m_singleLines[m_lineIndex].isSelection)
                {
                    subPhaseStarted = false;
                }

                /*while (true)
                {
                    PushNewLine(m_singleLines[m_lineIndex]);
                    m_lineIndex++;

                    if (m_lineIndex >= m_singleLines.Count)
                    {
                        EndPhase();
                        return;
                    }

                    if (!m_singleLines[m_lineIndex].isSelection)
                    {
                        subPhaseStarted = false;
                        break;
                    }
                }*/
            }
        }
    }

    public void JumpToLine(int _i)
    {
        if (_i < 0 || _i >= m_singleLines.Count)
        {
            EndPhase();
            return;
        }

        m_lineIndex = _i;
        m_timer = m_singleLines[_i].linePushDelay;
        subPhaseStarted = true;
    }

    void PushNewLine(SingleLine _line)
    {
        m_gm.NewLine((_line.isLeft) ? m_gm.textStageLeft : m_gm.textStageRight,
            (_line.isLeft) ? m_gm.textWaitLeft : m_gm.textWaitRight, _line.isSelection,
            _line.dialog, _line.nextLineIndex);

    }

    void EndPhase()
    {
        subPhaseStarted = false;
    }
}

[Serializable]
public class SingleLine
{
    public string dialog;
    public bool isLeft;
    public bool isSelection;
    public int nextLineIndex = -1;
    public float linePushDelay;
}
