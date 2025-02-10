using FabTilemapEditor;
using Raylib_cs;


Raylib.InitWindow(Constants.screenWidth, Constants.screenHeight, "Hello World");
var tileset = new Tileset();

tileset.GameStartup();

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

    tileset.HandleInput();

    tileset.GameRender();

    Raylib.EndDrawing();
}

Raylib.CloseWindow();
