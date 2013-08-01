using System.Text;

namespace TwitterManager.Helper
{
    static class XMLSanitizer
    {
        public static string SanitizeXmlString(string xml)
        {
            if (xml == null)
                return xml;

            StringBuilder buffer = new StringBuilder(xml.Length);

            foreach (char c in xml)
                if (IsLegalXmlChar(c))
                    buffer.Append(c);

            buffer = buffer.Replace("&", "&amp;");

            return buffer.ToString();
        }

        public static bool IsLegalXmlChar(int character)
        {
            return ( character == 0x9 /* == '\t' == 9   */  || character == 0xA /* == '\n' == 10  */ ||
                     character == 0xD /* == '\r' == 13  */  || (character >= 0x20 && character <= 0xD7FF) ||
                    (character >= 0xE000 && character <= 0xFFFD) || (character >= 0x10000 && character <= 0x10FFFF) );
        }
    }
}


