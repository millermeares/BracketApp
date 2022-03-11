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

        public virtual void AddGet(string route, Delegate func)
        {
            AddRoute(HttpRequestType.Get, route, func);
        }

        public virtual void AddPost(string route, Delegate func)
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
            return Results.Ok(result);
        }

        protected static IResult GoodResult(object payload)
        {
            RequestResult req_r = RequestResult.OkResult(payload);
            return Results.Ok(req_r);
        }

        protected static IResult EmptyValidResult()
        {
            RequestResult req_r = RequestResult.OkResult();
            return Results.Ok(req_r);
        }

        internal static List<EndpointManager> GetEndpointMangers()
        {
            return new List<EndpointManager>()
            {
                new UserEndpoints(),
                new BracketEndpoints(),
                new TournamentEndpoints(),
                new AdminEndpoints(), 
                new DeveloperEndpoints()
            };
        }

        protected static IResult ReturnBasedOnDBResult(DBResult result)
        {
            RequestResult req_r = RequestResultBasedOnDBResult(result);
            return Results.Ok(req_r);
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

        protected static void RecordError(IDataAccess access, Exception ex)
        {
            access.RecordError(ex);
        }

        protected static IResult ResultFromException(IDataAccess dal, Exception ex)
        {
            RecordError(dal, ex);
            var result = RequestResult.ErrorResult("something bad and unexpected happened");
            return Results.Ok(result);
        }

        protected static IResult ResultFromBool(bool success, string error_msg)
        {
            return success ? EmptyValidResult() : ErrorResult(error_msg);
        }


        protected static UserID ConfirmDesiredAuth(AuthToken token, IUserDAL user_dal, string desired_role)
        {
            AuthenticatedUser user = user_dal.AuthenticatedUserFromToken(token);
            if (user.IsEmpty()) return UserID.MakeEmpty();
            if (!user.HasRole(desired_role))
            {
                return UserID.MakeEmpty();
            }
            return user.ID;
        }
    }
}
