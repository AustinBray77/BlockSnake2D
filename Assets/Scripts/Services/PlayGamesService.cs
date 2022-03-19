using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayGamesService : SingletonDD<PlayGamesService>
{
    public static string HighScoreID = "CgkIiJOyzo0MEAIQAQ";
    public byte[] LastSave { get; private set; } = null;
    private ISavedGameClient _client => PlayGamesPlatform.Instance.SavedGame;
    private static PlayGamesClientConfiguration s_config;
    private bool _isSignedIn = false;

    private void OnEnable()
    {
        s_config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();

        PlayGamesPlatform.InitializeInstance(s_config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public IEnumerator SignIn(bool forcePrompt)
    {
        if (_isSignedIn) yield break;

        _isSignedIn = false;
        bool completed = false;

        PlayGamesPlatform.Instance.Authenticate(forcePrompt ? SignInInteractivity.CanPromptAlways : SignInInteractivity.CanPromptOnce, (result) =>
        {
            _isSignedIn = result == SignInStatus.Success;
            completed = true;
        });

        yield return new WaitUntil(() => completed);
    }

    public IEnumerator UpdateLastCollectionTime()
    {
        if (!_isSignedIn)
        {
            Serializer.Instance.activeData.SetLastRewardDay(0);
            yield break;
        }

        bool completed = false;

        ((PlayGamesLocalUser)Social.localUser).GetStats((rc, stats) =>
        {
            if (rc <= 0 && stats.HasDaysSinceLastPlayed())
            {
                Serializer.Instance.activeData.SetLastRewardDay(stats.DaysSinceLastPlayed);
            }
            else
            {
                Serializer.Instance.activeData.SetLastRewardDay(0);
            }

            completed = true;
        });

        yield return new WaitUntil(() => completed);
    }

    public void SaveGame(ISavedGameMetadata game, byte[] savedData)
    {
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        _client.CommitUpdate(
            game,
            updatedMetadata,
            savedData,
            (status, game) =>
            {
                if (status != SavedGameRequestStatus.Success)
                    LogWarning("Unable to save game data");
            }
        );
    }

    public IEnumerator LoadGame(string fileName)
    {
        LastSave = null;
        bool completed = false;

        _client.OpenWithAutomaticConflictResolution(
            fileName,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseMostRecentlySaved,
            (status, game) =>
            {
                if (status != SavedGameRequestStatus.Success)
                {
                    completed = true;
                }
                else
                {
                    _client.ReadBinaryData(game,
                    (status, data) =>
                    {
                        if (status == SavedGameRequestStatus.Success)
                        {
                            LastSave = data;
                        }

                        completed = true;
                    });
                }
            });

        yield return new WaitUntil(() => completed);
    }

    public void ShowLeaderboard(string id)
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(id);
    }

    public void AddLeaderboardScore(int score, string id)
    {
        Social.ReportScore(score, id, (success) =>
        {
            if (!success) Log($"Adding to leaderboard:{id}, failed");
        });
    }
}