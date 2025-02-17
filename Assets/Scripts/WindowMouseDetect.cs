﻿using UnityEngine;
using System.Collections;
using UnityEditor;

public class WindowMouseDetect : MonoBehaviour {

    private void Update() {
        GameVariable.mouseInWindow = MouseScreenCheck();
    }

    public bool MouseScreenCheck() {
#if UNITY_EDITOR
        if ( Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Handles.GetMainGameViewSize().x - 1 || Input.mousePosition.y >= Handles.GetMainGameViewSize().y - 1 ) {
            return false;
        }
#else
        if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1) {
            return false;
        }
#endif
        else {
            return true;
        }
    }
}