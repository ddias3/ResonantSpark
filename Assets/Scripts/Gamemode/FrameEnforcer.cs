using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ResonantSpark {
    [RequireComponent(typeof(GameTimeManager))]
    public class FrameEnforcer : MonoBehaviour {

        private Action<int> updateAction = null;
        private GameTimeManager gameTime;

        private const float FRAME_TIME = 1f / 60.0f; // 1 sec over 60 frames
        private float elapsedTime = 0f;

        private int frameIndex = 0;

        public void Start() {
            gameTime = gameObject.GetComponent<GameTimeManager>();
            elapsedTime = 0.0f;
            frameIndex = 0;
        }

        public int index {
            get {
                if (this.enabled) return frameIndex;
                else return -1;
            }
        }

        public void Update() {
            while (elapsedTime > FRAME_TIME) {
                updateAction.Invoke(frameIndex);

                frameIndex++;
                elapsedTime -= FRAME_TIME;
            }

            elapsedTime += gameTime.Layer(0);
        }

        public void SetUpdate(Action<int> updateAction) {
            this.updateAction = updateAction;
            this.enabled = true;

            StartCoroutine(TriggerEndOfFrame());
        }

        private IEnumerator TriggerEndOfFrame() {
            yield return new WaitForEndOfFrame();
            EventManager.TriggerEvent<Events.FrameEnforcerReady>();
        }
    }
}