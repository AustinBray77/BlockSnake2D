public interface ILoggerService
{
    public void Log(object data, bool forceLog = false);
    public void LogWarning(object data, bool forceLog = false);
    public void LogError(object data, bool forceLog = false);
}