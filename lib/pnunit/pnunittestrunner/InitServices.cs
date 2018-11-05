using System;
using NUnit.Util;

namespace PNUnitTestRunner
{
    public class InitServices
    {
        public static void InitNUnitServices()
        {
            // initialize NUnit services
            // Add Standard Services to ServiceManager
            ServiceManager.Services.AddService(new SettingsService());
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new ProjectService());

            // initialize NUnit services
            // Add Standard Services to ServiceManager
            ServiceManager.Services.AddService(new SettingsService());
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new ProjectService());

            NUnit.Core.CoreExtensions.Host.InitializeService();

            // Initialize Services
            ServiceManager.Services.InitializeServices();
        }
    }
}