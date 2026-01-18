namespace AvaloniaLib.Models;

public struct LogItem
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    public required string Message { get; set; }
    
    public LogType LogType { get; set; } =  LogType.Info;
    
    public LogItem() {}
    
    public override string ToString() => $"[{Timestamp:HH:mm:ss}] [{LogType}] {Message}";
}