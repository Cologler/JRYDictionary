using System;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows;
using Jasily.Data.Db.MongoDb;
using JryDictionary.DbAccessors;
using JryDictionary.Properties;
using MongoDB.Driver;

namespace JryDictionary
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private MongoClient mongoClient;
        private IMongoDatabase mongoDatabase;

        public new static App Current { get; private set; }

        public App()
        {
            Debug.Assert(Current == null);
            Current = this;
        }

        public ThingSetAccessor ThingSetAccessor { get; private set; }

        public SettingSetAccessor SettingSetAccessor { get; private set; }

        public CompositionContainer CompositionContainer { get; private set; }

        #region Overrides of Application

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(this.GetType().Assembly));
            this.CompositionContainer = new CompositionContainer(catalog);

            if (string.IsNullOrWhiteSpace(Settings.Default.MongoDbConnectionSettingFile))
            {
                WriteLog("mongodb connection setting empty.");
                return;
            }
            var file = new FileInfo(Settings.Default.MongoDbConnectionSettingFile);
            if (!file.Exists)
            {
                WriteLog("mongodb connection file not exists.");
                return;
            }
            var model = file.JsonFileToObject<MongoDbConnectionModel>();
            var builder = new MongoUrlBuilder
            {
                Server = MongoServerAddress.Parse(model.LoginAddress),
                DatabaseName = model.LoginDatabaseName,
                Username = model.LoginUserName,
                Password = model.LoginUserPassword
            };
            this.mongoClient = new MongoClient(builder.ToMongoUrl());
            this.mongoDatabase = this.mongoClient.GetDatabase("JryDictionary");
            this.ThingSetAccessor = new ThingSetAccessor(this.mongoDatabase);
            this.SettingSetAccessor = new SettingSetAccessor(this.mongoDatabase);
            this.ThingSetAccessor.Initialize();
        }

        #endregion

        public static void WriteLog(string line)
        {
            if (line == null) return;
            File.AppendAllText("dict.log", $"{DateTime.Now.ToString("u")} {line}");
        }
    }
}
