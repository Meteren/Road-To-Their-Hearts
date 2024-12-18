using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    [SerializeField] private GameObject key;

    private void Awake()
    {
        instance = this;    
    }
    public void ActivateKey()
    {
        key.SetActive(true); 
    }

    public void DiasbleKey()
    {
        key.SetActive(false);
    }
}
