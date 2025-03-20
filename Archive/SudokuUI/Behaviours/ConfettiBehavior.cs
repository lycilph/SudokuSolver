using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace SudokuUI.Behaviours;

public class ConfettiBehavior : Behavior<Canvas>
{
    private readonly Random random = new();

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register("IsActive", typeof(bool), typeof(ConfettiBehavior),
            new PropertyMetadata(false, OnIsActiveChanged));
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ConfettiBehavior behavior)
        {
            if ((bool)e.NewValue)
            {
                // Dispatcher.BeginInvoke with DispatcherPriority.Render queues the animation to run
                // after the current frame’s layout and rendering, ensuring ActualWidth and ActualHeight
                // are updated.
                behavior.AssociatedObject.Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Double-check IsActive in case it changed
                    if (behavior.IsActive)
                        behavior.StartConfettiAnimation();
                }), DispatcherPriority.Render);
            }
            else
                behavior.StopConfettiAnimation();
        }
    }

    private async Task StartConfettiAnimation()
    {
        AssociatedObject.Children.Clear();
        
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 100; j++)
                CreateConfettiPiece();
            await Task.Delay(500);
        }
    }

    private void StopConfettiAnimation()
    {
        AssociatedObject.Children.Clear();
    }

    private void CreateConfettiPiece()
    {
        // Create confetti piece
        Rectangle confetti = new()
        {
            Width = random.Next(5, 15),
            Height = random.Next(5, 15),
            Fill = new SolidColorBrush(GetRandomColor()),
            RenderTransform = new RotateTransform(random.Next(0, 360))
        };

        // Position at top of screen with random x-coordinate
        Canvas.SetLeft(confetti, random.Next(0, (int)AssociatedObject.ActualWidth));
        Canvas.SetTop(confetti, -20);
        AssociatedObject.Children.Add(confetti);

        // Create falling animation
        DoubleAnimation fallAnimation = new()
        {
            To = AssociatedObject.ActualHeight + 20,
            Duration = TimeSpan.FromSeconds(random.NextDouble() * 2 + 1),
            EasingFunction = new QuadraticEase()
        };

        // Create side-to-side animation
        DoubleAnimation swayAnimation = new()
        {
            From = Canvas.GetLeft(confetti),
            To = Canvas.GetLeft(confetti) + random.Next(-50, 50),
            Duration = TimeSpan.FromSeconds(random.NextDouble() + 0.5),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };

        // Create rotation animation
        DoubleAnimation rotateAnimation = new()
        {
            To = random.Next(0, 360),
            Duration = TimeSpan.FromSeconds(random.NextDouble() + 1),
            RepeatBehavior = RepeatBehavior.Forever
        };

        confetti.RenderTransform = new RotateTransform();
        confetti.RenderTransformOrigin = new Point(0.5, 0.5);

        // Start animations
        confetti.BeginAnimation(Canvas.TopProperty, fallAnimation);
        confetti.BeginAnimation(Canvas.LeftProperty, swayAnimation);
        confetti.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
    }

    private Color GetRandomColor()
    {
        Color[] colors =
        [
                Colors.Red,
                Colors.Blue,
                Colors.Green,
                Colors.Yellow,
                Colors.Purple,
                Colors.Orange
        ];
        return colors[random.Next(colors.Length)];
    }
}