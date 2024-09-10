using RoadOfGroping.Core.ZRoadOfGropingUtility.EventBus.Attributes;

namespace RoadOfGroping.Core.Files.Dtos
{
    [EventDiscriptor("FileEvent")]
    public class FileEventDto
    {
        public Stream Stream { get; set; }
        public string File { get; set; }
        public string ContentType { get; set; }

        public FileEventDto(Stream stream, string file, string contentType)
        {
            Stream = stream;
            File = file;
            ContentType = contentType;
        }
    }
}