using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public CinemachineConfiner confiner;

    private void Start()
    {
        GetComponent<CinemachineVirtualCamera>().Follow = GameObject.Find("Player").transform;
    }

}
