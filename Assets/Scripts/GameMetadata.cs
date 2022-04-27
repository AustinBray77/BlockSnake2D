using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

//Implementation class for saving games on google play
public class GameMetadata : ISavedGameMetadata
{
    //Stores whether the file is open
    public bool IsOpen { get; private set; }

    //Stores the filename
    public string Filename { get; private set; }

    //Stores the file description
    public string Description { get; private set; }

    //Stores the cover image for the save
    public string CoverImageURL { get; private set; }

    //Stores the total time played by the player
    public TimeSpan TotalTimePlayed { get; private set; }

    //Stores the last time this save was modified
    public DateTime LastModifiedTimestamp { get; private set; }

    //Default constructor
    public GameMetadata(bool isOpen, string filename, string description, string coverImageURL, TimeSpan totalTimePlayed, DateTime lastModifiedTimestamp)
    {
        //Assigns instance variable to the passed values
        IsOpen = isOpen;
        Filename = filename;
        Description = description;
        CoverImageURL = coverImageURL;
        TotalTimePlayed = totalTimePlayed;
        LastModifiedTimestamp = lastModifiedTimestamp;
    }
}