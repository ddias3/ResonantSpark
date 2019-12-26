using ResonantSpark.Input.Combinations;

namespace ResonantSpark {
    namespace Input {
        public interface IAdapter {
            FightingGameInputCodeBut[] GetButton();
        }

        public class Buttons : IAdapter {
            private Combination button;
            private int numButtons = 0;

            public Buttons(ButtonPress buttonPress) {
                button = buttonPress;
                numButtons = 1;
            }

            public Buttons(Button2Press button2Press) {
                button = button2Press;
                numButtons = 2;
            }

            public Buttons(Button3Press button3Press) {
                button = button3Press;
                numButtons = 3;
            }

            public FightingGameInputCodeBut[] GetButton() {
                throw new System.NotImplementedException();
            }
        }
    }
}