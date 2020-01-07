using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ResonantSpark {
    [RequireComponent(typeof(GameTimeManager))]
    public class FrameEnforcer : MonoBehaviour {

        public Text fpsCounter;
        private int updateCounter = 0;
        private int fixedUpdateCounter = 0;
        private float elapsedRealTime = 0.0f;

        private List<Action<int>> updateActions = new List<Action<int>>();
        private GameTimeManager gameTime;

        private const float FRAME_TIME = 1f / 60.0f; // 1 sec over 60 frames
        private float elapsedTime = 0f;

        private int frameIndex = 0;

        public void Start() {
            gameTime = gameObject.GetComponent<GameTimeManager>();
            elapsedTime = FRAME_TIME;
            frameIndex = 0;
        }

        public int index {
            get {
                if (this.enabled) return frameIndex;
                else return -1;
            }
        }

        public void FixedUpdate() {
            int stepsInFrame = 0;

                // TODO: This may be incorrect. I may need to pull this while loop out into an async call
            while (elapsedTime > FRAME_TIME) {
                foreach (Action<int> action in updateActions) {
                    action.Invoke(frameIndex);
                }

                stepsInFrame++;
                updateCounter++;

                frameIndex++;
                elapsedTime -= FRAME_TIME;
            }

            //if (stepsInFrame > 1) {
            //    Debug.LogWarning("Frame Skip at frame(" + frameIndex + "). Stepped " + stepsInFrame + " times in single frame");
            //}

            elapsedTime += gameTime.Layer("realTime");
            elapsedRealTime = Time.time;

            if (fixedUpdateCounter >= 45) {
                fpsCounter.text = (updateCounter / elapsedRealTime).ToString("F1") + " FPS";
                fixedUpdateCounter = 0;
            }
            fixedUpdateCounter++;
        }

        public void SetUpdate(Action<int> updateAction) {
            this.updateActions.Add(updateAction);

            if (!this.enabled) {
                StartCoroutine(TriggerEndOfFrame());
            }

            this.enabled = true;
        }

        private IEnumerator TriggerEndOfFrame() {
            yield return new WaitForEndOfFrame();
            EventManager.TriggerEvent<Events.FrameEnforcerReady>();
        }
    }
}