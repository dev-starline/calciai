namespace CalciAI.Models
{
    public class StringIdentifier : IModel
    {
        public StringIdentifier()
        {
        }

        public StringIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; set; }
    }
}