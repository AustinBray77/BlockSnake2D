using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GameMetadata : ISavedGameMetadata
{
    public bool IsOpen { get; private set; }
    public string Filename { get; private set; }
    public string Description { get; private set; }
    public string CoverImageURL { get; private set; }
    public TimeSpan TotalTimePlayed { get; private set; }
    public DateTime LastModifiedTimestamp { get; private set; }

    public GameMetadata(bool isOpen, string filename, string description, string coverImageURL, TimeSpan totalTimePlayed, DateTime lastModifiedTimestamp)
    {
        IsOpen = isOpen;
        Filename = filename;
        Description = description;
        CoverImageURL = coverImageURL;
        TotalTimePlayed = totalTimePlayed;
        LastModifiedTimestamp = lastModifiedTimestamp;
    }
}