﻿namespace Core.Commands;

public interface ICommand
{
    public string Name { get; }
    public string Description { get; }

    public void Do();
    public void Undo();
}
