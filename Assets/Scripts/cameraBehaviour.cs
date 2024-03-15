using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

using controller;

public class cameraBehaviour : MonoBehaviour
{

    public newPlayerMovement pm;
    public CinemachineVirtualCamera virtualCamera;
    // Update is called once per frame
    void Update()
    {
        if(pm.camReset == true)
        {
            //delta = pm._respawnPoint - pm.player.transform.position;
            //virtualCamera.OnTargetObjectWarped(pm.player.transform, delta);
            virtualCamera.PreviousStateIsValid = false;
            transform.position = pm._respawnPoint;
            pm.camReset = false;
        }
    }
}
