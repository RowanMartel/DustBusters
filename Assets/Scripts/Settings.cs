using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    // CONSTANTS
    public const int int_playerHealthMax = 3;
    public const int int_playerSpeed = 250;
    public const int int_playerJumpForce = 75;
    public const int int_SFXCullingDist = 5;

    public const float flt_menuTransitionSpeed = 0.5f;

    // NON-CONSTANTS
    public static float flt_volume = 0.5f;
    public static float flt_lookSensitivity = 2f;
}
