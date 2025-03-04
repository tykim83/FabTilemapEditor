﻿using FabTilemapEditor.App;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace FabTilemapEditor.Wasm;

public partial class BrowserFileService : IFileService
{
    [JSImport("PickFile", "interop.js")]
    public static partial Task<JSObject> PickFileInteropAsync();

    [JSImport("DownloadFile", "interop.js")]
    public static partial void DownloadFileInterop(string fileName, string dataBase64);

    public Task DownloadFileAsync(string fileName, string dataBase64)
    {
        DownloadFileInterop(fileName, dataBase64);

        return Task.CompletedTask;
    }

    public async Task<string> PickFileAsync()
    {
        var file = await PickFileInteropAsync();
        byte[] imageData = file?.GetPropertyAsByteArray("content") ?? [];

        string fileName = file?.GetPropertyAsString("name") ?? "temp.png";
        string filePath = "/tmp/" + fileName;
        System.IO.File.WriteAllBytes(filePath, imageData);

        return filePath;
    }
}
