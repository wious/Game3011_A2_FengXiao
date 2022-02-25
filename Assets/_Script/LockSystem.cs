using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum Difficulty
{
    EASY,
    MED,
    HARD,
    TOTAL
}
public class LockSystem : MonoBehaviour
{
    public Image TopFrame;
    public Image BottomFrame;
    public TextMeshProUGUI playerSkillLabel;
    public TextMeshProUGUI lockDifficultyLabel;
    public TextMeshProUGUI timerLabel;
    public GameObject SuccessLabel;
    public GameObject FailLabel;
    public GameObject TopPick;
    public GameObject BottomPick;
    public Difficulty lockDifficulty;
    public GameObject[] Number;
    public AudioSource audioSource;
    public AudioClip sweetSpotSound;
    public AudioClip unlockSound;

    public float timer = 1.0f;
    
    [SerializeField] private float topPickAngle;
    [SerializeField] private float bottomPickAngle;
    [SerializeField] private float bottomPickTurnRate = 0.001f;
    [SerializeField] private int requiredTumblers = 1;
    [SerializeField] private float playerSkill = 50.0f; 
    [SerializeField] private float _targetTopAngle;
    [SerializeField] private float _targetBottomAngle;
    [SerializeField] private float _targetBaseAngleThresholds = 0.500f;
    [SerializeField] private float _targetAngleThresholds;
    [SerializeField] private bool _topTargetReached = false;
    [SerializeField] private bool _bottomTargetReached = false;
    
    private bool topAudioPlayed = false;
    private bool bottomAudioPlayed = false;
    void Start()
    {
        InitLock();
    }
    
    void Update()
    {
        CalculatePickAngles();
        RotatePicks();
        CheckForTarget();
        CheckUnlockAttempt();
        CountDown();
    }

    void CalculatePickAngles()
    {
        Vector3 relative = transform.InverseTransformPoint(Input.mousePosition);
        float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        topPickAngle = angle; 

        if (Input.GetKey(KeyCode.A))
        {
            bottomPickAngle -= bottomPickTurnRate; 
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            bottomPickAngle += bottomPickTurnRate; 
        }

        if (bottomPickAngle >= 360)
        {
            bottomPickAngle = 0; 
        }
        
        if (bottomPickAngle < 0)
        {
            bottomPickAngle = 360; 
        }
    }

    void CheckForTarget()
    {
        if (requiredTumblers <= 0) return;
        _topTargetReached = Math.Abs(topPickAngle - _targetTopAngle) < _targetAngleThresholds;
        _bottomTargetReached = Math.Abs(bottomPickAngle - _targetBottomAngle) < _targetAngleThresholds;
        TopFrame.color = _topTargetReached ? Color.green : Color.black;
        BottomFrame.color = _bottomTargetReached ? Color.green : Color.black;
        if (_topTargetReached)
        {
            if(!topAudioPlayed) audioSource.PlayOneShot(sweetSpotSound);
            topAudioPlayed = true;
        }
        else
        {
            topAudioPlayed = false;
        }

        if (_bottomTargetReached)
        {
            if(!bottomAudioPlayed) audioSource.PlayOneShot(sweetSpotSound);
            bottomAudioPlayed = true;
        }
        else
        {
            bottomAudioPlayed = false;
        }
    }

    void RotatePicks()
    {
        if (requiredTumblers <= 0) return;
        TopPick.transform.eulerAngles = new Vector3(0, 0, topPickAngle);
        BottomPick.transform.eulerAngles = new Vector3(0,0,bottomPickAngle);
    }
    
    void ResetPicks()
    {
        TopPick.transform.eulerAngles = new Vector3(0, 0, 0);
        BottomPick.transform.eulerAngles = new Vector3(0,0,0);
    }

    public void InitLock()
    {
        _targetTopAngle = 360.0f * Random.Range(0.0f, 1.0f);
        _targetBottomAngle = 360.0f * Random.Range(0.0f, 1.0f);
        lockDifficulty = (Difficulty) Random.Range(0, (int) Difficulty.TOTAL);
        lockDifficultyLabel.text = lockDifficulty switch
        {
            Difficulty.EASY => "Easy",
            Difficulty.MED => "Medium",
            Difficulty.HARD => "Hard",
            _ => lockDifficultyLabel.text
        };
        requiredTumblers = (int)lockDifficulty + 1;
        playerSkill = Random.Range(1, 101);
        playerSkillLabel.text = "Player Skill: "+playerSkill.ToString();
        _targetAngleThresholds = _targetBaseAngleThresholds * (1 + playerSkill / 100.0f); 
        DisplayTumblers();
    }

    void DisplayTumblers()
    {
        for (int i = 0; i < requiredTumblers; i++)
        {
            Number[i].SetActive(true);
        }
    }
    
    void ResetTumblers()
    {
        for (int i = 0; i < requiredTumblers; i++)
        {
            Number[i].SetActive(false);
        }
    }

    private void CheckUnlockAttempt()
    {
        if (requiredTumblers <= 0) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (_topTargetReached && _bottomTargetReached)
            {
                requiredTumblers--;
                Number[requiredTumblers].SetActive(false);
                _targetTopAngle = 360.0f * Random.Range(0.0f, 1.0f);
                _targetBottomAngle = 360.0f * Random.Range(0.0f, 1.0f);
                audioSource.PlayOneShot(unlockSound);
                if (requiredTumblers == 0)
                {
                    SuccessLabel.SetActive(true);
                }
            }
        }
    }

    public void ResetLock()
    {
        ResetPicks();
        ResetTumblers();
        SuccessLabel.SetActive(false);
        FailLabel.SetActive(false);
        timer = 60;
        InitLock();
    }

    public void CountDown()
    {
        if (requiredTumblers <= 0) return;
        timer -= Time.deltaTime;
        int roundedTime = (int)timer;
        timerLabel.text = roundedTime.ToString() + "s";
        if (roundedTime <= 0)
        {
            FailLabel.SetActive(true);
            requiredTumblers = 0; 
        }
    }
    
    
}
