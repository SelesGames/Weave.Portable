
namespace Common.Compression
{
    /// <summary>
    /// Specifies the app-level Compression settings.  This is needed because on every 
    /// platform, Compression is handled by native code for that particular platform, 
    /// making true Portable Class Library method insufficient
    /// </summary>
    public static class Settings
    {
        public static CompressionHandlerCollection CompressionHandlers { get; set; }
    }
}
