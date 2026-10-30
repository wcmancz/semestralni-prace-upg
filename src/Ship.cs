using System.IO;
using System.Collections.Generic;
using System;

namespace DefaultNamespace;

public enum LoadingStrategy
{
    Full,
    Immediate,
    MaxLoadtime
}

public enum State
{
    Loading,
    Flying
}

public class Ship
{
    private string Name { get; }
    private double Speed { get; }
    private int Capacity { get; }
    private int Transfer_speed { get; }
    private LoadingStrategy Strategy { get; }
    private int Max_loadtime { get; }

    private State state = State.Loading;
    private SpaceBalls current;
    private SpaceBalls? target;
    private int cargo = 0;
    private double x, y;

    private int loadDays = 0;
    private bool loadingPhase = false;

    private double dirX, dirY;

    public Ship(string name, double speed, SpaceBalls start, int capacity, int transferSpeed, LoadingStrategy strategy, int maxLoadtime)
    {
        Name = name;
        Speed = speed;
        Capacity = capacity;
        Transfer_speed = transferSpeed;
        Strategy = strategy;
        Max_loadtime = maxLoadtime;

        current = start;
    }

    public string GetName() => Name;
    public int GetCargo() => cargo;
    public int GetCapacity() => Capacity;
    public bool IsFlying() => state == State.Flying;
    public string GetTargetName() => target?.GetName() ?? "-";
    public string GetCurrentName() => current.GetName();
    public bool IsUnloading() => state == State.Loading && !loadingPhase;
    public bool IsLoading() => state == State.Loading && loadingPhase;

    public static LoadingStrategy ParseStrategy(string text) => text.ToUpperInvariant() switch
    {
        "FULL" => LoadingStrategy.Full,
        "IMMEDIATE" => LoadingStrategy.Immediate,
        "MAX_LOADTIME" => LoadingStrategy.MaxLoadtime,
        _ => throw new InvalidDataException($"Neznámá strategie '{text}'")
    };

    private List<Destination> OptionsFromHere(List<Destination> options)
    {
        var result = new List<Destination>();
        foreach (var option in options)
        {
            if (option.GetSource() == current)
            {
                result.Add(option);
            }
        }
        return result;
    }

    private int TotalProbability(List<Destination> options)
    {
        int total = 0;
        foreach (var option in options)
        {
            total += option.GetProbability();
        }
        return total;
    }

    public SpaceBalls? ChooseTarget(List<Destination> all, Random rng)
    {
        var options = OptionsFromHere(all);
        if (options.Count == 0)
        {
            return null;
        }

        int roll = rng.Next(TotalProbability(options));

        foreach (var option in options)
        {
            if (roll < option.GetProbability())
            {
                return option.GetDest();
            }
            roll -= option.GetProbability();
        }
        return null;
    }

    public (double X, double Y) GetPosition() => (x, y);

    public void Update(double day, double dt)
    {
        if (state == State.Loading)
        {
            (x, y) = current.GetPositionAt(day);
            return;
        }

        x += dirX * Speed * dt;
        y += dirY * Speed * dt;
    }

    public void DailyStep(List<Destination> destinations, Random rng, double day)
    {
        if (state == State.Flying)
        {
            var (tx, ty) = target!.GetPositionAt(day);
            double dx = tx - x;
            double dy = ty - y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            if (dist < Speed)
            {
                current = target;
                target = null;
                state = State.Loading;
                loadingPhase = false;
                loadDays = 0;
            }
            else
            {
                dirX = dx / dist;
                dirY = dy / dist;
            }
            return;
        }

        int budget = Transfer_speed;

        if (!loadingPhase)
        {
            int unload = Math.Min(budget, cargo);
            cargo -= unload;
            current.AddToWarehouse(unload);
            budget -= unload;

            if (cargo > 0) return;
            loadingPhase=true;
            if (budget == 0) return;
        }

        int wanted = Math.Min(budget, Capacity - cargo);
        int taken = current.TakeFromWarehouse(wanted);
        cargo += taken;
        loadDays++;

        bool full = cargo == Capacity;

        bool finished = Strategy switch
        {
            LoadingStrategy.Full => full,
            LoadingStrategy.Immediate => full || taken < wanted,
            LoadingStrategy.MaxLoadtime => full || loadDays > Max_loadtime,
            _ => false
        };

        if (finished)
        {
            target = ChooseTarget(destinations, rng);
            if (target != null)
            {
                var (tx, ty) = target.GetPositionAt(day);
                double dx = tx - x;
                double dy = ty - y;
                double dist = Math.Sqrt(dx * dx + dy * dy);
                dirX = dx / dist;
                dirY = dy / dist;
                state = State.Flying;
            }
        }
    }
}