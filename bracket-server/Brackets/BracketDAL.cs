using MillerAPI.DataAccess;

namespace bracket_server.Brackets
{
    // this will be used but not yet.
    public class BracketDAL : BaseDAL
    {
        public BracketDAL(IDataAccess access) : base(access) { }

        public override string GetExceptionCategory()
        {
            return "bracket";
        }
    }
}
