namespace Scottie.Server
{
    public class MultiOpParams
    {
        public string Path { get; set; }

        public CreateParams Create { get; set; }
        public UpdateParams Update { get; set; }
        public DeleteParams Delete { get; set; }
        public CheckVersionParams CheckVersion { get; set; }
    }
}