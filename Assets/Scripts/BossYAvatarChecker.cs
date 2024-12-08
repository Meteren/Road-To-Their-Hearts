using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossYAvatarChecker : MonoBehaviour
{
    [SerializeField] private Image avatar;
    public void HideAvatar()
    {
        Color avatarColor = avatar.color;
        avatarColor.a = 0f;
        avatar.color = avatarColor;
    }
}
