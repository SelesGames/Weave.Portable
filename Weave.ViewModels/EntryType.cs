
namespace Weave.ViewModels
{
    // Mark Entry Types: 0 = none, 1 = mark, 2 = extend previous mark

    public enum EntryType
    {
                        // MARK ENTRY   |   REFRESH
        Peek,           //      0               0
//      PeekRefresh,    //      0               1       NOT USED
        Mark,           //      1               0
//      MarkRefresh,    //      1               1       NOT USED
//      Extend,         //      2               0       NOT USED
        ExtendRefresh   //      2               1
    }
}
