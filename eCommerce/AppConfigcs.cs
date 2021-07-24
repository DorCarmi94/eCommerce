using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NLog;

namespace eCommerce
{
    public class AppConfig
    {
        private static AppConfig _instance = new AppConfig();
        private static Logger _logger;

        private IConfigurationRoot _config;
        private AppConfig()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appConfig.json")
                .Build();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public static AppConfig GetInstance()
        {
            return _instance;
        }
        
        public bool Init(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            _config = new ConfigurationBuilder()
                .AddJsonFile(filePath)
                .Build();
            return true;
        }

        public string GetData(string dataPath)
        {
            return _config?[dataPath];
        }

        public void ThrowErrorOfData(string data, string state)
        {
            string message = $"{data} data in the config file is {state}";
            _logger.Error(message);
            throw new InvalidDataException(message);
        }
    }
}