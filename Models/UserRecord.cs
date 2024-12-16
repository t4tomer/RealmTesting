using MongoDB.Bson;
using Realms;
using RealmTodo.Services;

namespace RealmTodo.Models
{
    public partial class UserRecord 
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("owner_id")]
        [Required]
        public string OwnerId { get; set; }

        [MapTo("profilename")]
        [Required]
        public string Profilename { get; set; }


        [MapTo("mapname")]
        [Required]

        public string Mapname { get; set; }

        [MapTo("tracktime")]
        [Required]

        public string TrackTime { get; set; }




        [MapTo("uploadateime")] // the date and time the user uploaded to mongodb.
        [Required]

        public string UploadDateTime { get; set; }



        public bool IsMine => OwnerId == RealmService.CurrentUser.Id;
    }
}

