using FabTilemapEditor.Gui;
using FabTilemapEditor.Shared;
using Raylib_cs;
using System.Numerics;

namespace FabTilemapEditor;

//TODO: Need to unload the Texture2D
public class Layers
{
    const int PANEL_X = 0;
    const int PANEL_Y = 700;
    const int PANEL_WIDTH = 600;
    const int PANEL_HEIGHT = 380;

    private Texture2D gearIcon;
    private Texture2D eyeIcon;
    private Texture2D visibleIcon;
    private TextButton? button;

    // Layers Properties
    public int ActiveLayer { get; private set; } = 0;
    public List<LayerPanel> LayerPanels { get; private set; } = [];

    // Drag fields
    private int? draggingLayerIndex = null;
    private float dragOffsetY = 0;
    private float dragStartTime = 0f;
    private bool isDragging = false;
    private const float DRAG_DELAY = 0.20f;

    // Setup Tilemap Callbacks
    Action<int>? addLayerCallback;
    Action<int>? renameLayerCallback;
    Action<int>? clearLayerCallback;
    Action<int>? removeLayerCallback;
    Action<int>? toggleLayerVisibilityCallback;
    Action? notifyLayerSwapCallback;

    // Modals
    public List<TextInputModal> InputModals
    {
        get
        {
            var modals = new List<TextInputModal>();
            foreach (var layer in LayerPanels)
            {
                if (layer != null && layer.InputModal is not null)
                {
                    modals.Add(layer.InputModal);
                }
            }
            return modals;
        }
    }

    public void SetupAddLayerCallback(Action<int> action) => addLayerCallback = action;
    public void SetupRenameLayerCallback(Action<int> action) => renameLayerCallback = action;
    public void SetupClearLayerCallback(Action<int> action) => clearLayerCallback = action;
    public void SetupRemoveLayerCallback(Action<int> action) => removeLayerCallback = action;
    public void SetupToggleLayerVisibilityCallback(Action<int> action) => toggleLayerVisibilityCallback = action;
    public void SetupNotifyLayerSwapCallback(Action action) => notifyLayerSwapCallback = action;

    public void GameStartup()
    {
        // Init Button
        var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;
        var height = (int)availableSpace.Height;
        button = new TextButton(startingX + 10, startingY + height - 50, 130, 30, "Add Layer", () => AddLayer("New Layer"));

        // Load Gear Icons
        Image image = Raylib.LoadImage("Assets/gear_icon.png");
        Raylib.ImageResize(ref image, 28, 28);
        gearIcon = Raylib.LoadTextureFromImage(image);
        Raylib.UnloadImage(image);

        // Load Eye Icons
        Image imageEye = Raylib.LoadImage("Assets/eye.png");
        Raylib.ImageResize(ref imageEye, 28, 28);
        eyeIcon = Raylib.LoadTextureFromImage(imageEye);
        Raylib.UnloadImage(imageEye);

        // Load Visible Icons
        Image imageVisible = Raylib.LoadImage("Assets/visible.png");
        Raylib.ImageResize(ref imageVisible, 28, 28);
        visibleIcon = Raylib.LoadTextureFromImage(imageVisible);
        Raylib.UnloadImage(imageVisible);

        // Init Layers
        AddLayer("Background", true);
    }

    public void HandleInput()
    {
        button?.Update();

        for (int i = 0; i < LayerPanels.Count; i++)
        {
            LayerPanel? layerPanel = LayerPanels[i];
            layerPanel.Update();
        }

        Vector2 mousePos = Raylib.GetMousePosition();
        if (!Raylib.CheckCollisionPointRec(mousePos, new Rectangle(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT)))
            return;

        // Init Drag Layer with Delay
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            for (int i = 0; i < LayerPanels.Count; i++)
            {
                if (Raylib.CheckCollisionPointRec(mousePos, LayerPanels[i].Rect))
                {
                    draggingLayerIndex = i;
                    dragOffsetY = mousePos.Y - LayerPanels[i].Rect.Y;
                    dragStartTime = (float)Raylib.GetTime();
                    isDragging = false;
                    break;
                }
            }
        }

        // Start Drag Layer
        if (draggingLayerIndex.HasValue && !isDragging)
        {
            if (Raylib.GetTime() - dragStartTime > DRAG_DELAY)
                isDragging = true;
        }

