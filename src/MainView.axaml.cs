using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;

namespace SpaceTraffic;

public partial class MainView: UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var rect = new Rect(Bounds.Size);
        context.FillRectangle(Brushes.DarkBlue, rect);

	//TODO: vykreslení vesmirnych teles a transportnich lodi v aktualnim case
    }
}