using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    public List<SubPhase> subPhaseList = new List<SubPhase>();
    public SubPhase currentPhase;

    GameManager m_gm;

    void Awake()
    {
        for (int i = 0; i < subPhaseList.Count; i++)
        {
            subPhaseList[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        m_gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPhase != null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartPhase(0);
        }

    }

    public void StartPhase(int _i)
    {
        if (currentPhase != null)
        {
            currentPhase.gameObject.SetActive(false);
        }
        currentPhase = subPhaseList[_i];
        subPhaseList[_i].gameObject.SetActive(true);
    }

    public void ClearStage()
    {
        m_gm.ClearUp();
    }
}