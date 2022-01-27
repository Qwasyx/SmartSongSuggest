﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ActivePlayerData;
using BanLike;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using FileHandling;
using HMUI;
using LinkedData;
using PlaylistNS;
using Settings;
using SongLibraryNS;
using TaohSongSuggest.Configuration;
using TaohSongSuggest.Managers;
using TMPro;
using UnityEngine;
using WebDownloading;

namespace TaohSongSuggest.UI
{
    [HotReload(RelativePathToLayout = @"Views\SongSuggestMain.bsml")]
    [ViewDefinition("TaohSongSuggest.UI.Views.SongSuggestMain.bsml")]
    class SettingsController : BSMLAutomaticViewController, INotifyPropertyChanged
    {
        public static PluginConfig cfgInstance;

        [UIComponent("bgProgress")] 
        public ImageView bgProgress;


        [UIComponent("SuggestBTN")]
        public NoTransitionsButton suggestBTN;

        [UIComponent("OldestBTN")]
        public NoTransitionsButton oldestBTN;

        bool _suggestShow;

        [UIValue("suggest-show")]
        public bool ShowSuggestSettings
        {
            get => _suggestShow;
            set
            {
                _suggestShow = value;
                NotifyPropertyChanged(nameof(SuggestColor));
                NotifyPropertyChanged(nameof(OldestColor));
                NotifyPropertyChanged(nameof(ShowOldestSettings));
                NotifyPropertyChanged(nameof(ShowSuggestSettings));
            }
        }

        [UIValue("oldest-show")]
        public bool ShowOldestSettings => !_suggestShow;


        [UIValue("color-suggest")]
        public string SuggestColor => _suggestShow ? "#00ff00" : "white";


        [UIValue("color-oldest")]
        public string OldestColor => _suggestShow ? "white" : "#00ff00";

        [UIAction("settingsOldest")]
        void so()
        {
            ShowSuggestSettings = false;
        }

        [UIAction("settingsSuggest")]
        void ss()
        {
            ShowSuggestSettings = true;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            RefreshProgressBar(0f);
        }

        void GenerateOldest()
        {
            SongSuggestManager.Oldest100ActivePlayer();
        }

        void GeneratePlaylist()
        {
            Plugin.Log.Info("Generate pretty please");

            SongSuggestManager.SuggestSongs();
        }
        public void SetButtonsEnable(bool enable)
        {
            suggestBTN.interactable = enable;
            oldestBTN.interactable = enable;
        }

        public void RefreshProgressBar(float prog)
        {
            try
            {
                //Plugin.Log.Info($"{prog} / 1");

                bgProgress.color = Color.green;

                var x = (bgProgress.gameObject.transform as RectTransform);
                if (x == null)
                    return;

                x.anchorMax = new Vector2(prog, 1);

                x.ForceUpdateRectTransforms();
            } catch
            {

            }

        }


    }
}
