using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Hotels.Config;
using Hotels.Core;
using Hotels.Core.ForecastRounders;
using Hotels.Core.QuadraticProgramming.ProblemSolvers;
using Hotels.Core.QuadraticProgramming.ProblemTransformers;
using Hotels.Data;

namespace Hotels.UiHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
        }

        private Solver _solver;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            var planningHorizon = int.Parse(ConfigurationManager.AppSettings["planningHorizon"]);  // 365
            var pastPeriod = int.Parse(ConfigurationManager.AppSettings["pastPeriod"]);
            var startDate = DateTime.ParseExact(ConfigurationManager.AppSettings["startDate"], "yyyy.MM.dd",
                CultureInfo.InvariantCulture);  // new DateTime(2011, 6, 24);
            var config = new DefoConfiguration();
            
                
                _solver = new Solver(startDate, planningHorizon, pastPeriod,
                    new DummyDataProvider(startDate, ConfigurationManager.AppSettings["orders"], ConfigurationManager.AppSettings["inflation"], config),
                    new MatLabProblemSolver(),
                    config,
                    new FloorForecastRounder(),
                    new MatLabProblemTransformer());

                _solver.Run();

                //#region ExcelResults
                ////price and load
                //List<List<string[]>> strings = new List<List<string[]>>();
                //var hasValues = true;
                //var item = new List<string[]>();
                //item.Add(null);
                //strings.Add(item);

                //for (int d = 0; d < planningHorizon; ++d)
                //{
                //    item = new List<string[]>();
                //    var text = startDate.AddDays(d).ToShortDateString();
                //    string[] st = new string[2] { text, text };
                //    item.Add(st);
                //    strings.Add(item);
                //}
                //double profit = 0;
                //for (var r = 0; r < _solver.TotalRoomTypes; ++r)
                //    for (var c = 0; c < _solver.TotalCategories; ++c)
                //        for (var m = 0; m < _solver.TotalMealTypes; ++m)
                //        {
                //            hasValues = false;
                //            string text = (r + 1).ToString() + " " + (c + 1).ToString() + " " + (m + 1).ToString();
                //            item = strings.First();
                //            item.Add(new string[2] { text, text });
                //            for (int d = 0; d < planningHorizon; ++d)
                //            {
                //                item = strings.ElementAt(d + 1);
                //                var price = _solver.GetPrice(startDate.AddDays(d), r, c, m);
                //                if (price > 0)
                //                {
                //                    var load = _solver.GetExpectedLoad(startDate.AddDays(d), r, c, m);
                //                    profit += load * (price - _solver.GetOperationalCost(r, m));
                //                    item.Add(new string[2] { price.ToString("F2"), load.ToString() });
                //                    hasValues = true;
                //                }
                //                else
                //                    item.Add(new string[2] { "0", "0" });
                //            }
                //            if (!hasValues)
                //                strings.ForEach(k => k.Remove(k.Last()));
                //        }
                //System.IO.File.WriteAllLines(startDate.ToShortDateString() + ".csv", strings.Select(c => string.Join(";", c.Select(k => k == null ? null : k[0]).ToArray())));
                //System.IO.File.AppendAllLines(startDate.ToShortDateString() + ".csv", strings.Select(c => string.Join(";", c.Select(k => k == null ? null : k[1]).ToArray())));
                //System.IO.File.AppendAllText(startDate.ToShortDateString() + ".csv", "\n");
                //System.IO.File.AppendAllText(startDate.ToShortDateString() + ".csv", "Profit: "+ profit.ToString());
                //#endregion
            
            #region PrintResults

            PricesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            PriceCushionsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            LoadPredictionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            for (int i = 0; i < _solver.TotalCategories * _solver.TotalRoomTypes * _solver.TotalMealTypes; ++i)
            {
                PricesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
                PriceCushionsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
                LoadPredictionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            }
            PricesGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });
            PriceCushionsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });
            LoadPredictionGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });
            for (int i = 0; i < planningHorizon; ++i)
            {
                PricesGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });
                PriceCushionsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });
                LoadPredictionGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });
            }

            for (int d = 0; d < planningHorizon; ++d)
            {
                TextBlock tx1 = new TextBlock { Text = startDate.AddDays(d).ToShortDateString() };
                Grid.SetRow(tx1, d + 1);
                Grid.SetColumn(tx1, 0);
                PricesGrid.Children.Add(tx1);

                TextBlock tx2 = new TextBlock { Text = startDate.AddDays(d).ToShortDateString() };
                Grid.SetRow(tx2, d + 1);
                Grid.SetColumn(tx2, 0);
                PriceCushionsGrid.Children.Add(tx2);

                TextBlock tx3 = new TextBlock { Text = startDate.AddDays(d).ToShortDateString() };
                Grid.SetRow(tx3, d + 1);
                Grid.SetColumn(tx3, 0);
                LoadPredictionGrid.Children.Add(tx3);
            }

            string[] rooms = new string[config.ConfigurationRoot.RoomTypes.Total];

            foreach (var r in config.ConfigurationRoot.RoomTypes.RoomTypeDescriptions
                .Where(r => !string.IsNullOrEmpty(r.Description)))
            {
                rooms[r.Number] = r.Description;
            }

            var categories = new string[config.ConfigurationRoot.Categories.Total];

            if (config.ConfigurationRoot.Categories.Descriptions != null)
            {
                foreach (var c in config.ConfigurationRoot.Categories.Descriptions
                    .Where(c => !string.IsNullOrEmpty(c.Description)))
                {
                    categories[c.Number] = c.Description;
                }
            }

            for (var r = 0; r < _solver.TotalRoomTypes; ++r)
                for (var c = 0; c < _solver.TotalCategories; ++c)
                    for (var m = 0; m < _solver.TotalMealTypes; ++m)
                    {
                        string text = (rooms[r] ?? (r + 1).ToString()) + "-" + (categories[c] ?? (c + 1).ToString()) + "-" + (m + 1).ToString();
                        int index = 1 + r * _solver.TotalCategories * _solver.TotalMealTypes + c * _solver.TotalMealTypes + m;
                        var tx1 = new TextBlock { Text = text };
                        Grid.SetRow(tx1, 0);
                        Grid.SetColumn(tx1, index);
                        PricesGrid.Children.Add(tx1);

                        var tx2 = new TextBlock { Text = text };
                        Grid.SetRow(tx2, 0);
                        Grid.SetColumn(tx2, index);
                        PriceCushionsGrid.Children.Add(tx2);

                        var tx3 = new TextBlock { Text = text };
                        Grid.SetRow(tx3, 0);
                        Grid.SetColumn(tx3, index);
                        LoadPredictionGrid.Children.Add(tx3);
                    }

            for (int d = 0; d < planningHorizon; ++d)
                for (int r = 0; r < _solver.TotalRoomTypes; ++r)
                    for (int c = 0; c < _solver.TotalCategories; ++c)
                        for (int m = 0; m < _solver.TotalMealTypes; ++m)
                        {
                            int index = 1 + r * _solver.TotalCategories * _solver.TotalMealTypes + c * _solver.TotalMealTypes + m;
                            TextBlock tx1 = new TextBlock { Text = _solver.GetPrice(startDate.AddDays(d), r, c, m).ToString("F2") };
                            Grid.SetRow(tx1, d + 1);
                            Grid.SetColumn(tx1, index);
                            PricesGrid.Children.Add(tx1);

                            TextBlock tx2 = new TextBlock { Text = _solver.GetPriceCushion(startDate.AddDays(d), r, c, m).ToString("F2") };
                            Grid.SetRow(tx2, d + 1);
                            Grid.SetColumn(tx2, index);
                            PriceCushionsGrid.Children.Add(tx2);

                            TextBlock tx3 = new TextBlock { Text = _solver.GetExpectedLoad(startDate.AddDays(d), r, c, m).ToString() };
                            Grid.SetRow(tx3, d + 1);
                            Grid.SetColumn(tx3, index);
                            LoadPredictionGrid.Children.Add(tx3);
                        }
            ((Button)sender).IsEnabled = false;

            #endregion
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
