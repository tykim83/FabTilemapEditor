using FabTilemapEditor.App;
using Raylib_cs;

namespace FabTilemapEditor.Desktop;

public class Program
{
    private static readonly IFileService _fileService = new AvaloniaFileService();

    public static void Main()
    {
        // Init App
        AvaloniaHelper.Initialize();
        RaylibApp.Init(_fileService);

        // Run App
        while (!Raylib.WindowShouldClose())
            RaylibApp.UpdateFrame();

        // Close App
        Raylib.CloseWindow();
    }
}
