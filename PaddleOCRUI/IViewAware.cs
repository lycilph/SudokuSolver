﻿namespace PaddleOCRUI;

public interface IViewAware
{
    event EventHandler<bool> OnRequestClose;

    void WindowContentRendered();
    void WindowClosing();
}
