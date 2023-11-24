namespace YoloMVC.Models
{
    public interface IDetector
    {
        public Task<List<DetectedResult>> ProcessImages(byte[] img);
    }
}