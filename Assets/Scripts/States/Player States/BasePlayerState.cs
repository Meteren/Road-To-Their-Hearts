using AdvancedStateHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdvancedStateHandling
{
    public abstract class BasePlayerState : IState
    {
        protected float direction;
        private delegate void RollEvent();
        private event RollEvent checkRoll;
        protected float moveSpeed = 8f;
        public PlayerController controller { get; private set; }

        public BasePlayerState(PlayerController controller)
        {
            this.controller = controller;
            checkRoll += controller.CheckHighness;
        }

        public virtual void OnStart()
        {
            direction = controller.isFacingRight ? 1 : -1;
            return;
            
        }

        public virtual void OnExit()
        {
            return;
        }

        public virtual void Update()
        {
            Debug.Log("Base Update Called");
            if(SceneManager.GetActiveScene().buildIndex == 0)
            {
                controller.xAxis = 1;
            }
            else
            {
                controller.HandleMovement();
            }
            
        }


        public void CheckHighness() => checkRoll.Invoke();
 

    }
}
