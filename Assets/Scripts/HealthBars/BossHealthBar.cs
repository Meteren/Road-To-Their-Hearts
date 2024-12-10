using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : HealthBar
{
    [SerializeField] private Boss boss;

    private void Update()
    {
        if(avatarAnimator is null)
        {
            return;
        }

        if (boss.canAvatarDie)
        {
            avatarAnimator.SetBool("isDead", boss.canAvatarDie);
        }

        if (boss.phaseTwo || boss.inPhaseTwo)
        {
            avatarAnimator.SetBool("passPhaseTwo", boss.phaseTwo);
            avatarAnimator.SetBool("phaseTwoIdle", boss.phaseTwoIdle);
        }
    }

    public void OnAvatarDie()
    {
        Color color = avatarOfHealthBar.color;

        color.a = 0;
        
        avatarOfHealthBar.color = color;

    }
}
