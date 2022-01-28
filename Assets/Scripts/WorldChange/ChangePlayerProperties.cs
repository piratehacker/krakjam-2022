﻿using StarterAssets;
using System;
using Cinemachine;
using Player;
using UnityEngine;
using WorldChange;
using System.Collections;

[Serializable]
public class PlayerProperty
{
    public float value;
    public float changeOverTime;
    public float edgeValue;

    public float ChangeOverTime(float original)
    {
        if (changeOverTime == 0) return original;
        
        return edgeValue > value
            ? Math.Min(edgeValue, original + ((changeOverTime / 10) * Time.deltaTime))
            : Math.Max(edgeValue, original + ((changeOverTime / 10) * Time.deltaTime));
    }
}

[Serializable]
public class PlayerPropertiesData
{
    public PlayerProperty moveSpeed;
    public PlayerProperty sprintSpeed;
    public PlayerProperty cameraNoiseAmplitude;
    public PlayerProperty cameraNoiseFrequency;
}


public class ChangePlayerProperties : WorldChangeLogic
{
    [SerializeField] private WorldTypeDict<PlayerPropertiesData> data;

    private PlayerPropertiesData currentProps;

    private FirstPersonController fpsc;
    private CinemachineBasicMultiChannelPerlin noise;

    protected override void Start()
    {


        base.Start();
        StartCoroutine(WaitTest());
    }

    private IEnumerator WaitTest()
    {
        yield return null;
        fpsc = GetComponent<FirstPersonController>();
        noise = CinemachineVCamInstance.Current.Cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public override void OnWorldTypeChange(WorldTypeController.WorldType type)
    {
        currentProps = data[type];
        
        // build error fix
        if (fpsc == null)
        {
            return;
        }
        fpsc.MoveSpeed = currentProps.moveSpeed.value;
        fpsc.SprintSpeed = currentProps.sprintSpeed.value;
        if (noise == null)
        {
            return;
        }
        noise.m_AmplitudeGain = currentProps.cameraNoiseAmplitude.value;
        noise.m_FrequencyGain = currentProps.cameraNoiseFrequency.value;
    }

    private void Update()
    {
        if (noise == null)
        {
            return;
        }
        
        var props = currentProps;
        
        // todo move speed changing doesn't work
        fpsc.MoveSpeed = props.moveSpeed.ChangeOverTime(fpsc.MoveSpeed);
        fpsc.SprintSpeed = props.sprintSpeed.ChangeOverTime(fpsc.SprintSpeed);

        noise.m_AmplitudeGain = props.cameraNoiseAmplitude.ChangeOverTime(noise.m_AmplitudeGain);
        noise.m_FrequencyGain = props.cameraNoiseFrequency.ChangeOverTime(noise.m_FrequencyGain);
    }
}
