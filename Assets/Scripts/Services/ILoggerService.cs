//Interface for logger
public interface ILoggerService
{
    //Method to log objects of different priority
    public void Log(object data, bool forceLog = false);
    public void LogWarning(object data, bool forceLog = false);
    public void LogError(object data, bool forceLog = false);
}