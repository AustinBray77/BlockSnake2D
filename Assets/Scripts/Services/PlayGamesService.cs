using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;
using UnityEngine.SocialPlatforms;

//Class to control the service connecting unity to the google play games services
public class PlayGamesService : SingletonDD<PlayGamesService>
{
    //Stores the id of the highscore leaderboard
    public const string HighScoreID = "CgkIiJOyzo0MEAIQAQ";

    //Stores the last loaded save from the play games server
    public byte[] LastSave { get; private set; } = null;

    //Property to get the constant client for saved games
    private ISavedGameClient _client => PlayGamesPlatform.Instance.SavedGame;

    //Stores the configuration
    private static PlayGamesClientConfiguration s_config;

    //Stores if the user is signed in
    private bool _isSignedIn = false;

    //Method called on scene start
    public void Start()
    {
        //Builds the configuration
        s_config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();

        //Initializes the configurations
        PlayGamesPlatform.InitializeInstance(s_config);

        //Enables logging and activates the platform
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    //Coroutine called to sign the user in 
    public IEnumerator SignIn(bool forcePrompt)
    {
        //Return if the user is already signed in
        if (_isSignedIn) yield break;

        //Stors if the authentication has completed
        bool completed = false;

        //Prompts the user to be authenticated if forcePrompt is enabled or the user has not been prompted yet.
        PlayGamesPlatform.Instance.Authenticate(forcePrompt ? SignInInteractivity.CanPromptAlways : SignInInteractivity.CanPromptOnce, (result) =>
        {
            //Set signed in to true if the sign in was successful
            _isSignedIn = result == SignInStatus.Success;

            //Flag that the authentication has completed
            completed = true;
        });

        //Wait until the authentication is completed
        yield return new WaitUntil(() => completed);
    }

    //Coroutine called to update the last time the user collected there 
    public IEnumerator UpdateLastCollectionTime()
    {
        //Trigger if the user is not signed in
        if (!_isSignedIn)
        {
            //Set the reward day to 0th day and return (no connection so can't get last collection time)
            Serializer.Instance.activeData.SetLastRewardDay(0);
            yield break;
        }

        //Stores if the get states callback has completed
        bool completed = false;

        //Gets the stats from play games server
        ((PlayGamesLocalUser)Social.localUser).GetStats((rc, stats) =>
        {
            //Trigger if status code is negative (success) and the stats have the days sinces last played
            if (rc <= 0 && stats.HasDaysSinceLastPlayed())
            {
                //Assign days since last player to last reward day
                Serializer.Instance.activeData.SetLastRewardDay(stats.DaysSinceLastPlayed);
            }
            else
            {
                //Else set the reward day to 0th day and return (no connection so can't get last collection time)
                Serializer.Instance.activeData.SetLastRewardDay(0);
            }

            //Flag that the stats callback has completed
            completed = true;
        });

        //Wait until the stats callback is completed
        yield return new WaitUntil(() => completed);
    }

    //Method to save a bytes to the cloud
    public void SaveGame(ISavedGameMetadata game, byte[] savedData)
    {
        //Instatiates a builder object
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

        //Builds the updates metadata
        SavedGameMetadataUpdate updatedMetadata = builder.Build();

        //Updates the save
        _client.CommitUpdate(
            game,
            updatedMetadata,
            savedData,
            //Callback
            (status, game) =>
            {
                //If the game was unable to save, log it to the console
                if (status != SavedGameRequestStatus.Success)
                    LogWarning("Unable to save game data");
            }
        );
    }

    //Coroutine to load game data from the server
    public IEnumerator LoadGame(string fileName)
    {
        //Prepare last save for resetting
        LastSave = null;

        //Stores if the attempt to load the data has completed
        bool completed = false;

        //Loads the game from the server
        _client.OpenWithAutomaticConflictResolution(
            fileName,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseMostRecentlySaved,
            //Callback
            (status, game) =>
            {
                //Trigger if the save game could not be loaded
                if (status != SavedGameRequestStatus.Success)
                {
                    //End the callback
                    completed = true;
                }
                else
                {
                    //Else read the binary data fromt he savegame file
                    _client.ReadBinaryData(game,
                    (status, data) =>
                    {
                        //Trigger if the file was successfully read
                        if (status == SavedGameRequestStatus.Success)
                        {
                            //Save the data
                            LastSave = data;
                        }

                        //End the callback
                        completed = true;
                    });
                }
            });

        //Wait for the load data to complete
        yield return new WaitUntil(() => completed);
    }

    //Method to show a leaderboard from an id
    public void ShowLeaderboard(string id)
    {
        //Shows the leaderboard
        PlayGamesPlatform.Instance.ShowLeaderboardUI(id);
    }

    //Method to add a score to a leaderboard
    public void AddLeaderboardScore(int score, string id)
    {
        //Adds the score to the leaderboard
        Social.ReportScore(score, id, (success) =>
        {
            //Log if it fails
            if (!success) Log($"Adding to leaderboard:{id}, failed");
        });
    }
}