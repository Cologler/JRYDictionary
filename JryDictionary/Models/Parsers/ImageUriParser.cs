namespace JryDictionary.Models.Parsers
{
    public class ImageUriParser : UriParser
    {
        public override UriInfo TryParse(string text)
        {
            text = text.Trim();
            if (text.Length > 0 && text[0] == '!') return base.TryParse(text.Substring(1));
            return base.TryParse(text);
        }
    }
}