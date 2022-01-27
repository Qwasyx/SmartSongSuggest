﻿using System;
using System.Collections.Generic;
using System.Linq;
using SongLibraryNS;
using FileHandling;

namespace BanLike
{
    public class SongBanning
    {
        public SongLibrary songLibrary { get; set; }
        public FileHandler fileHandler { get; set; }
        public List<SongBan> bannedSongs = new List<SongBan>();

        public List<String> GetBannedIDs()
        {
            return bannedSongs.Select(p => p.songID).ToList();
        }

        public Boolean IsBanned(String songHash, String difficulty)
        {
            String songID = songLibrary.GetID(songHash, difficulty);
            return bannedSongs.Any(p => p.songID == songID);
        }

        public Boolean IsPermaBanned(String songHash, String difficulty)
        {
            String songID = songLibrary.GetID(songHash, difficulty);
            return bannedSongs.Any(p => p.songID == songID && p.expire == DateTime.MaxValue);
        }

        public void LiftBan(String songHash, String difficulty)
        {
            String songID = songLibrary.GetID(songHash, difficulty);
            bannedSongs.RemoveAll(p => p.songID == songID);
            Save();
        }

        public void SetBan(String songHash, String difficulty, int days)
        {
            //If a ban is in place, remove it before setting the new ban.
            if (IsBanned(songHash, difficulty)) LiftBan(songHash, difficulty);
            bannedSongs.Add(new SongBan { expire = DateTime.UtcNow.AddDays(days), activated = DateTime.UtcNow, songID = songLibrary.GetID(songHash, difficulty) });
            Save();
        }

        public void SetPermaBan(String songHash, String difficulty)
        {
            //If a ban is in place, remove it before setting the new ban.
            if (IsBanned(songHash, difficulty)) LiftBan(songHash, difficulty);
            bannedSongs.Add(new SongBan { expire = DateTime.MaxValue, activated = DateTime.UtcNow, songID = songLibrary.GetID(songHash, difficulty) });
            Save();
        }

        public void Save()
        {
            fileHandler.SaveBannedSongs(bannedSongs);
        }
    }
}
