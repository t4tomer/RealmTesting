using MongoDB.Bson;
using Realms;
using RealmTodo.Services;

namespace RealmTodo.Models
{
    public partial class Item : IRealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("owner_id")]
        [Required]
        public string OwnerId { get; set; }

        [MapTo("summary")]
        [Required]
        public string Summary { get; set; }

        //new attribute-latitude
        [MapTo("latitude")]
        [Required]
        public string Latitude { get; set; }




        //new attribute
        [MapTo("xummary")]
        [Required]
        public string Xummary { get; set; }

        //new attribute
        [MapTo("mapname")]
        [Required]
        public string Mapname { get; set; }


        [MapTo("isComplete")]
        public bool IsComplete { get; set; }

        public bool IsMine => OwnerId == RealmService.CurrentUser.Id;
    }
}

