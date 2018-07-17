using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LiteDB;

namespace DominoBlockchain
{
    public enum CollectionType { Block, Transaction }

    public class DbCollection
    {
        public class DbData
        {
            public object Collection { get; set; }
            public bool IsMutated { get; set; }
            public int Count { get; set; }

            public object FullCollection { get; set; }
        }

        private readonly Dictionary<CollectionType, DbData> _internalDictionary = new Dictionary<CollectionType, DbData>();

        // The Enumerator
        public Dictionary<CollectionType, DbData>.Enumerator GetEnumerator() => _internalDictionary.GetEnumerator();

        // Set method
        private void SetCollection<T>(LiteCollection<T> collection) => _internalDictionary[typeof(T).Name.ParseType()].Collection = collection;

        /// Add method
        private void AddCollection<T>(LiteCollection<T> collection) => _internalDictionary.Add(typeof(T).Name.ParseType(), new DbData { Collection = collection, Count = collection.Count(), IsMutated = false});

        // SetOrAdd method
        public void SetOrAdd<T>(LiteCollection<T> collection)
        {
            var type = typeof(T).Name.ParseType();
            if (ContainsType(type)) SetCollection(collection);
            else AddCollection(collection);
        }

        // Get method
        public LiteCollection<T> Get<T>(CollectionType type) => (LiteCollection<T>) _internalDictionary[type].Collection;

        // GetFull method
        public object GetFull(CollectionType type) => _internalDictionary[type].FullCollection;

        // SetFull method
        public void SetFull<T>(IEnumerable<T> list) => _internalDictionary[typeof(T).Name.ParseType()].FullCollection = list;

        // Count method
        public int Count<T>(LiteCollection<T> collection)
        {
            var type = collection.Name.ParseType();
            if (ContainsType(type) && !IsMutated(type)) return _internalDictionary[type].Count;
            return _internalDictionary[type].Count = collection.Count();
        }

        public void SetMutated<T>(LiteCollection<T> collection, bool value) => _internalDictionary[typeof(T).Name.ParseType()].IsMutated = value;

        public bool IsMutated(CollectionType type) => _internalDictionary[type].IsMutated;

        public bool ContainsType(CollectionType type) => _internalDictionary.ContainsKey(type);
    }

    public static class Database
    {
        internal static readonly DbCollection DatabaseCollection = new DbCollection();
        internal static LiteDatabase db;

        private const string DatabaseDirectory = @"bridge\resources\DominoBlockchain\database";
        private const string DatabaseFile = DatabaseDirectory + @"\ledger.db";

        public static void Initialize()
        {
            if (!Directory.Exists(DatabaseDirectory)) Directory.CreateDirectory(DatabaseDirectory);

            db = new LiteDatabase(DatabaseFile);

            GetFullCollection<Block>();
            GetFullCollection<Transaction>();
        }

        public static LiteCollection<T> GetCollection<T>()
        {
            CollectionType colType = typeof(T).Name.ParseType();

            if (DatabaseCollection.ContainsType(colType) && !DatabaseCollection.IsMutated(colType))
                return DatabaseCollection.Get<T>(colType);

            LiteCollection<T> freshCollection = db.GetCollection<T>(colType.ToString());
            DatabaseCollection.SetOrAdd(freshCollection);
            return freshCollection;
        }

        public static List<T> GetFullCollection<T>()
        {
            CollectionType colType = typeof(T).Name.ParseType();

            if (!DatabaseCollection.ContainsType(colType) || DatabaseCollection.IsMutated(colType))
            {
                GetCollection<T>();
                return null;
            }

            var collectionObj = (List<T>) DatabaseCollection.GetFull(colType);
            if (collectionObj != null) return collectionObj;

            var getCollection = GetCollection<T>();
            var findCollections = getCollection.FindAll().ToList();
            DatabaseCollection.SetFull(findCollections);
            return findCollections;
        }

        public static bool CollectionExists<T>()
        {
            return DatabaseCollection.ContainsType(typeof(T).Name.ParseType());
        }
    }

    public static class LiteCollectionExtensions
    {
        public static int CountEx<T>(this LiteCollection<T> collection)
        {
            return Database.DatabaseCollection.Count(collection);
        }

        public static void InsertEx<T>(this LiteCollection<T> collection, T data)
        {
            collection.Insert(data);
            Database.DatabaseCollection.SetMutated(collection, true);
        }
    }
}
