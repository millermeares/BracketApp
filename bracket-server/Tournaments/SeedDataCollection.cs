namespace bracket_server.Tournaments
{
    public class SeedDataCollection
    {
        private Dictionary<int, SeedData> _seedData = new Dictionary<int, SeedData>();
        public SeedDataCollection()
        {

        }
        public SeedDataCollection(List<SeedData> seedData)
        {
            foreach(SeedData data in seedData)
            {
                _seedData.Add(data.SeedID, data);
            }
        }

        public SeedData GetSeedData(int seedID)
        {
            return _seedData[seedID];
        }
    }
}
