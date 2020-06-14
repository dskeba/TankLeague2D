using Cinemachine;
using UnityEngine;

namespace TankGame
{
    public class VirtualCamera : MonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;
        public float shakeDuration = 0.3f;
        public float shakeAmplitude = 1.2f;
        public float shakeFrequency = 2.0f;

        private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
        private float ShakeElapsedTime = 0f;

        private void Start()
        {
            if (virtualCamera != null)
            {
                virtualCameraNoise = virtualCamera.GetCinemachineComponent
                    <Cinemachine.CinemachineBasicMultiChannelPerlin>();
            }
        }

        private void FixedUpdate()
        {
            if (virtualCamera != null && virtualCameraNoise != null)
            {
                if (ShakeElapsedTime > 0)
                {
                    virtualCameraNoise.m_AmplitudeGain = shakeAmplitude;
                    virtualCameraNoise.m_FrequencyGain = shakeFrequency;

                    ShakeElapsedTime -= Time.deltaTime;
                }
                else
                {
                    virtualCameraNoise.m_AmplitudeGain = 0f;
                    ShakeElapsedTime = 0f;
                }
            }
        }

        public void ShakeCamera()
        {
            ShakeElapsedTime = shakeDuration;
        }
    } 
}
