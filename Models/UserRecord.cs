using MongoDB.Bson;
using Realms;
using RealmTodo.Services;

namespace RealmTodo.Models
{
    public partial class UserRecord : IRealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("owner_id")]
        [Required]
        public string OwnerId { get; set; }

        [MapTo("profilename")]
        [Required]
        public string ProfileName { get; set; }


        [MapTo("mapname")]
        [Required]

        public string MapName { get; set; }


        [MapTo("tracktime")]
        [Required]
        public string TrackTime { get; set; }



        [MapTo("uploadateime")] // the date and time the user uploaded to mongodb.
        [Required]

        public string UploadDateTime { get; set; }


        [MapTo("comment")] // user comments 
        [Required]

        public string Comment { get; set; }



        public bool IsMine => OwnerId == RealmService.CurrentUser.Id;
    }
}

