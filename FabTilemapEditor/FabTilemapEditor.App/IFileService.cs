namespace FabTilemapEditor.App;

public interface IFileService
{
    Task<string> PickFileAsync();
    Task DownloadFileAsync(string fileName, string dataBase64);
}
