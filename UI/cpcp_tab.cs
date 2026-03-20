using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;

namespace CameraPlusChangeProfile.UI
{
    public class CameraPlusChangeProfileMenu : BSMLAutomaticViewController
    {
        // ─────────────────────────────────────────────
        //  custom-list の 1 行を表すデータモデル
        // ─────────────────────────────────────────────
        internal class ProfileElement
        {
            public string Name { get; set; } = "";

            [UIValue("item-text")]
            public string ItemText => Name;
        }

        // ─────────────────────────────────────────────
        //  custom-list の UI コンポーネント
        // ─────────────────────────────────────────────
        [UIComponent("profile-list")]
        public CustomCellListTableData Table;

        [UIValue("profiles")]
        public List<object> Profiles { get; } = new();

        // ─────────────────────────────────────────────
        //  custom-list の UI コンポーネント
        // ─────────────────────────────────────────────
        [UIComponent("selected-item-text")]
        private TextMeshProUGUI modifiedText;

        // ─────────────────────────────────────────────
        //  BSML 読み込み後に呼ばれる
        // ─────────────────────────────────────────────
        [UIAction("#post-parse")]
        private void Setup()
        {
            Plugin.Log.Info($"[BSML] Setup start");

            if (Profiles == null)
                Plugin.Log.Error("[BSML] Profiles is NULL!");
            else
                Plugin.Log.Info($"[BSML] Profiles initialized with count: {Profiles.Count}");

            if (Table == null)
                Plugin.Log.Error("[BSML] Table is NULL!");
            else
                Plugin.Log.Info("[BSML] Table is ready");

            LoadProfiles();
            //modifiedText.text = "Select a profile to apply";
        }

        // ─────────────────────────────────────────────
        //  Profiles フォルダを読み取り custom-list に反映
        // ─────────────────────────────────────────────
        private void LoadProfiles()
        {
            Profiles.Clear();
            string profilesPath = Path.Combine(Environment.CurrentDirectory, "UserData", "CameraPlus", "Profiles");

            if (!Directory.Exists(profilesPath))
                Directory.CreateDirectory(profilesPath);

            var dirs = Directory.GetDirectories(profilesPath)
                                .Select(Path.GetFileName)
                                .ToList();

            foreach (var dir in dirs)
            {
                Plugin.Log.Info($"Add options {dir}");
                Profiles.Add(new ProfileElement { Name = dir });
                
            }
            foreach (var profile in Profiles)
            {
                Plugin.Log.Info($"Profile: {(profile as ProfileElement)?.Name}");
            }
            Table?.TableView?.ReloadData();
        }

        // ─────────────────────────────────────────────
        //  選択時の処理
        // ─────────────────────────────────────────────
        [UIAction("on-select")]
        public void OnSelect(TableView _, object row)
        {
            var elem = row as ProfileElement;
            if (elem == null) return;

            modifiedText.text = $"Selected: {elem.Name}";
            //ApplyProfile(elem.Name);
        }

        [UIAction("click-Game-action")]
        private void GameButtonAction()
        {
            if (modifiedText.text.StartsWith("Selected: "))
            {
                string profileName = modifiedText.text.Substring("Selected: ".Length);
                ApplyGameProfile(profileName);
            }
            else
            {
                Plugin.Log.Warn("No profile selected to apply.");
            }
        }

        [UIAction("click-Menu-action")]
        private void MenuButtonAction()
        {
            if (modifiedText.text.StartsWith("Selected: "))
            {
                string profileName = modifiedText.text.Substring("Selected: ".Length);
                ApplyMenuProfile(profileName);
            }
            else
            {
                Plugin.Log.Warn("No profile selected to apply.");
            }
        }

        // ─────────────────────────────────────────────
        //  CameraPlus.json に Profile を適用
        // ─────────────────────────────────────────────
        private void ApplyGameProfile(string profile)
        {
            string configPath = Path.Combine(Environment.CurrentDirectory, "UserData", "CameraPlus.json");
            
            if (!File.Exists(configPath))
                return;

            Plugin.Log.Info($@"configPath = {configPath}");
            var json = File.ReadAllText(configPath);
            CPconfig cpconfig = JsonConvert.DeserializeObject<CPconfig>(json);
            Plugin.Log.Info($"Current GameProfile: {cpconfig.GameProfile}");

            cpconfig.GameProfile = profile;
            Plugin.Log.Info($"Current GameProfile: {cpconfig.GameProfile}");

            //string output = JsonSerializer.Serialize(cpconfig, new JsonSerializerOptions { WriteIndented = true });
            var output = JsonConvert.SerializeObject(cpconfig, Formatting.Indented);
            File.WriteAllText(configPath, output);

            Plugin.Log.Info($"Applied CameraPlus Profile: {profile}");
        }
        private void ApplyMenuProfile(string profile)
        {
            string configPath = Path.Combine(Environment.CurrentDirectory, "UserData", "CameraPlus.json");

            if (!File.Exists(configPath))
                return;

            Plugin.Log.Info($@"configPath = {configPath}");
            var json = File.ReadAllText(configPath);
            CPconfig cpconfig = JsonConvert.DeserializeObject<CPconfig>(json);
            Plugin.Log.Info($"Current MenuProfile: {cpconfig.GameProfile}");

            cpconfig.MenuProfile = profile;
            Plugin.Log.Info($"Current MenuProfile: {cpconfig.GameProfile}");

            //string output = JsonSerializer.Serialize(cpconfig, new JsonSerializerOptions { WriteIndented = true });
            var output = JsonConvert.SerializeObject(cpconfig, Formatting.Indented);
            File.WriteAllText(configPath, output);

            Plugin.Log.Info($"Applied CameraPlus Profile: {profile}");
        }

        // ─────────────────────────────────────────────
        //  CameraPlus.json の構造
        // ─────────────────────────────────────────────
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
