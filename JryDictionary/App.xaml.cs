using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows;
using Jasily.ComponentModel;
using Jasily.Data.Db.MongoDb;
using JryDictionary.Models;
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

        public IMongoCollection<Thing> ThingCollection { get; private set; }

        #region Overrides of Application

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var model = new FileInfo(Settings.Default.MongoDbConnectionSettingFile)
                .JsonFileToObject<MongoDbConnectionModel>();
            var builder = new MongoUrlBuilder
            {
                Server = MongoServerAddress.Parse(model.LoginAddress),
                DatabaseName = model.LoginDatabaseName,
                Username = model.LoginUserName,
                Password = model.LoginUserPassword
            };
            this.mongoClient = new MongoClient(builder.ToMongoUrl());
            this.mongoDatabase = this.mongoClient.GetDatabase("JryDictionary");
            this.ThingCollection = this.mongoDatabase.GetCollection<Thing>("Thing");
            var index = new IndexKeysDefinitionBuilder<Thing>().Ascending(
                PropertySelector<Thing>.Start(z => z).SelectMany(z => z.Words).Select(z => z.Text).ToString());
            this.ThingCollection.Indexes.CreateOne(index, new CreateIndexOptions
            {
                Version = 1,
                Background = true
            });
        }

        #endregion
    }
}
