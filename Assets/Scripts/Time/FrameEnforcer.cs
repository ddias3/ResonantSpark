﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ResonantSpark {
    public enum FramePriority : int {
        ActivePollingReset = 1,

        Gamemode = 16,
        Service = 32,
        StateMachine = 64,
        StateMachinePass1 = 64 + 1,

        InputBuffer = 128,

        PhysicsMovement = 256,
        PhysicsCollisions = 256 + 1,
        PhysicsResolve = 256 + 2,

        LateService = 0x1000 + 32,
        LateStateMachine = 0x1000 + 64,
    }

    [RequireComponent(typeof(GameTimeManager))]
    public class FrameEnforcer : MonoBehaviour {

        public static float FRAME_TIME = 1f / 60.0f; // 1 sec over 60 frames

        private class FrameEnforcerCallback : IComparable<FrameEnforcerCallback> {
            public int priority;
            public Action<int> callback;

            public int CompareTo(FrameEnforcerCallback other) {
                return other.priority - this.priority;
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

        private int frameIndex = 0;

        private float deltaTime;

        public void Awake() {
            this.enabled = false;
            //elapsedTime = FRAME_TIME;
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
        }

        public void FixedUpdate() {
            int stepsInFrame = 0;

            deltaTime = Time.time - prevTime;

            while (Time.time - prevTime > FRAME_TIME) {
                foreach (FrameEnforcerCallback action in updateActions) {
                    action.callback.Invoke(frameIndex);
                }

                prevTime = Time.time;

                stepsInFrame++;
                updateCounter++;

                frameIndex++;
                frameIndexText.text = "Fr#: " + frameIndex.ToString();
            }

            //if (stepsInFrame > 1) {
            //    Debug.LogWarning("Frame Skip at frame(" + frameIndex + "). Stepped " + stepsInFrame + " times in single frame");
            //}

            if (Time.time - timeSnapshot >= 0.45) {
                fpsCounterText.text = ((updateCounter - updateCounterSnapshot) / (Time.time - timeSnapshot)).ToString("F1") + " FPS";
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