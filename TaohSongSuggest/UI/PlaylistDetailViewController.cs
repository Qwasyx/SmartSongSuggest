﻿using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Types;
using HMUI;
using Settings;
using SmartSongSuggest.Configuration;
using SmartSongSuggest.Managers;
using SongSuggestNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SmartSongSuggest.UI
{
    public class PlaylistDetailViewController : INotifyPropertyChanged
    {
        internal static readonly PlaylistDetailViewController persController = new PlaylistDetailViewController();
        internal LevelPackDetailViewController lpdvc;
        internal static AnnotatedBeatmapLevelCollectionsViewController annotatedBeatmapLevelCollectionsViewController;
        internal static IPlaylist selectedPlaylist;

        public event PropertyChangedEventHandler PropertyChanged;

        [UIParams]
        private readonly BSMLParserParams parserParams;

        [UIComponent("root")]
        private readonly Transform rootTransform;

        [UIComponent("syncurl-button")]
        private readonly NoTransitionsButton syncUrlButton;

        private void ProcessSyncPlaylist()
        {
            Task.Run(() =>
            {
                if (selectedPlaylist == null)
                    return;

                string path = PlaylistManager.DefaultManager.GetManagerForPlaylist(selectedPlaylist).PlaylistPath;
                path = path.Replace(PlaylistManager.DefaultManager.PlaylistPath, "");

                PlaylistPath playlistPath = new PlaylistPath() { FileExtension = selectedPlaylist.SuggestedExtension, FileName = selectedPlaylist.Filename, Subfolders = path };
                SongSuggest.MainInstance.FilterSyncURL(playlistPath, playlistPath);

                IPA.Utilities.Async.UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                {
                    SongSuggestManager.UpdatePlaylists(selectedPlaylist.Filename);
                });
            });
        }

        public static bool AttachTo(Transform t, LevelPackDetailViewController pack)
        {
            if (t == null)
                return false;

            try
            {
                annotatedBeatmapLevelCollectionsViewController = GameObject.FindObjectOfType<AnnotatedBeatmapLevelCollectionsViewController>();

                BSMLReflectionsManager.Instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "SmartSongSuggest.UI.Views.PlaylistDetailView.bsml"), t.gameObject, persController);
                persController.rootTransform.localScale *= 0.6f;
                persController.lpdvc = pack;
                persController.lpdvc.didActivateEvent += Lpdvc_didActivateEvent;
                annotatedBeatmapLevelCollectionsViewController.didSelectAnnotatedBeatmapLevelCollectionEvent += collectionSelected;
                collectionSelected(annotatedBeatmapLevelCollectionsViewController.selectedAnnotatedBeatmapLevelCollection);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void collectionSelected(IAnnotatedBeatmapLevelCollection obj)
        {
            if (obj is IPlaylist pl)
            {
                selectedPlaylist = pl;
            } 
            else
            {
                selectedPlaylist = null;
            }

            persController.UpdateView();
        }

        private static void Lpdvc_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            persController.UpdateView();
        }

        private void UpdateView()
        {
            var cfg = SettingsController.cfgInstance;
            bool active = selectedPlaylist != null && selectedPlaylist.TryGetCustomData("syncURL", out var outSyncURL) && cfg.ShowSyncURL;
            syncUrlButton.gameObject.SetActive(active);
            //syncUrlButton.gameObject.SetActive(selectedPlaylist != null && selectedPlaylist.TryGetCustomData("syncURL", out var outSyncURL));
        }
    }
}
