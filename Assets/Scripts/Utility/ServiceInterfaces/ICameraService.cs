using UnityEngine;

namespace ResonantSpark
{
    namespace Service
    {
        public interface ICameraService
        {
            void CameraShake(float duration, float magnitude);
        }
    }
}