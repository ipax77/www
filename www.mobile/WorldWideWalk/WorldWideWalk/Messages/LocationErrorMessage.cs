namespace WorldWideWalk.Messages
{
    public class LocationErrorMessage
    {
        public string Error { get; set; }
        public bool isFatal { get; set; } = false;
    }
}
