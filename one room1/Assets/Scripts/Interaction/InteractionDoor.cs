using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDoor : MonoBehaviour
{
    [SerializeField] bool isOnlyView;
    [SerializeField] string sceneName;
    [SerializeField] string locationName;

    public string GetSceneName()
    {
        CameraController.onlyView = isOnlyView;
        return sceneName;
    }
   
    public string GetLocationName() 
    {
        return locationName;
    }
}
