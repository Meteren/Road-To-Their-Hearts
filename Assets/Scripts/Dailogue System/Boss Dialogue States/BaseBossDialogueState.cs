using System.Collections;
using UnityEngine;
using AdvancedStateHandling;
using TMPro;

public abstract class BaseBossDialogueState : IState
{
    protected Boss boss;
    protected GameObject panel;
    protected TextMeshProUGUI dialogue;
    protected int currentDialogueIndex = 0;
    private bool isTyping;
    private float typeSpeed = 0.03f;
    private bool passNext = false;
    protected BaseBossDialogueState(Boss boss, GameObject panel)
    {
        this.boss = boss;
        this.panel = panel;
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

