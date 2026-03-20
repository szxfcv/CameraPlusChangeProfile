using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Util;
using BeatSaberMarkupLanguage.ViewControllers;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine.Pool;

namespace CameraPlusChangeProfile.UI
{
    public class CameraPlusChangeProfileMenu : BSMLAutomaticViewController
    {

        private List<object> listOptions = new List<object> { "" };

        [UIComponent("DD-options")]
        public DropDownListSetting DDLS;

        // ▼ Dropdown の選択肢（BSML: choices="list-options"）
        [UIValue("list-options")]
        public List<object> profileList = new List<object> { "" };
        //[UIValue("list-choice")]
        //public object listChoice = "";

        // ▼ UI が構築された後に呼ばれる
        [UIAction("#post-parse")]
        private void Setup()
        {
            profileList = LoadProfiles();
            NotifyPropertyChanged("list-options");
        }

        
        [UIAction("OnTestButton")]
        public void OnTestButton()
        {
            //listOptions = new object[] { "hoge", "fuga", "piyo" }.ToList();
            //listOptions.Add("hoge");
            //NotifyPropertyChanged("list-options");
            //ListOptions = new object[] { "hoge", "fuga", "piyo" }.ToList();
            LoadProfiles();
            Plugin.Log.Info("Test Button Clicked!");
            //DDLS.SetProperty("choices", profileList);
        }
        /*
        [UIValue("profileList")]
        private List<object> profileList = new List<object>(); // Changed here

        
        [UIValue("list-options")]
        public List<object> ListOptions
        {
            get => listOptions;
            set => Setproperty(ref listOptions, value);
        }

        [UIValue("list-choice")]
        public object listChoice = "";

        private string selectedProfile;

        private void Setproperty(ref List<object> field, List<object> value)
        {
            Plugin.Log.Info($"Updating ListOptions: {string.Join(", ", field)}");
            field = value;
            Plugin.Log.Info($"ListOptions updated: {string.Join(", ", field)}");
            NotifyPropertyChanged("list-options");
        }
        */
        private List<object> LoadProfiles()
        {
            var profilesPath = Path.Combine(Environment.CurrentDirectory, "UserData", "CameraPlus", "Profiles");
            Plugin.Log.Info($"Looking for profiles in: {profilesPath}");
            var profiles = Directory.GetDirectories(profilesPath)
                                    .Select(Path.GetFileName)
                                    .ToList();
            //var profiles = Directory.GetFiles(profilesPath, "*.json")
            //                        .Select(Path.GetFileNameWithoutExtension)
            //                        .ToList();
            foreach (var profile in profiles)
            {
                Plugin.Log.Info($"Found profile: {profile}");
                //listOptions.Add(profile);
            }

            Plugin.Log.Info($"Before Update - Profiles: {string.Join(", ", profileList)}");
            // List<string> → List<object> に変換
            profileList = profiles.Cast<object>().ToList();
            Plugin.Log.Info($"After Update - ProfileList: {string.Join(", ", profileList)}");

            DDLS.SetProperty("list-options", profileList);
            NotifyPropertyChanged("list-options");
            return profileList;

            // 初期選択値
            //listChoice = profiles.FirstOrDefault() ?? "";

            

            //listOptions = new object[] { "hoge", "fuga", "piyo" }.ToList();
            //NotifyPropertyChanged("list-options");
            //listOptions = profiles.Cast<object>().ToList();
            //profileList = profiles.Cast<object>().ToList();
            //selectedProfile = profiles.FirstOrDefault();
        }
        
        [UIAction("OnProfileChanged")]
        private void OnProfileChanged(string value)
        {
            //listChoice = value;
            Plugin.Log.Info($"Selected CameraPlus profile: {value}");

            //ApplyProfile(value);
        }

        private void ApplyProfile(string profile)
        {
            // CameraPlus.json を書き換え
            var configPath = Path.Combine(Environment.CurrentDirectory, "UserData", "CameraPlus.json");
            var json = File.ReadAllText(configPath);
            CPconfig cpconfig = JsonSerializer.Deserialize<CPconfig>(json);
            cpconfig.GameProfile = profile;
            string scpconfig = JsonSerializer.Serialize(cpconfig, new JsonSerializerOptions { WriteIndented = true });
            //var node = SimpleJSON.JSON.Parse(json);
            //node["profile"] = profile;
            File.WriteAllText(configPath, scpconfig);

        }
        public class CPconfig
        {
            public bool ProfileSceneChange { get; set; }
            public string MenuProfile { get; set; }
            public string GameProfile { get; set; }
            public string RotateProfile { get; set; }
            public string MultiplayerProfile { get; set; }
            public string SongSpecificScriptProfile { get; set; }
            public string CameraQuadPosition { get; set; }
            public float CameraCubeSize { get; set; }
            public bool CameraQuadStretch { get; set; }

        }

    }
}
