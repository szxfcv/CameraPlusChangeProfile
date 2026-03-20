using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage.Tags;
using CameraPlusChangeProfile.UI;
using IPA;
using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace CameraPlusChangeProfile
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin : MonoBehaviour
    {
        internal static IPA.Logging.Logger Log;
        private readonly CameraPlusChangeProfileMenu menu = new CameraPlusChangeProfileMenu();
        private bool tabAdded = false;
        private string profilesPath;
        private List<string> profiles;

        [Init]
        public void Init(IPA.Logging.Logger logger)
        {
            Log = logger;
        }
        [OnStart]
        public void OnApplicationStart()
        {
            // ★ Menu シーンがロードされたときに呼ばれる
            SceneManager.activeSceneChanged += OnSceneChanged;
            
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.name != "GameCore")
            {
                Log.Info($"newScene name is not GameCore.[{newScene.name}]");
                return;
            }
            if (tabAdded)
            {
                //DDLS.SetProperty("choices", menu.profileList);
                return;
            }
            else
            {
                tabAdded = true;
                try
                {
                    GameplaySetup.Instance.AddTab(
                    //"CP Profile Change",
                    "Banao Mod",
                    "CameraPlusChangeProfile.UI.xav7ze_tab.bsml",
                    menu
                    );
                    Log.Info("xav7ze_mod tab added safely!");
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to add xav7ze_mod tab: {ex}");
                }
            }

        }

    }

    // コルーチンを実行するためのMonoBehaviour
    public class CoroutineRunner : MonoBehaviour { }
}
