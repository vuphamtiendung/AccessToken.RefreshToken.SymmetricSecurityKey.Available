namespace AccessRefreshToken.Domain.RefreshToken
{
    public interface IRefreshToken
    {
        public string GenerateRefreshToken(string userId);
    }
}
