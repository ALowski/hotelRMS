using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Hotels.Config;
using Hotels.Configurator.Helpers;
using Xceed.Wpf.Toolkit;

namespace Hotels.Configurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDefoConfiguration _configuration = new FileDefoConfiguration();

        public MainWindow()
        {
            InitializeComponent();

            foreach (var tab in _configuration.ConfigurationRoot.ToDictionary())
            {
                var content = new StackPanel { Margin = new Thickness(15) };
                foreach (var prop in tab.Value.ToDictionary())
                {
                    InitializeObject(prop, content, tab.Value);
                }
                _tabs.Items.Add(new TabItem { Header = tab.Key, Content = new ScrollViewer { Content = content } });
            }
        }

        private static void InitializeObject(KeyValuePair<string, object> prop, Panel content, object parent)
        {
            content.Children.Add(new TextBlock { Text = prop.Key, Padding = new Thickness(0, 10, 0, 0) });
            if (prop.Value is int)
            {
                var input = new IntegerUpDown();
                BindingOperations.SetBinding(input, IntegerUpDown.ValueProperty, GetBinding(prop, parent));
                content.Children.Add(input);
            }
            else if (prop.Value is double)
            {
                var input = new DoubleUpDown();
                BindingOperations.SetBinding(input, DoubleUpDown.ValueProperty, GetBinding(prop, parent));
                content.Children.Add(input);
            }
            else if (prop.Value is IList)
            {
                ProcessList(prop, content);
            }
            else
            {
                var input = new TextBox { HorizontalContentAlignment = HorizontalAlignment.Right };
                BindingOperations.SetBinding(input, TextBox.TextProperty, GetBinding(prop, parent));
                content.Children.Add(input);
            }
        }

        private static void ProcessList(KeyValuePair<string, object> prop, Panel content)
        {
            var innerContent = AddNewStackPanelWithBorder(content).Item2;
            var list = (IList)prop.Value;
            foreach (var obj in list)
            {
                AddNewListItemControls(innerContent, obj, list);
            }

            AddNewButton(content, list, innerContent);
        }

        private static void AddNewButton(Panel content, IList list, Panel innerContent)
        {
            var addNewBtn = new Hyperlink(new Run(Properties.Resources.AddNew));
            addNewBtn.Click += (sender, args) =>
            {
                var newItem = Activator.CreateInstance(list.GetType().GenericTypeArguments.First());
                list.Add(newItem);
                AddNewListItemControls(innerContent, newItem, list);
            };
            content.Children.Add(new TextBlock(addNewBtn) {HorizontalAlignment = HorizontalAlignment.Right});
        }

        private static void AddNewListItemControls(Panel innerContent, object obj, IList list)
        {
            var objGroupContent = AddNewStackPanelWithBorder(innerContent);
            foreach (var item in obj.ToDictionary())
            {
                InitializeObject(item, objGroupContent.Item2, obj);
            }

            var removeBtn = new Hyperlink(new Run(Properties.Resources.Remove));
            var txt = new TextBlock(removeBtn) { HorizontalAlignment = HorizontalAlignment.Right };
            innerContent.Children.Add(txt);
            removeBtn.Click += (sender, args) =>
            {
                list.Remove(obj);
                innerContent.Children.Remove(objGroupContent.Item1);
                innerContent.Children.Remove(txt);
            };
        }

        private static Tuple<Border, StackPanel> AddNewStackPanelWithBorder(Panel content)
        {
            var innerContent = new StackPanel { Margin = new Thickness(10) };
            var border = new Border
            {
                Child = innerContent,
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.DarkGray),
                Margin = new Thickness(5)
            };
            content.Children.Add(border);
            return new Tuple<Border, StackPanel>(border, innerContent);
        }

        private static Binding GetBinding(KeyValuePair<string, object> prop, object parent)
        {
            var binding = new Binding
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Path = new PropertyPath(prop.Key),
                Source = parent
            };
            return binding;
        }

        private void _saveBtn_Click(object sender, RoutedEventArgs e)
        {
            _configuration.Flush();
        }
    }
}
