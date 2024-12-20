using AdvancedStateHandling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoDialogueState : BaseBossDialogueState
{
    
    public NoDialogueState(Boss boss,GameObject panel) : base(boss,panel)
    {
    }

    public override void OnStart()
    {
        //base.OnStart();
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
       
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
    }

}
public class FirstEncounterToSay : BaseBossDialogueState
{
    DialogueContainer firstEncounterToSay;

    public FirstEncounterToSay(Boss bossX, DialogueContainer firstEncounterToSay, GameObject panel) : base(bossX,panel)
    {
        this.firstEncounterToSay = firstEncounterToSay;
    }

    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = firstEncounterToSay.dialogues[currentDialogueIndex];
        boss.StartCoroutine(WriteTextOneByOne(dialogue));
    }
    public override void OnExit()
    {
        base.OnExit();
        boss.isVulnerable = false;
    }

    public override void Update()
    {
        base.Update();
        DialogBoxController(firstEncounterToSay,out bool isReady,firstEncounterToSay.isInControl);
        boss.firstEncounterReady = isReady;

    }

}


public class InSpecialOneToSay : BaseBossDialogueState
{
    DialogueContainer inSpecialOneToSay;
    public InSpecialOneToSay(Boss bossX, DialogueContainer inSpecialOneToSay, GameObject panel) : base(bossX, panel)
    {
        this.inSpecialOneToSay = inSpecialOneToSay;
    }
    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = inSpecialOneToSay.dialogues[currentDialogueIndex];
        boss.StartCoroutine(WriteTextOneByOne(dialogue));
        
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
        DialogBoxController(inSpecialOneToSay,out bool isReady,inSpecialOneToSay.isInControl);
        boss.inSpecialOneToSayIsReady = isReady;
    }
}

public class InSpecialTwoToSay : BaseBossDialogueState
{
    DialogueContainer inSpecialTwoToSay;
    public InSpecialTwoToSay(Boss bossX, DialogueContainer inSpecialTwoToSay, GameObject panel) : base(bossX, panel)
    {
        this.inSpecialTwoToSay = inSpecialTwoToSay;
    }

    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = inSpecialTwoToSay.dialogues[currentDialogueIndex];
        boss.StartCoroutine(WriteTextOneByOne(dialogue));
    }

    public override void OnExit()
    {
        base.OnExit();
    }


    public override void Update()
    {
        base.Update();
        DialogBoxController(inSpecialTwoToSay,out bool isReady,inSpecialTwoToSay.isInControl);
        boss.inSpecialTwoToSayIsReady = isReady;
    }

}

public class InDeathToSay : BaseBossDialogueState, IState
{
    DialogueContainer inDeathToSay;
    public InDeathToSay(DialogueContainer inDeathToSay, Boss bossX, GameObject panel) : base(bossX, panel)
    {
        this.inDeathToSay = inDeathToSay;
    }

    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = inDeathToSay.dialogues[currentDialogueIndex];
        GameManager.instance.StartCoroutine(WriteTextOneByOne(dialogue));

    }

    public override void OnExit()
    {
        base.OnExit();
        Portal portal = GameObject.Instantiate(boss.deathPortal, boss.portalSpawnPoint.transform.position, Quaternion.identity);
        portal.OnSpawn();
    }

    public override void Update()
    {
        base.Update();
        DialogBoxController(inDeathToSay, out bool isReady, inDeathToSay.isInControl);
        boss.inDeathToSayIsReady = isReady;
    }
}

public class InCharacterDeathToSay : BaseBossDialogueState, IState
{
    DialogueContainer inCharacterDeathToSay;
    public InCharacterDeathToSay(DialogueContainer inCharacterDeathToSay,Boss bossX, GameObject panel) : base(bossX, panel)
    {
        this.inCharacterDeathToSay = inCharacterDeathToSay;
    }


    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = inCharacterDeathToSay.dialogues[currentDialogueIndex];
        boss.StartCoroutine(WriteTextOneByOne(dialogue));
    }
    public override void OnExit()
    {
        base.OnExit();
        GameManager.instance.afterDeathUI.SetActive(true);
    }
    public override void Update()
    {
        base.Update();
        DialogBoxController(inCharacterDeathToSay, out bool isReady, inCharacterDeathToSay.isInControl);
        boss.inCharacterDeathToSayIsReady = isReady;

    }
}
