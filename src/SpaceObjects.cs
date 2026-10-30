using Avalonia;
using Avalonia.Media;
using DefaultNamespace;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SpaceTraffic;

public class SpaceObjects
{
    private readonly LabelLayer labels;
    private static readonly Pen orbit_pen = new(Brushes.Gray, 1);
    private static readonly Pen arrow_pen = new(Brushes.White, 1.5)
    {
        LineJoin = PenLineJoin.Miter,
        LineCap = PenLineCap.Round,
    };

    public SpaceObjects(LabelLayer labels)
    {
        this.labels = labels;
    }

    public void DrawBodies(
        DrawingContext context, 
        IEnumerable<SpaceBalls> objs, 
        double day, 
        Point center, 
        double scale, 
        double maxWarehouse,
        double sizeFactor)
    {
        foreach (var obj in objs)
        {
            var (x, y) = obj.GetPositionAt(day);
            var position = new Point(center.X + x * scale, center.Y - y * scale);

            double planet_radius = Math.Max(2, obj.GetSize() * scale * sizeFactor);
            double planet_orbit_radius = obj.GetOrbitRadius() * scale;

            var parent = obj.GetParent();

            // vykreslení orbit těles
            if (parent != null && obj.GetBallType() != "asteroid")
            {
                var (px, py) = parent.GetPositionAt(day);
                var orbit_position = new Point(center.X + px * scale, center.Y - py * scale);

                context.DrawEllipse(null, orbit_pen, orbit_position, planet_orbit_radius, planet_orbit_radius);
            }

            IBrush planet_brush = obj.GetBallType() switch
            {
                "planet" => obj.GetName() switch
                {
                    "Earth" => Brushes.Green,
                    "Mars" => Brushes.Red,
                    "Jupiter" or "Saturn" => Brushes.Orange,
                    _ => Brushes.Black
                },
                "moon" => Brushes.Gray,
                "star" => Brushes.Gold,
                "asteroid" => Brushes.Brown,
                _ => Brushes.Black
            };
            // vykreslení těles a jejich bublina
            context.DrawEllipse(planet_brush, null, position, planet_radius, planet_radius);
            if (obj.GetBallType() != "star")
            {   // vykreslení pouze těles co nejsou hvězda zatím
                labels.Add($"{obj.GetName()}\nsklad: {obj.GetWarehouse()}",
                   new Point(position.X + planet_radius, position.Y - planet_radius),
                   obj.GetWarehouse() / maxWarehouse, Brushes.DeepSkyBlue);
            }

        }
    }
    public void DrawShips(DrawingContext context, IEnumerable<Ship> ships, Point center, double scale)
    {
        foreach (var ship in ships)
        {
            // přilepení lodě k tělesu
            var (x, y) = ship.GetPosition();
            var position = new Point(center.X + x * scale, center.Y - y * scale);
            context.DrawRectangle(Brushes.White, null, new Rect(position.X - 3, position.Y - 3, 6, 6));

            var top = new Point(position.X - 12, position.Y -9);
            var bottom = new Point(position.X - 12, position.Y +9);

            if (ship.IsLoading())
            {
                DrawArrow(context, bottom, top, 7);
            }
            else if (ship.IsUnloading())
            {
                DrawArrow(context, top, bottom, 7);
            }

            // informace k bublince
            string status = ship.IsFlying()
                ? $"letí k: {ship.GetTargetName()}"
                : $"na: {ship.GetCurrentName()}";

            // přidání do labels
            labels.Add($"{ship.GetName()}\nnáklad: {ship.GetCargo()}/{ship.GetCapacity()}\n{status}",
                       new Point(position.X + 3, position.Y - 3),
                       (double)ship.GetCargo() / ship.GetCapacity(), Brushes.LimeGreen);
        }
    }

    public void DrawArrow(DrawingContext context, Point A, Point B, double tip_length)
    {
        Vector u = B - A;
        double len = Math.Sqrt(u.X * u.X + u.Y * u.Y);
        if (len == 0) return;
        u = new Vector(u.X / len, u.Y / len);

        Point C = B - u * tip_length;
        double d = 0.25 * tip_length * 2;
        Vector v = new Vector(-u.Y, u.X);

        Point D = C + v * d;
        Point E = C - v * d;

        context.DrawLine(arrow_pen, A, B);

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(D, false);
            ctx.LineTo(B);
            ctx.LineTo(E);
            ctx.EndFigure(false);
        }
        context.DrawGeometry(null, arrow_pen, geometry);
    }

    public void DrawDate(DrawingContext context, DateTime date)
    {
        var formatted = new FormattedText(
            $"{date:d. M. yyyy}",
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Inter"),
            16,
            Brushes.White);
        context.DrawText(formatted, new Point(10, 10));
    }
}