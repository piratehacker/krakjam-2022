using StarterAssets;
using System;
using Cinemachine;
using Player;
using UI;
using UnityEngine;
using WorldChange;

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
    public PlayerProperty jumpHeight;
    public PlayerProperty slideSpeed;
    public PlayerProperty strengthPush;
    public PlayerProperty cameraNoiseAmplitude;
    public PlayerProperty cameraNoiseFrequency;
}


public class ChangePlayerProperties : WorldChangeLogic
{
    [SerializeField] private WorldTypeDict<PlayerPropertiesData> data;

    private PlayerPropertiesData currentProps;

    private FirstPersonController fpsc;
    private CinemachineBasicMultiChannelPerlin noise;
    private BasicRigidBodyPush push;

    protected override void Start()
    {
        fpsc = GetComponent<FirstPersonController>();
        push = GetComponent<BasicRigidBodyPush>(); 
        noise = CinemachineVCamInstance.Current.Cam?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        base.Start();
    }

    public override void OnWorldTypeChange(WorldTypeController.WorldType type)
    {
        currentProps = data[type];

        if (push != null)
        {
            push.strength = currentProps.strengthPush.value;
        }

        if (fpsc != null)
        {
            fpsc.MoveSpeed = currentProps.moveSpeed.value;
            fpsc.SprintSpeed = currentProps.sprintSpeed.value;
            fpsc.SlideSpeed = currentProps.slideSpeed.value;
            fpsc.JumpHeight = currentProps.jumpHeight.value;
        }

        SetNoiseIfNull();
        if (noise != null)
        {
            noise.m_AmplitudeGain = currentProps.cameraNoiseAmplitude.value;
            noise.m_FrequencyGain = currentProps.cameraNoiseFrequency.value;
        }
    }

    private bool corpoObjSet = false;
    private void Update()
    {
        SetNoiseIfNull();
        
        var props = currentProps;

        push.strength = props.strengthPush.ChangeOverTime(push.strength);

        fpsc.MoveSpeed = props.moveSpeed.ChangeOverTime(fpsc.MoveSpeed);
        fpsc.SprintSpeed = props.sprintSpeed.ChangeOverTime(fpsc.SprintSpeed);
        fpsc.SlideSpeed = props.slideSpeed.ChangeOverTime(fpsc.SlideSpeed);
        fpsc.JumpHeight = props.jumpHeight.ChangeOverTime(fpsc.JumpHeight);

        var prc = 1 - Math.Abs(fpsc.MoveSpeed - props.moveSpeed.edgeValue) /
            Math.Abs(props.moveSpeed.value - props.moveSpeed.edgeValue);
        
        PercentageOverlay.Get(OverlayType.Corpo).UpdateAmount(prc);

        if (Math.Abs(prc - 1) < 0.01 && !corpoObjSet)
        {
            ObjectivesUI.Current.SetObjective("CORPO POISON IS TAKING OVER", "FIND SOME PILLS TO BEHAVE NORMALLY");
            corpoObjSet = true;
        }
        else if(!corpoObjSet)
        {
            corpoObjSet = false;
        }

        if (noise != null)
        {
            noise.m_AmplitudeGain = props.cameraNoiseAmplitude.ChangeOverTime(noise.m_AmplitudeGain);
            noise.m_FrequencyGain = props.cameraNoiseFrequency.ChangeOverTime(noise.m_FrequencyGain);
        }
    }

    private void SetNoiseIfNull()
    {
        if (noise == null)
        {
            noise = CinemachineVCamInstance.Current.Cam?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }
}
