using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using LibInfluxDB.Net.Models;
using NUnit.Framework;

namespace LibInfluxDB.Net.Tests
{
    public class InfluxDbTests : TestBase
    {
        private IInfluxDb _db;

        protected override void FinalizeSetUp()
        {
            _db = new InfluxDb("http://q0q.nl:8086", "root", "D1hwvK@rst!");
            EnsureInfluxDbStarted();
        }

        protected override void FinalizeTearDown()
        {
           
        }

        private async void EnsureInfluxDbStarted()
        {
            bool influxDBstarted = false;
            do
            {
                try
                {
                    Pong response = await _db.PingAsync();
                    if (response.Status.Equals("ok"))
                    {
                        influxDBstarted = true;
                    }
                }
                catch (Exception e)
                {
                    // NOOP intentional
                }
                Thread.Sleep(100);
            } while (!influxDBstarted);

            Console.WriteLine("##################################################################################");
            Console.WriteLine("#  Connected to InfluxDB Version: " + await _db.VersionAsync() + " #");
            Console.WriteLine("##################################################################################");
        }

        private static string GetNewDbName()
        {
            return Guid.NewGuid().ToString("N").Substring(10);
            //return "LibInfluxDB";
        }

        [Test]
        public async void Create_DB_Test()
        {
            string dbName = GetNewDbName();
            InfluxDbApiCreateResponse response = await _db.CreateDatabaseAsync(dbName);
            Thread.Sleep(100);
            InfluxDbApiDeleteResponse deleteResponse = await _db.DeleteDatabaseAsync(dbName);
            Thread.Sleep(100);
            response.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Create_DB_With_Config_Test()
        {
            string dbName = Guid.NewGuid().ToString("N").Substring(10);

            InfluxDbApiCreateResponse response = await _db.CreateDatabaseAsync(new DatabaseConfiguration
            {
                Name = dbName
            });

            Thread.Sleep(100);
            InfluxDbApiDeleteResponse deleteResponse = await _db.DeleteDatabaseAsync(dbName);
            Thread.Sleep(100);
            response.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Delete_Database_Test()
        {
            string dbName = GetNewDbName();
            InfluxDbApiCreateResponse createResponse = await _db.CreateDatabaseAsync(dbName);
            Thread.Sleep(100);
            createResponse.Success.Should().BeTrue();
            Thread.Sleep(100);
            InfluxDbApiDeleteResponse response = await _db.DeleteDatabaseAsync(dbName);
            Thread.Sleep(100);
            response.Success.Should().BeTrue();
        }

        [Test]
        public async void DescribeDatabases_Test()
        {
            string dbName = GetNewDbName();
            InfluxDbApiCreateResponse createResponse = await _db.CreateDatabaseAsync(dbName);
            createResponse.Success.Should().BeTrue();

            List<Database> databases = await _db.DescribeDatabasesAsync();
            Thread.Sleep(100);

            InfluxDbApiDeleteResponse deleteResponse = await _db.DeleteDatabaseAsync(dbName);
            Thread.Sleep(100);
            databases.Should().NotBeNullOrEmpty();
            databases.Where(database => database.Name.Equals(dbName)).Should().NotBeNull();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Ping_Test()
        {
            Pong pong = await _db.PingAsync();
            pong.Should().NotBeNull();
            pong.Status.Should().BeEquivalentTo("ok");
        }

        [Test]
        public async void Query_DB_Test()
        {
            string dbName = GetNewDbName();

            InfluxDbApiCreateResponse createResponse = await _db.CreateDatabaseAsync(dbName);
            Thread.Sleep(100);
            const string TMP_SERIE_NAME = "testSeries";
            Serie serie = new Serie.Builder(TMP_SERIE_NAME)
                .Columns("value1", "value2")
                .Values(DateTime.Now.Millisecond, 5)
                .Build();
            InfluxDbApiResponse writeResponse = await _db.WriteAsync(dbName, TimeUnit.Milliseconds, serie);
            Thread.Sleep(100);
            List<Serie> series =
                await _db.QueryAsync(dbName, string.Format("select * from {0}", TMP_SERIE_NAME), TimeUnit.Milliseconds);
            Thread.Sleep(100);
            InfluxDbApiDeleteResponse deleteResponse = await _db.DeleteDatabaseAsync(dbName);
            Thread.Sleep(100);
            series.Should().NotBeNull();
            series.Count.Should().Be(1);

            createResponse.Success.Should().BeTrue();
            writeResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Test]
        public async void Write_DB_Test()
        {
            string dbName = GetNewDbName();

            InfluxDbApiCreateResponse createResponse = await _db.CreateDatabaseAsync(dbName);
            Thread.Sleep(100);
            Serie serie = new Serie.Builder("testSeries")
                .Columns("value1", "value2")
                .Values(DateTime.Now.Millisecond, 5)
                .Build();
            InfluxDbApiResponse writeResponse = await _db.WriteAsync(dbName, TimeUnit.Milliseconds, serie);
            Thread.Sleep(100);
            InfluxDbApiDeleteResponse deleteResponse = await _db.DeleteDatabaseAsync(dbName);
            Thread.Sleep(100);
            createResponse.Success.Should().BeTrue();
            writeResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }
    }
}
