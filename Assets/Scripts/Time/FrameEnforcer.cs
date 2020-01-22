using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ResonantSpark {
    public enum FramePriority : int {
        ActivePollingReset = 1,
        Gamemode = 256,
        Service = 512,
        StateMachine = 1024,
        InputBuffer = 2048,

        UpdateInput = 9999999,
    }

    [RequireComponent(typeof(GameTimeManager))]
    public class FrameEnforcer : MonoBehaviour {

        private class FrameEnforcerCallback : IComparable<FrameEnforcerCallback> {
            public int priority;
            public Action<int> callback;

            public int CompareTo(FrameEnforcerCallback other) {
                return other.priority - this.priority;
            }
        }

        public Text fpsCounter;
        private int updateCounterSnapshot = 0;
        private float timeSnapshot = 0.0f;

        private int updateCounter = 0;

        private List<FrameEnforcerCallback> updateActions = new List<FrameEnforcerCallback>();
        private GameTimeManager gameTime;

        private const float FRAME_TIME = 1f / 60.0f; // 1 sec over 60 frames
        private float elapsedTime = 0f;

        private int frameIndex = 0;

        public void Awake() {
            this.enabled = false;
            elapsedTime = FRAME_TIME;
            frameIndex = 0;
        }

        public void Start() {
            gameTime = gameObject.GetComponent<GameTimeManager>();
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
                foreach (FrameEnforcerCallback action in updateActions) {
                    action.callback.Invoke(frameIndex);
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

            if (Time.time - timeSnapshot >= 0.45) {
                fpsCounter.text = ((updateCounter - updateCounterSnapshot) / (Time.time - timeSnapshot)).ToString("F1") + " FPS";
                timeSnapshot = Time.time;
                updateCounterSnapshot = updateCounter;
            }
        }

        public void AddUpdate(int priority, Action<int> updateAction) {
            var feCb = new FrameEnforcerCallback {
                priority = priority,
                callback = updateAction
            };

            this.updateActions.Add(feCb);
            this.updateActions.Sort();
        }

        public void StartFrameEnforcer() {
            this.enabled = true;
            EventManager.TriggerEvent<Events.FrameEnforcerReady>();
        }
    }
}