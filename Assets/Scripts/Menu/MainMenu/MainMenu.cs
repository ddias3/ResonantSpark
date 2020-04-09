using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

using ResonantSpark.Service;

namespace ResonantSpark {
    namespace Menu {
        public class MainMenu : MonoBehaviour {

            public GameObject mainMenu;
            public GameObject optionsMenu;
            public GameObject soundMenu;
            public GameObject controllerMenu;
            public GameObject creditsMenu;

            public Selectable mainMenuActive;
            public Selectable optionsMenuActive;
            public Selectable soundMenuActive;
            public Selectable controllerMenuActive;
            public Selectable creditsMenuActive;

            public MultiplayerEventSystem multiplayerEventSystem;

            public void OnPressVersus() {
                Persistence.GetPersistence().gamemode = "oneOnOneRoundBased";
                //SceneManager.LoadScene("Scenes/Menu/CharacterSelect");

                SceneManager.LoadScene("Scenes/Levels/Practice");
                Persistence pers = Persistence.GetPersistence();
                pers.SetCharacterSelected(0, "lawrence");
                pers.SetColorSelected(0, 1);
                pers.SetCharacterSelected(1, "male0");
                pers.SetColorSelected(1, 0);
            }

            public void OnPressTraining() {
                Persistence.GetPersistence().gamemode = "training";
                //SceneManager.LoadScene("Scenes/Menu/CharacterSelect");

                SceneManager.LoadScene("Scenes/Levels/Practice");
                Persistence pers = Persistence.GetPersistence();
                pers.SetCharacterSelected(0, "lawrence");
                pers.SetColorSelected(0, 1);
                pers.SetCharacterSelected(1, "male0");
                pers.SetColorSelected(1, 0);
            }

            public void OnPressOptions() {
                mainMenu.SetActive(false);
                optionsMenu.SetActive(true);

                optionsMenuActive.Select();
            }

            public void OnPressOptionsSounds() {
                optionsMenu.SetActive(false);
                soundMenu.SetActive(true);

                soundMenuActive.Select();
            }

            public void OnPressCredits() {
                mainMenu.SetActive(false);
                creditsMenu.SetActive(true);

                creditsMenuActive.Select();
            }

            public void OnBackCredits() {
                creditsMenu.SetActive(false);
                mainMenu.SetActive(true);

                mainMenuActive.Select();
            }

            public void OnBackOptions() {
                optionsMenu.SetActive(false);
                mainMenu.SetActive(true);

                mainMenuActive.Select();
            }

            public void OnBackOptionsSounds() {
                soundMenu.SetActive(false);
                optionsMenu.SetActive(true);

                optionsMenuActive.Select();
            }

            public void OnPressOptionsController() {
                optionsMenu.SetActive(false);
                controllerMenu.SetActive(true);

                controllerMenuActive.Select();
            }

            public void OnBackOptionsController() {
                controllerMenu.SetActive(false);
                optionsMenu.SetActive(true);

                optionsMenuActive.Select();
            }

            public void OnPressQuit() {
                Application.Quit();
            }
        }
    }
}