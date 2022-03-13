public interface ILoggerService
{
    public void Log(object data);
    public void LogWarning(object data);
    public void LogError(object data);
}