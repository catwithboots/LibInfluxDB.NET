using System.Net.Http;

namespace LibInfluxDB.Net
{
    internal interface IRequestContent
    {
        HttpContent GetContent();
    }
}