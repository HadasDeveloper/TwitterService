using System.Collections.Generic;
using System.Xml;

namespace oAuthTwitterWrapper
{
    public class TwitterData
    {
        public List<XmlDocument> xmls { get; set; }
        public string error { get; set; }
    }
}
