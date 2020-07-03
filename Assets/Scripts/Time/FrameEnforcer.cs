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

        private float elapsedTime = 0f;

        private int frameIndex = 0;

        private float deltaTime;

        public void Awake() {
            this.enabled = false;
            elapsedTime = FRAME_TIME;
            frameIndex = 0;

            gameTime = gameObject.GetComponent<GameTimeManager>();
            gameTime.AddNode(x => FRAME_TIME, new List<string> { "frameDelta" });
            gameTime.AddNode(x => deltaTime, new List<string> { "realDelta" });
        }

        public void Start() {
            StartTimeCount();
        }

        public int index {
            get {
                if (this.enabled) return frameIndex;
                else return -1;
            }
        }

        public void StartTimeCount() {
            startTime = Time.time;
            prevTime = startTime;
            frameIndex = 0;
        }

        public void FixedUpdate() {
            int stepsInFrame = 0;

            deltaTime = Time.time - prevTime;

            while (elapsedTime > FRAME_TIME * frameIndex) {
                foreach (FrameEnforcerCallback action in updateActions) {
                    action.callback.Invoke(frameIndex);
                }

                prevTime = Time.time;

                stepsInFrame++;
                updateCounter++;

                frameIndex++;
                frameIndexText.text = string.Format("Fr#: {0}", frameIndex.ToString());

                elapsedTime -= FRAME_TIME;
            }

            //elapsedTime += Time.fixedDeltaTime;
            elapsedTime = Time.time - startTime;

            //if (stepsInFrame > 1) {
            //    Debug.LogWarning("Frame Skip at frame(" + frameIndex + "). Stepped " + stepsInFrame + " times in single frame");
            //}

            if (Time.time - timeSnapshot >= 0.45) {
                fpsCounterText.text = ((updateCounter - updateCounterSnapshot) / (Time.time - timeSnapshot)).ToString("F1") + " FPS";
                timeSnapshot = Time.time;
                updateCounterSnapshot = updateCounter;
            }
        }

        public void Update() {
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

        public void StartFrameEnforcer() {
            this.enabled = true;
            EventManager.TriggerEvent<Events.FrameEnforcerReady>();
        }
    }
}