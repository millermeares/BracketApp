using MillerAPI;
using MillerAPI.DataAccess;

namespace bracket_server.Routing
{
    public abstract class EndpointManager
    {
        
        public enum HttpRequestType { Post, Get }
        private Dictionary<HttpRequestType, Dictionary<string, Delegate>> _routes = new Dictionary<HttpRequestType, Dictionary<string, Delegate>>();

        public EndpointManager()
        {
            _routes.Add(HttpRequestType.Post, new Dictionary<string, Delegate>());
            _routes.Add(HttpRequestType.Get, new Dictionary<string, Delegate>());
            AddRoutes();
        }
        public abstract void AddRoutes();

        public void RegisterRoutes(WebApplication app)
        {
            AddPosts(app);
            AddGets(app);
        }

        

        private void AddGets(WebApplication app)
        {
            foreach (string route in _routes[HttpRequestType.Get].Keys)
            {
                app.MapGet(route, _routes[HttpRequestType.Get][route]);
            }
        }

        private void AddPosts(WebApplication app)
        {
            foreach(string route in _routes[HttpRequestType.Post].Keys)
            {
                app.MapPost(route, _routes[HttpRequestType.Post][route]);
            }
        }

        public void AddGet(string route, Delegate func)
        {
            AddRoute(HttpRequestType.Get, route, func);
        }

        public void AddPost(string route, Delegate func)
        {
            AddRoute(HttpRequestType.Post, route, func);
        }

        private void AddRoute(HttpRequestType requestType, string route, Delegate func)
        {
            if (!_routes.ContainsKey(requestType))
            {
                _routes.Add(requestType, new Dictionary<string, Delegate>());
            }
            _routes[requestType].Add(route, func);
        }

        protected static IResult ErrorResult(string message)
        {
            RequestResult result = RequestResult.ErrorResult(message);
            return OkWithRequestResult(result);
        }


        internal static List<EndpointManager> GetEndpointMangers()
        {
            return new List<EndpointManager>()
            {
                new UserEndpoints(),
                new BracketEndpoints(),
                new TournamentEndpoints(),
            };
        }

        protected static IResult ReturnBasedOnDBResult(DBResult result)
        {
            RequestResult req_r = RequestResultBasedOnDBResult(result);
            return OkWithRequestResult(req_r);
        }

        protected static IResult OkWithRequestResult(RequestResult result)
        {
            return Results.Ok(result);
        }

        protected static RequestResult RequestResultBasedOnDBResult(DBResult result)
        {
            switch(result)
            {
                case DBResult.Success:
                    return RequestResult.OkResult();
                case DBResult.Fail:
                    return RequestResult.ErrorResult("Database Error");
                default:
                    return RequestResult.ErrorResult("something weird happened");
            }
        }
    }
}
