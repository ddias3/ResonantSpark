using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace UI {
        public class InGameElement : MonoBehaviour {

            protected GameTimeManager gameTime;

            public void Start() {
                gameTime = GameObject.FindGameObjectWithTag("rspTime").GetComponent<GameTimeManager>();
            }
        }
    }
}