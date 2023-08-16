using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
 
public class OrientationManager : MonoBehaviour
{
 
    // This event will only be called when an orientation changed (i.e. won't be call at launch)
    public static event UnityAction<ScreenOrientation> orientationChangedEvent;
    public static event Action<ScreenOrientation> OnOrientationChange;

    [SerializeField] private bool _debugMode = false;
 
    public ScreenOrientation orientation;
 
    private void Start()
    {
        ToggleRefresh(true);
        Screen.autorotateToPortraitUpsideDown = false;
        orientation = Screen.orientation;
        InvokeRepeating("CheckForChange", 1, 1);
    }

   

    private static void OnOrientationChanged(ScreenOrientation orientation)
    {
        if (orientationChangedEvent != null)
            orientationChangedEvent(orientation);
    }
 
    private void CheckForChange()
    {
        if (_debugMode)
            Debug.Log("Screen.orientation=" + Screen.orientation);
        ToggleRefresh(true);
        if (orientation != Screen.orientation) {
            orientation = Screen.orientation;
            OnOrientationChange?.Invoke(orientation);
            ToggleRefresh(false);
            OnOrientationChanged(orientation);
        }
    }

    void ToggleRefresh(bool state)
    {
        Screen.autorotateToLandscapeLeft = state;
        Screen.autorotateToLandscapeRight = state;
        Screen.autorotateToPortrait = state;
    }

#if UNITY_EDITOR

    [ContextMenu("Print Orientation")]
    private void PrintOrientation()
    {
        Debug.Log(orientation);
    }
 
    [ContextMenu("Simulate Landscape Left")]
    private void SetLandscapeLeft()
    {
        OnOrientationChanged(ScreenOrientation.LandscapeLeft);
    }
 
    [UnityEditor.MenuItem("Debug/Orientation/Simulate Landscape Left")]
    private static void DoSetLandscapeLeft()
    {
        OnOrientationChanged(ScreenOrientation.LandscapeLeft);
    }
 
    [ContextMenu("Simulate Landscape Right")]
    private void SetLandscapeRight()
    {
        OnOrientationChanged(ScreenOrientation.LandscapeRight);
    }
 
    [UnityEditor.MenuItem("Debug/Orientation/Simulate Landscape Right")]
    private static void DoSetLandscapeRight()
    {
        OnOrientationChanged(ScreenOrientation.LandscapeRight);
    }
 
    #endif
 
}