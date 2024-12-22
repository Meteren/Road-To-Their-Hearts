using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedStateHandling;
using TMPro;
using System;

public abstract class BaseBossDialogueState : IState
{
    protected Boss boss;
    protected GameObject panel;
    protected TextMeshProUGUI dialogue;
    protected int currentDialogueIndex = 0;
    private bool isTyping;
    private float typeSpeed = 0.03f;
    private bool passNext = false;

    protected Dictionary<Type, int> bossMusics;

    protected BaseBossDialogueState(Boss boss, GameObject panel)
    {
        this.boss = boss;
        this.panel = panel;
        bossMusics = new Dictionary<Type, int>()
        {
            { typeof(BossX), boss.MusicIndex() },
            { typeof(BossY), boss.MusicIndex() },
            { typeof(BossZ), boss.MusicIndex() }
        };
    }

    public virtual void OnStart()
    {
        this.dialogue = panel.GetComponentInChildren<TextMeshProUGUI>();
        panel.SetActive(true);
 
    }

    public virtual void OnExit()
    {
        return;
    }

    public virtual void Update()
    {
        return;
    }

    //To use in need of flexibility
    protected virtual void DialogBoxController()
    {
        return;
    }


    protected void PlayBossMusic()
    {
        if(bossMusics.TryGetValue(boss.GetType(),out int musicIndex))
        {
            GameManager.instance.audioManager.PlayBossMusic(musicIndex);
        }
    }

    protected void StopBossMusic()
    {
        if (bossMusics.TryGetValue(boss.GetType(), out int musicIndex))
        {
            boss.StartCoroutine(DecreaseVolume(GameManager.instance.audioManager.BossMusicAudioSource(musicIndex),musicIndex));
        }
    }

    private IEnumerator DecreaseVolume(AudioSource source,int musicIndex)
    {
        for(float i = source.volume;  i >= 0; i-= Time.deltaTime)
        {
            yield return null;
            source.volume = i;

        }
        GameManager.instance.audioManager.StopBossMusic(musicIndex);
    }
    protected void DialogBoxController(DialogueContainer dialogueParam, out bool isReady,bool isInControl)
    {
        
        if (!isInControl && !isTyping)
        {
            if (!passNext)
            {
                Debug.Log("ExitedContainer");
                isReady = true;
                return;
            }

            currentDialogueIndex++;
            if (currentDialogueIndex >= dialogueParam.dialogues.Length)
            {
                Debug.Log("Exited");
                isReady = false;
                currentDialogueIndex = 0;
                passNext = false;
                return;
            }
            dialogue.maxVisibleCharacters = 0;
            dialogue.text = dialogueParam.dialogues[currentDialogueIndex];
            GameManager.instance.StartCoroutine(WriteTextOneByOne(dialogue));
        }

        if (Input.GetKeyDown(KeyCode.E) && !isTyping && isInControl)
        {

            currentDialogueIndex++;

            if (currentDialogueIndex >= dialogueParam.dialogues.Length)
            {
                Debug.Log("Exited");
                isReady = false;
                currentDialogueIndex = 0;
                return;
            }
            dialogue.maxVisibleCharacters = 0;
            dialogue.text = dialogueParam.dialogues[currentDialogueIndex];
            GameManager.instance.StartCoroutine(WriteTextOneByOne(dialogue));

        }

        isReady = true;

    }

    protected IEnumerator WriteTextOneByOne(TextMeshProUGUI text)
    {
        passNext = false;
        isTyping = true;
        while (dialogue.text.Length != text.maxVisibleCharacters)
        {
            yield return new WaitForSeconds(typeSpeed);
            GameManager.instance.audioManager.PlaySFX(4);
            text.maxVisibleCharacters++;
             
        }
        GameManager.instance.StartCoroutine(WaitForNextText());
        isTyping = false;
    }

    protected IEnumerator WaitForNextText()
    {
        yield return new WaitForSeconds(1f);
        passNext = true;
    }
}

