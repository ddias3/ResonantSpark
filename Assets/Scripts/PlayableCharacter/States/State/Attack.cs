using System;

using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace CharacterStates {
        public class Attack : BaseState {

            private int startFrame;
            private int frameCount;

            public new void Start() {
                base.Start();
                states.Register(this, "attack");

                RegisterInputCallbacks()
                    .On<ButtonPress>(OnButtonPress)
                    .On<DirectionPress>(OnDirectionPress);
            }

            public override void Enter(int frameIndex, IState previousState) {
                    // Start OnEnter with this
                GivenInput(fgChar.GivenCombinations());

                startFrame = frameIndex;
                frameCount = 0;

                fgChar.SetLocalMoveDirection(0.0f, 0.0f);
                fgChar.Play("5AA", 0, 0.0f);
            }

            public override void Execute(int frameIndex) {
                FindInput(fgChar.GetFoundCombinations());

                if (frameCount >= 30) {
                    changeState(states.Get("stand"));
                }

                ++frameCount;
            }

            public override void Exit(int frameIndex) {
                // do nothing
            }

            private void OnDirectionPress(Action stop, Combination combo) {
                // TODO: figure out the stuff on attack
            }

            private void OnButtonPress(Action stop, Combination combo) {
                // TODO: figure out the stuff on button press
            }
        }
    }
}