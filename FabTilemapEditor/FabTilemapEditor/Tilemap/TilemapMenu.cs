using FabTilemapEditor.Gui;
using FabTilemapEditor.Shared;

namespace FabTilemapEditor.Tilemap;

public class TilemapMenu(int x, int y, int tilesWidht, int tilesHeight, Action<TilemapMenuState, int> action)
{
    private TextLabel? tilesWidthLabel;
    private TextLabel? tilesHeightLabel;
    private TextButton? tilesWidthButton;
    private TextButton? tilesHeightButton;

    private TilemapMenuState? menuState = null;

    public TextInputModal? InputModal { get; private set; } = null;

    public void GameStartup()
    {
        tilesWidthLabel = new TextLabel(x + 20, y + 10, 150, 30, $"Widht(tiles): {tilesWidht}", false, true);
        tilesHeightLabel = new TextLabel(x + 310, y + 10, 150, 30, $"Height(tiles): {tilesHeight}", false, true);
        tilesWidthButton = new TextButton(x + 190, y + 10, 100, 30, "Edit Width", () => { menuState = TilemapMenuState.EditTilesWidth; }, false);
        tilesHeightButton = new TextButton(x + 480, y + 10, 100, 30, "Edit Height", () => { menuState = TilemapMenuState.EditTilesHeight; }, false);
    }

    public void Update()
    {
        tilesHeightButton?.Update();
        tilesWidthButton?.Update();
        InputModal?.Update();

        if (menuState is not null && InputModal is null)
        {
            var value = menuState.Value is TilemapMenuState.EditTilesWidth ? tilesWidht : tilesHeight;
            InputModal = new TextInputModal(value.ToString(), RemameLayer);
        }
    }

    public void GameRender()
    {
        tilesWidthLabel?.Draw();
        tilesHeightLabel?.Draw();
        tilesWidthButton?.Draw();
        tilesHeightButton?.Draw();
    }

    private void RemameLayer(TextInputModalState state, string text)
    {
        InputModal = null;
        var isValid = int.TryParse(text, out var result);

        if (state is TextInputModalState.Close || !isValid || menuState is null)
        {
            menuState = null;
            return;
        }

        if (menuState is TilemapMenuState.EditTilesWidth)
        {
            tilesWidht = result;
            tilesWidthLabel = new TextLabel(x + 20, y + 10, 150, 30, $"Widht(tiles): {tilesWidht}", false, true);
        }
        else if (menuState is TilemapMenuState.EditTilesHeight)
        {
            tilesHeight = result;
            tilesHeightLabel = new TextLabel(x + 310, y + 10, 150, 30, $"Height(tiles): {tilesHeight}", false, true);
        }

        action.Invoke(menuState.Value, result);
        menuState = null;
    }
}
