using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button mainMenu;

    private void Update()
    {
        AnimatorStateInfo animatorStateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if(animatorStateInfo.normalizedTime >= 1)
        {
            title.gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(true);
        }
    }
}
