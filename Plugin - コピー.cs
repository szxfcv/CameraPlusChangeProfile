using BeatSaberMarkupLanguage.GameplaySetup;
using CameraPlusChangeProfile.UI;
using HMUI;
using IPA;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CameraPlusChangeProfile
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance;
        internal static IPA.Logging.Logger Log;

        private CameraPlusChangeProfileMenu menu;
        internal bool tabAdded = false;

        [Init]
        public void Init(IPA.Logging.Logger logger)
        {
            Log = logger;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Instance = this;
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.name != "MainMenu")
                return;

            Log.Info("MainMenu loaded. Waiting for MainMenuViewController...");

            new GameObject("CPProfileInit").AddComponent<WaitForMainMenu>();
        }

        private class WaitForMainMenu : MonoBehaviour
        {
            private void Update()
            {
                var mainMenu = Resources.FindObjectsOfTypeAll<MainMenuViewController>().FirstOrDefault();
                if (mainMenu == null)
                    return;

                Plugin.Log.Info("MainMenuViewController found. Registering event...");

                mainMenu.didActivateEvent += OnMainMenuActivated;

                Destroy(gameObject);
            }
            /*
            private void OnMainMenuActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
            {
                if (!firstActivation)
                    return;

                if (Plugin.Instance.tabAdded)
                    return;

                Plugin.Instance.tabAdded = true;

                Plugin.Log.Info("MainMenu activated. Creating ViewController...");

                // ★ ViewController を MainMenu の子として生成
                var go = new GameObject("CameraPlusChangeProfileMenu");
                go.transform.SetParent(Resources.FindObjectsOfTypeAll<MainMenuViewController>().First().transform, false);

                Plugin.Instance.menu = go.AddComponent<CameraPlusChangeProfileMenu>();

                Plugin.Log.Info("Adding GameplaySetup tab...");

                GameplaySetup.Instance.AddTab(
                    "Banao Mod",
                    "CameraPlusChangeProfile.UI.xav7ze_tab.bsml",
                    Plugin.Instance.menu
                );

                Plugin.Log.Info("Tab added successfully!");
            }
            */
            private void OnMainMenuActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
            {
                if (!firstActivation)
                    return;

                if (Plugin.Instance.tabAdded)
                    return;

                Plugin.Instance.tabAdded = true;

                Plugin.Log.Info("MainMenu activated. Creating ViewController...");

                // ★ MainMenuViewController の transform を親にする
                var parent = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First().transform;

                // ★ ViewController として正しく生成
                var go = new GameObject("CameraPlusChangeProfileMenu");
                go.transform.SetParent(parent, false);

                Plugin.Instance.menu = go.AddComponent<CameraPlusChangeProfileMenu>();

                Plugin.Log.Info("Adding GameplaySetup tab...");

                GameplaySetup.Instance.AddTab(
                    "Banao Mod",
                    "CameraPlusChangeProfile.UI.xav7ze_tab.bsml",
                    Plugin.Instance.menu
                );

                Plugin.Log.Info("Tab added successfully!");
            }

        }
    }
}
