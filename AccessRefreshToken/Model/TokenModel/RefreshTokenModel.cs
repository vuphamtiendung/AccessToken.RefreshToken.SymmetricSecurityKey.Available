namespace AccessRefreshToken.Model.TokenModel
{
    public class RefreshTokenModel
    {
        public string ? Token { get; set; }
        public string ? UserId { get; set; }
        public DateTime? Expiration { get; set; }
        public bool IsRevoke { get; set; }  
    }
}
