namespace CoreCodeCamp.Models
{
    public class TalkModel
    {
        // Id é necessário para identificar o Talk
        public int TalkId { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public int Level { get; set; }

        public SpeakerModel Speaker { get; set; }
    }
}