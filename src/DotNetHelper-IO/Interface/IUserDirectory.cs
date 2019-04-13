namespace DotNetHelper_IO.Interface
{
    public interface IUserDirectory
    {
        IFolderObject GetUserFolder(DirectoryType type);
        bool DeleteAllUserFolder();
    }


    public enum DirectoryType
    {
          Files = 1
        , Videos = 2
        , Photos = 3
        , Audio = 4
        , Documents = 5
        , Sqls = 6
        , Other = 7
        , Root = 8
    }
}
