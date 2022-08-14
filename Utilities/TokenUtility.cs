using aspnetapp.Entities;
using aspnetapp.Helpers;

namespace aspnetapp.Utilities
{
    public static class TokenUtility
    {
        public static bool isTokenValid(DataContext context, string id) {
            Token? token = Entities.Token.find(context, Guid.Parse(id));
            if (token == null) return false;
            if (token.IsBanned) return false;
            if (DateTime.UtcNow-token.CreatedAt > TimeSpan.FromDays(7)) return false;
            return true;
        }

        public static bool isTokenValid(DataContext context, Guid id) {
            Token? token = Entities.Token.find(context, id);
            if (token == null) return false;
            if (token.IsBanned) return false;
            if (DateTime.UtcNow-token.CreatedAt > TimeSpan.FromDays(7)) return false;
            return true;
        }
    }
}