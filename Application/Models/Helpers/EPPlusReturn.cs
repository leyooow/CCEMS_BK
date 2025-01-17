

namespace Application.Models.Helpers
{
    public class EPPlusReturn
    {
        public string FileName { get; set; } = "";
        public byte[] FileByte { get; set; } = [];
        public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}
