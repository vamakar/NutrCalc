using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NutrCalc.Infrastructure
{
    public static class DoubleClickCommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(DoubleClickCommand),
            new PropertyMetadata(null, OnCommandChanged));

        public static ICommand GetCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        private static void OnCommandChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (!(element is Control control))
            {
                return;
            }

            control.MouseDoubleClick -= OnMouseDoubleClick;

            if (e.NewValue is ICommand)
            {
                control.MouseDoubleClick += OnMouseDoubleClick;
            }
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ListView listView) || listView.SelectedItem == null)
            {
                return;
            }

            var command = GetCommand(listView);

            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}
