﻿using System;
using Commoner.Core.Testing;
using log4net.Config;
using NUnit.Framework;
using System.IO;

namespace Abot.Tests.Integration
{
    [SetUpFixture]
    public class AssemblySetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var dir = Path.GetDirectoryName(typeof(AssemblySetup).Assembly.Location);
            Directory.SetCurrentDirectory(dir);

            XmlConfigurator.Configure();

            FiddlerProxyUtil.StartAutoRespond(@"..\..\..\TestResponses.saz");
            Console.WriteLine("Started FiddlerCore to autorespond with pre recorded http responses.");
        }

        [OneTimeTearDown]
        public void After()
        {
            FiddlerProxyUtil.StopAutoResponding();
            Console.WriteLine("Stopped FiddlerCore");
        }
    }
}
