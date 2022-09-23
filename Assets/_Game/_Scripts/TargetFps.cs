using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFps : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        // Make the game run as fast as possible
        Application.targetFrameRate = 60;
#endif
    }
}
