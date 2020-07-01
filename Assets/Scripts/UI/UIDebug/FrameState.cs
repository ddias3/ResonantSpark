using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace ResonantSpark {
    namespace UI {
        namespace UIDebug {
            public class FrameState : MonoBehaviour {
                public RawImage imgHit;
                public RawImage imgTrack;
                public RawImage imgSound;
                public RawImage imgProjectile;
                public RawImage imgChainCancellable;
                public RawImage imgSpecialCancellable;
                public RawImage imgCounterHit;

                public bool hit {
                    get {
                        return imgHit.enabled;
                    }
                    set {
                        imgHit.enabled = value;
                    }
                }

                public bool track {
                    get {
                        return imgTrack.enabled;
                    }
                    set {
                        imgTrack.enabled = value;
                    }
                }

                public bool sound {
                    get {
                        return imgSound.enabled;
                    }
                    set {
                        imgSound.enabled = value;
                    }
                }

                public bool projectile {
                    get {
                        return imgProjectile.enabled;
                    }
                    set {
                        imgProjectile.enabled = value;
                    }
                }

                public bool chainCancellable {
                    get {
                        return imgChainCancellable.enabled;
                    }
                    set {
                        imgChainCancellable.enabled = value;
                    }
                }

                public bool specialCancellable {
                    get {
                        return imgSpecialCancellable.enabled;
                    }
                    set {
                        imgSpecialCancellable.enabled = value;
                    }
                }

                public bool counterHit {
                    get {
                        return imgCounterHit.enabled;
                    }
                    set {
                        imgCounterHit.enabled = value;
                    }
                }
            }
        }
    }
}