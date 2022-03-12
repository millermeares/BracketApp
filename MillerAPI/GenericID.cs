namespace MillerAPI
{
    public class GenericID
    {
        public string ID { get; set; } = string.Empty;
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(ID);
        }
        private GenericID()
        {

        }

        public GenericID(string id)
        {
            ID = id;
        }

        public static GenericID MakeEmpty()
        {
            return new GenericID();
        }
    }
}
