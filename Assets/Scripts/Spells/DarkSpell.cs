using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSpell : Spell
{
    private void Start()
    {
        damage = 2f;
    }
    private void Update()
    {
        DestroySpell("spell");
    }
    public override void CastSpell(PlayerController controller)
    {
        damageDirection = controller.isFacingRight ? new Vector2(1,1) : new Vector2 (-1,1);
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
