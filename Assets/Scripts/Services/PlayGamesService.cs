using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayGamesService : Singleton<PlayGamesService>, IUnityService
{
    private PlayGamesClientConfiguration config;
    private bool isSignedIn = false;

    public void InitializeUnityService()
    {
        config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public IEnumerator SignIn(bool forcePrompt)
    {
        if (isSignedIn) yield break;

        isSignedIn = false;
        bool completed = false;

        PlayGamesPlatform.Instance.Authenticate(forcePrompt ? SignInInteractivity.CanPromptAlways : SignInInteractivity.CanPromptOnce, (result) =>
        {
            isSignedIn = result == SignInStatus.Success;
            completed = true;
        });

        yield return new WaitUntil(() => completed);
    }

    public IEnumerator UpdateLastCollectionTime()
    {
        if (!isSignedIn)
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
        ISavedGameClient client = PlayGamesPlatform.Instance.SavedGame;
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        client.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status != SavedGameRequestStatus.Success) Log("Unable to save game data");
    }
}