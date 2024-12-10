using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Spell
{
    private void Start()
    {
        damage = 8f;
    }

    private void Update()
    {
        DestroySpell("spellFourAnim");
        DestroySpell("spellFiveAnim");
    }
    public override void CastSpell(PlayerController controller)
    {
        if(gameObject.layer == LayerMask.NameToLayer("RedDragon"))
        {
            damageDirection = new Vector2(1, 1);
        }

        if(gameObject.layer == LayerMask.NameToLayer("BlueDragon"))
        {
            damageDirection = new Vector2(-1, 1);
        }

        SpellEffect(controller, damageDirection);
    }

    public override float InflictDamage()
    {
        return damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            CastSpell(controller);
        }
    }
}
