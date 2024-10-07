    using MongoDB.Bson;
    using Realms;
    using RealmTodo.Services;

    namespace RealmTodo.Models
    {
        public partial class PinOnMap : IRealmObject
        {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; }

        [MapTo("owner_id")]
        public string OwnerId { get; set; }

        [MapTo("latitude")]
        public double Latitude { get; set; }

        [MapTo("longitude")]
        public double Longitude { get; set; }

        [MapTo("label")]
        public string Label { get; set; }

        [MapTo("mapname")]
        public string Mapname { get; set; }

        public bool IsMine => OwnerId == RealmService.CurrentUser.Id;



    }
    }
