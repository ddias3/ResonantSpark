using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ResonantSpark {
    public enum FramePriority : int {
        InputBuffer = 8,

        StateMachine = 64,
        StateMachinePass1 = 64 + 1,

        PhysicsMovement = 128,
        PhysicsCollisions = 128 + 1,
        PhysicsResolve = 128 + 2,

        Service = 256,
        ServiceHit = 256 + 1,
        ServiceFG = 256 + 2,

        Gamemode = 512,

        LateStateMachine = 0x10000 + 64,
        LateService = 0x10000 + 256,
        LateGamemode = 0x10000 + 512,

        ActivePollingReset = 0xFFFFFFF,
    }

    [RequireComponent(typeof(GameTimeManager))]
    public class FrameEnforcer : MonoBehaviour {

        public static float FRAME_TIME = 1f / 60.0f; // 1 sec over 60 frames

        private class FrameEnforcerCallback : IComparable<FrameEnforcerCallback> {
            public int priority;
            public Action<int> callback;

            public int CompareTo(FrameEnforcerCallback other) {
                return this.priority - other.priority;
            }
        }

        public Text fpsCounterText;
        public Text frameIndexText;
        private int updateCounterSnapshot = 0;
        private float timeSnapshot = 0.0f;

        private int updateCounter = 0;

        private List<FrameEnforcerCallback> updateActions = new List<FrameEnforcerCallback>();
        private GameTimeManager gameTime;

        private float startTime = 0.0f;
        private float prevTime = 0.0f;

        private uint elapsedTimeSec = 0;
        private double elapsedTimeMilliSec = 0f;

        private float frameSkipTime = 0f;

        private int frameIndex = 0;

        private float deltaTime;

        public void Awake() {
            this.enabled = false;
            ResetTime();

            gameTime = gameObject.GetComponent<GameTimeManager>();
            gameTime.AddNode(x => FRAME_TIME, new List<string> { "frameDelta" });
            gameTime.AddNode(x => deltaTime, new List<string> { "realDelta" });
        }

        public int index {
            get {
                if (this.enabled) return frameIndex;
                else return -1;
            }
        }

        public void FixedUpdate() {
            while (frameSkipTime > FRAME_TIME) {
                foreach (FrameEnforcerCallback action in updateActions) {
                    action.callback.Invoke(frameIndex);
                }

                updateCounter++;

                frameIndex++;
                frameIndexText.text = string.Format("Fr#: {0}", frameIndex.ToString());

                frameSkipTime -= FRAME_TIME;
            }

            frameSkipTime += Time.fixedDeltaTime;

            elapsedTimeMilliSec += Time.fixedDeltaTime;
            if (elapsedTimeMilliSec > 1.0) {
                elapsedTimeSec += 1;
                elapsedTimeMilliSec -= 1.0;
            }

            if ((elapsedTimeSec + elapsedTimeMilliSec) - timeSnapshot >= 0.45) {
                fpsCounterText.text = ((updateCounter - updateCounterSnapshot) / ((elapsedTimeSec + elapsedTimeMilliSec) - timeSnapshot)).ToString("F1") + " FPS";
                timeSnapshot = (float)(elapsedTimeSec + elapsedTimeMilliSec);
                updateCounterSnapshot = updateCounter;
            }
        }

        public void Update() {
            deltaTime = Time.deltaTime;

            if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame) {
                Debug.LogFormat("Frame : {0}", frameIndex);
                Debug.LogFormat("Time : {0}", Time.time);
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

        public void PauseExecution(bool pause) {
            this.enabled = !pause;
        }

        public void ResetTime() {
            frameSkipTime = FRAME_TIME;
            frameIndex = 0;

            elapsedTimeMilliSec = 0.0;
            elapsedTimeSec = 0;
        }

        public void StartFrameEnforcer() {
            this.enabled = true;
            EventManager.TriggerEvent<Events.FrameEnforcerReady>();
        }
    }
}