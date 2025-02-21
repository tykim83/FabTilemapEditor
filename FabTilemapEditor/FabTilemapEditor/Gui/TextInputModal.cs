using FabTilemapEditor.Shared;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor.Gui;

public class TextInputModal(string inputText, Action<TextInputModalState, string> onConfirm)
{
    private static readonly int boxWidth = 500;
    private static readonly int boxHeight = 100;
    private static readonly int boxX = (Raylib.GetScreenWidth() - boxWidth) / 2;
    private static readonly int boxY = (Raylib.GetScreenHeight() - boxHeight) / 2;
    private static readonly Rectangle inputBoxRect = new(boxX, boxY, boxWidth, boxHeight);

    private const int MAX_INPUT_LENGTH = 30;

    public void Update()
    {
        int key = Raylib.GetCharPressed();
        while (key > 0)
        {
            if (key >= 32 && key <= 126 && inputText.Length < MAX_INPUT_LENGTH)
            {
                inputText += (char)key;
            }
            key = Raylib.GetCharPressed();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && inputText.Length > 0)
            inputText = inputText.Substring(0, inputText.Length - 1);

        if (Raylib.IsKeyPressed(KeyboardKey.Enter) && inputText.Length > 0)
            onConfirm?.Invoke(TextInputModalState.Submit, inputText);

        if (Raylib.IsKeyPressed(KeyboardKey.Delete))
            onConfirm?.Invoke(TextInputModalState.Close, inputText);

        Vector2 mousePos = Raylib.GetMousePosition();
        if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !Raylib.CheckCollisionPointRec(mousePos, inputBoxRect))
            onConfirm?.Invoke(TextInputModalState.Close, inputText);
    }

    public void Draw()
    {
        // Display background
        Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), new Color(0, 0, 0, 150));

        // Display text input box
        Raylib.DrawRectanglePro(inputBoxRect, new Vector2(0, 0), 0, Color.DarkGray);
        Raylib.DrawRectangleLines(boxX, boxY, boxWidth, boxHeight, Color.White);

        // Display input text
        int textWidth = Raylib.MeasureText(inputText, 24);
        Raylib.DrawText(inputText, boxX + boxWidth / 2 - textWidth / 2, boxY + 40, 24, Color.White);

        // Display instructions
        Raylib.DrawText("Press Enter to confirm, Esc to cancel", boxX + 10, boxY + boxHeight - 30, 16, Color.LightGray);
    }
}
