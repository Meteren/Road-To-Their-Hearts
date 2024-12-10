using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossAvatarChecker : MonoBehaviour
{
    [SerializeField] private Image avatar;
    public void HideAvatar()
    {
        Color avatarColor = avatar.color;
        avatarColor.a = 0f;
        avatar.color = avatarColor;
    }
}
