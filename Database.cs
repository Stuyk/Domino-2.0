using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LiteDB;

namespace DominoBlockchain
{
    public static class Database
    {
        private static bool _ledgerMutated;

        private static int _count;
        public static int Count
        {
            get
            {
                if (_ledgerMutated) _count = LedgerCollection.Count();
                return _count;
            }
        }

        private static LiteCollection<Block> _ledgerCollection;
        public static LiteCollection<Block> LedgerCollection
        {
            get
            {
                if (_ledgerMutated)
                {
                    _ledgerCollection = Get<Block>();
                    _ledgerMutated = false;
                }
                return _ledgerCollection;
            }
        }

        private static Block[] _fullLedgerCollection;
        public static Block[] FullLedgerCollection
        {
            get
            {
                if (_ledgerMutated)
                {
                    _fullLedgerCollection = Get<Block>()?.FindAll().ToArray();
                    _ledgerMutated = false;
                }
                return _fullLedgerCollection;
            }
        }

        public static Block LastBlock => _ledgerCollection?.FindOne(Query.All(Query.Descending));

        private static readonly char Path = System.IO.Path.DirectorySeparatorChar;
        private static readonly string AssemblyName = typeof(Database).Assembly.GetName().Name;
        private static string _databaseDirectory;
        private static string _databaseFile;

        public static void Initialize()
        {
            _databaseDirectory = $"bridge{Path}resources{Path}{AssemblyName}{Path}database";
            if (!Directory.Exists(_databaseDirectory)) Directory.CreateDirectory(_databaseDirectory);
            _databaseFile = $"{_databaseDirectory}{Path}ledger.db";

            // Debugging purposes
            var fullCollection = FullLedgerCollection;
        }

        // private for the sake of caching
        private static LiteCollection<T> Get<T>()
        {
            var colName = typeof(T).Name;
            using (var db = new LiteDatabase(_databaseFile)) return db.CollectionExists(colName) ? db.GetCollection<T>(colName) : null;
        }

        public static bool Exists<T>()
        {
            using (var db = new LiteDatabase(_databaseFile)) return db.CollectionExists(typeof(T).Name);
        }

        public static void Add<T>(T data)
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                db.GetCollection<T>().Insert(data);
                _ledgerMutated = true;
            }
        }
    }
}