        // Drag Layer 
        if (isDragging && draggingLayerIndex.HasValue)
        {
            int index = draggingLayerIndex.Value;
            var draggedRect = LayerPanels[index].Rect;
            draggedRect.Y = mousePos.Y - dragOffsetY;
            LayerPanels[index].Rect = draggedRect;

            // Swap layers
            for (int i = 0; i < LayerPanels.Count; i++)
            {
                if (i != index)
                {
                    float halfHeight = LayerPanels[i].Rect.Height / 2;
                    float centerY = LayerPanels[i].Rect.Y + halfHeight;

                    if ((index < i && LayerPanels[index].Rect.Y < centerY) ||
                        (index > i && LayerPanels[index].Rect.Y + LayerPanels[index].Rect.Height > centerY))
                    {
                        if (ActiveLayer == i)
                            ActiveLayer = index;
                        else if (ActiveLayer == index)
                            ActiveLayer = i;

                        (LayerPanels[index].Index, LayerPanels[i].Index) = (i, index);
                        UpdateLayerReacts();
                        draggingLayerIndex = i;
                        break;
                    }
                }
            }
        }

        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            // Set active if only click
            if (!isDragging && draggingLayerIndex.HasValue)
            {
                for (int i = 0; i < LayerPanels.Count; i++)
                {
                    if (Raylib.CheckCollisionPointRec(mousePos, LayerPanels[i].Rect))
                    {
                        LayerPanels[ActiveLayer].ToggleActive();
                        LayerPanels[i].ToggleActive();
                        ActiveLayer = i;
                        break;
                    }
                }
            }

            // Release Drag Layer
            draggingLayerIndex = null;
            dragOffsetY = 0;
            isDragging = false;

            UpdateLayerReacts();

            // Tilemap Callbacks
            if (isDragging && draggingLayerIndex.HasValue)
                notifyLayerSwapCallback?.Invoke();
        }
    }

    public void GameRender()
    {
        GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");

        foreach (var layerPanel in LayerPanels)
            layerPanel.Draw();

        button?.Draw();
    }

    private void UpdateLayerReacts()
    {
        var availableSpace = GuiUtilities.RenderSectionUI(PANEL_X, PANEL_Y, PANEL_WIDTH, PANEL_HEIGHT, "Layers");
        var startingX = (int)availableSpace.X;
        var startingY = (int)availableSpace.Y;
        var width = (int)availableSpace.Width;

        LayerPanels = [.. LayerPanels.OrderBy(c => c.Index)];

        for (var i = 0; i < LayerPanels.Count; i++)
        {
            var drawIndex = (LayerPanels.Count - 1) - i;
            LayerPanels[i].Rect = new Rectangle(startingX + 20, startingY + (drawIndex * 35) + 20, width - 40, 32);
            LayerPanels[i].GameStartup();
        }
    }

    private void AddLayer(string layerName, bool isActive = false)
    {
        var index = LayerPanels.Count;
        var layerPanel = new LayerPanel(new Rectangle(), layerName, index, LayerPanelAction, gearIcon, eyeIcon, visibleIcon);
        if (isActive)
            layerPanel.ToggleActive();
        LayerPanels.Add(layerPanel);
        UpdateLayerReacts();

        // Tilemap Callbacks
        addLayerCallback?.Invoke(index);
    }

    // LayerPanel Callback Handler
    private void LayerPanelAction(LayerPanelState actionType, int index)
    {
        var active = actionType switch
        {
            LayerPanelState.Remove => RemoveLayer(index),
            LayerPanelState.Clear => ClearLayer(index),
            LayerPanelState.Visible => ToggleLayerVisibility(index),
            LayerPanelState.Rename => RenameLayer(index),
            _ => 0
        };

        if (active == index) return;

        ActiveLayer = active;
        LayerPanels[ActiveLayer].ToggleActive();
    }

    // Tilemap Callbacks
    private int RenameLayer(int index)
    {
        renameLayerCallback?.Invoke(index);

        return index;
    }

    private int RemoveLayer(int index)
    {
        LayerPanels.RemoveAt(index);
        UpdateLayerReacts();

        removeLayerCallback?.Invoke(index);

        return 0;
    }

    private int ClearLayer(int index)
    {
        clearLayerCallback?.Invoke(index);

        return index;
    }

    private int ToggleLayerVisibility(int index)
    {
        toggleLayerVisibilityCallback?.Invoke(index);

        return index;
    }
}
