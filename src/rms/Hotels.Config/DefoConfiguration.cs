using System;
using System.Collections.Generic;
using System.IO;
using Hotels.Config.ConfigModel;
using Newtonsoft.Json;

namespace Hotels.Config
{
    public interface IDefoConfiguration
    {
        ConfigurationRoot ConfigurationRoot { get; set; }
        void Flush();
    }
    public class DefoConfiguration : IDefoConfiguration
    {
        public DefoConfiguration(string jsonConfig)
        {
            if (string.IsNullOrEmpty(jsonConfig))
                ConfigurationRoot = new ConfigurationRoot();
            else
                ConfigurationRoot = JsonConvert.DeserializeObject<ConfigurationRoot>(jsonConfig);
        }
        public ConfigurationRoot ConfigurationRoot { get; set; }
        public void Flush()
        {
        }
    }
    public class FileDefoConfiguration : IDefoConfiguration
    {
        private readonly string _fileDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _fileName = "settings.json";
        private readonly object _locker = new object();
        private ConfigurationRoot _configurationRootObject;
        private FileSystemWatcher _fileSystemWatcher;

        public ConfigurationRoot ConfigurationRoot
        {
            get
            {
                if (_configurationRootObject == null)
                {
                    LoadConfig();
                    _fileSystemWatcher = new FileSystemWatcher(_fileDirectory) { EnableRaisingEvents = true };
                    _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
                }
                return _configurationRootObject;
            }
            set { _configurationRootObject = value; }
        }

        public void Flush()
        {
            lock (_locker)
            {
                _fileSystemWatcher.EnableRaisingEvents = false;
                File.WriteAllText(Path.Combine(_fileDirectory, _fileName), JsonConvert.SerializeObject(_configurationRootObject, Formatting.Indented));
            }
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if (fileSystemEventArgs.Name.Equals(_fileName, StringComparison.OrdinalIgnoreCase))
            {
                LoadConfig();
            }
        }

        private void LoadConfig()
        {
            lock (_locker)
            {
                var filePath = Path.Combine(_fileDirectory, _fileName);
                _configurationRootObject = File.Exists(filePath) ? JsonConvert.DeserializeObject<ConfigurationRoot>(File.ReadAllText(filePath)) : Default;
            }
        }

        private static ConfigurationRoot Default = new ConfigurationRoot
        {
            Seasons = new Seasons
            {
                Total = 2,
                SeasonDescriptions = new List<SeasonDescription>
                {
                    new SeasonDescription { Number = 0, Start = 1, Finish = 22 },
                    new SeasonDescription { Number = 1, Start = 23, Finish = 36 },
                    new SeasonDescription { Number = 0, Start = 37, Finish = 53 }
                },
                PriceReductions = new List<PriceReduction>
                {
                    new PriceReduction { Number = 0, Amount = 0.2 }
                }
            },
            Weekdays = new Weekdays
            {
                Total = 2,
                Monday = 0,
                Tuesday = 0,
                Wednesday = 0,
                Thursday = 0,
                Friday = 1,
                Saturday = 1,
                Sunday = 1,
                PriceReductions = new List<PriceReduction>
                {
                    new PriceReduction { Number = 0, Amount = 0.12 }
                }
            },
            RoomTypes = new RoomTypes
            {
                Total = 4,
                RoomTypeDescriptions = new List<RoomTypeDescription>
                {
                    new RoomTypeDescription { Number = 0, Quantity = 150, OperationalCost = 2, MarketBasePrice = 20, Description = "1E" },
                    new RoomTypeDescription { Number = 1, Quantity = 150, OperationalCost = 2, MarketBasePrice = 25, Description = "2E" },
                    new RoomTypeDescription { Number = 2, Quantity = 150, OperationalCost = 2, MarketBasePrice = 35, Description = "BK" },
                    new RoomTypeDescription { Number = 3, Quantity = 100, OperationalCost = 5, MarketBasePrice = 45, Description = "BK+" }
                },
                PriceConstraints = new List<PriceConstraint>
                {
                    new PriceConstraint { Less = 0, More = 1 },
                    new PriceConstraint { Less = 1, More = 2 },
                    new PriceConstraint { Less = 2, More = 3 }
                }
            },
            Categories = new Categories
            {
                Total = 4,
                PriceReductions = new List<PriceReduction>
                {
                    new PriceReduction { Number = 0, Amount = 0.08 },
                    new PriceReduction { Number = 1, Amount = 0.04 },
                    new PriceReduction { Number = 2, Amount = 0.04 }
                }
            }
        };
    }
}
