using UnityEngine;
using UnityEngine.SceneManagement;

using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Menu {
        public class MainMenu : MonoBehaviour {
            public TMPro.TMP_InputField char0;
            public TMPro.TMP_InputField char1;
            public TMPro.TMP_InputField level;
            public void OnPressLoad() {
                string strChar0 = "male0", strChar1 = "male0", strLevel = "corporation";

                if (char0.text != "") {
                    strChar0 = char0.text;
                }

                if (char1.text != "") {
                    strChar1 = char1.text;
                }

                if (level.text != "") {
                    strLevel = level.text;
                }

                Persistence persistence = Persistence.GetPersistence();

                persistence.gamemode = "oneOnOneRoundBased";
                persistence.SetCharacterSelected(0, strChar0);
                persistence.SetCharacterSelected(1, strChar1);

                SceneManager.LoadScene(strLevel, LoadSceneMode.Single);
            }
        }
    }
}