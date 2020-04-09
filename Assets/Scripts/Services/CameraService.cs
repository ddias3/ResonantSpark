using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ResonantSpark.Utility;

namespace ResonantSpark
{
    namespace Service
    {
        public class CameraService : MonoBehaviour, ICameraService
        {
            private FrameEnforcer frame;
            private float maxDuration;
            private float duration;
            private float magnitude;
            private GameTimeManager gameTimeManager;
            bool doneLerping;
            System.Random rand;
            int randNum;
            GameObject cam;
            GameObject parentCam;
            float currGameTime;
            bool firstFrameUpdateComplete;
            Vector3 origPos;
            bool isShaking;
            float cumTime;
            float xPos;
            float yPos;
            // Use this for initialization
            void Start()
            {
                magnitude = 0.0f;
                xPos = 0.0f;
                yPos = 0.0f;
                cumTime = 0.0f;
                isShaking = false;
                cam = null;
                firstFrameUpdateComplete = false;
                doneLerping = true;
                rand = new System.Random();
                duration = 0.0f;
                maxDuration = 0.0f;
                frame = GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>();
                frame.AddUpdate((int)FramePriority.Service, new System.Action<int>(FrameUpdate));
                //frame.AddUpdate((int)FramePriority.CameraShake, new System.Action<int>(CameraShake));
                gameTimeManager = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
                //gameTimeManager.AddLayer(new Func<float, float>(GameTime), "gameTime");

                EventManager.TriggerEvent<Events.ServiceReady, Type>(typeof(CameraService));
            }

            private void FrameUpdate(int frameIndex)
            {
                if (cam == null || !isShaking)
                {
                    return;
                }
                if (duration <= 0.0f)
                {
                    cam.transform.localPosition = new Vector3(0,0,0)/*parentCam.transform.position*/;
                    isShaking = false;
                    firstFrameUpdateComplete = false;
                    return;
                }
                //Debug.Log("CameraService Frame Update called!");
                float prevGameTime;
                currGameTime = gameTimeManager.Layer("gameTime");
                
                if (firstFrameUpdateComplete)
                {
                    prevGameTime = currGameTime;
                    cumTime += currGameTime;
                    Debug.Log("prev game time: " + prevGameTime);
                }
                else
                {
                    firstFrameUpdateComplete = true;
                    cumTime = currGameTime;
                }

                
                //Debug.Log("curr game time: " + currGameTime);

                
                /*if (doneLerping)
                {
                    randNum = rand.Next(0, 2); // returns an int between 0 and 1
                    int dir1 = 1;
                    int dir2 = 1;
                    if (randNum == 0)
                    {
                        dir1 = -1;
                    }

                    randNum = rand.Next(0, 2);
                    if (randNum == 0)
                    {
                        dir2 = -1;
                    }
                    float amount = Mathf.Sin(cumTime) * (duration / maxDuration);
                    xPos = amount * dir1;
                    yPos = amount * dir2;

                    Debug.Log("<CameraService> cumTime: " + cumTime);
                    Debug.Log("<CameraService> Sin(cumTime): " + Mathf.Sin(cumTime));
                    Debug.Log("<CameraService> amount: " + amount);
                    Debug.Log("<CameraService> duration: " + duration);
                    Debug.Log("<CameraService> max duration: " + maxDuration);
                    Debug.Log("<CameraService> xPos: " + xPos);
                    Debug.Log("<CameraService> yPos: " + yPos);
                    doneLerping = false;
                }
                else
                {*/
                    /*Debug.Log("<CameraService> lerping");
                    float newX = Mathf.Lerp(cam.transform.localPosition.x, xPos, 0.1f);
                    float newY = Mathf.Lerp(cam.transform.localPosition.y, yPos, 0.1f);
                    Debug.Log("<CameraService> newX: " + newX + ", lerping to " + xPos);
                    Debug.Log("<CameraService> newY: " + newY + ", lerping to " + yPos);
                    */

                    ///////////////////////////////
                    int newX = rand.Next(-1, 1); // returns an int between 0 and 1;
                    int newY = rand.Next(-1, 1);
                    //float amt = (float)rand.NextDouble();
                    cam.transform.localPosition = new Vector3(magnitude * newX, magnitude * newY, cam.transform.localPosition.z);

                    ///////////////////////////////
                    /*
                    if (newX >= xPos && newY >= yPos)
                    {
                        doneLerping = true;
                    }
                    else if (newX >= xPos)
                    {
                        cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, newY, cam.transform.localPosition.z);
                    }
                    else if(newY >= yPos)
                    {
                        cam.transform.localPosition = new Vector3(newX, cam.transform.localPosition.y, cam.transform.localPosition.z);
                    }
                    else
                    {
                        cam.transform.localPosition = new Vector3(newX, newY, cam.transform.localPosition.z);
                    }
                    */

                    Debug.Log("<CameraService> X, y: " + newX + ", " + newY);

                //}
                duration -= currGameTime/*timeDiff*/;
                Debug.Log("duration remaining: " + duration);
            }

            public void CameraShake(float duration, float magnitude)
            {
                isShaking = true;
                this.magnitude = magnitude;
                Debug.Log("CameraService Camera Shake called!");
                parentCam = GameObject.FindGameObjectWithTag("rspCamera");
                cam = parentCam.transform.GetChild(1).gameObject;
                origPos = cam.transform.position;
                Debug.Log("camera component: " + cam.name);
                maxDuration = duration;
                this.duration = duration;

            }
        }
    }
}
