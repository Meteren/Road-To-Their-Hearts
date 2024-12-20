using Cinemachine;
using UnityEngine;

public class WideCam : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera wideCam => 
        GameObject.Find("VCamera").GetComponent<CinemachineVirtualCamera>();
    public PlayerController controller => 
        GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null;
    CinemachineBasicMultiChannelPerlin wideChannel;
    void Start()
    {
        if(wideCam != null)
        {
            wideChannel = wideCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            GameManager.instance.blackBoard.SetValue("WideChannel", wideChannel);
        } 
        
    }

    private void Update()
    {
        if (controller.isDead)
        {
            if(wideCam is not null)
            {
                wideChannel.m_AmplitudeGain = 0f;
            }   
        }
    }

}
