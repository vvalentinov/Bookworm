namespace Bookworm.Common.Options.Authentication
{
    public class AuthenticationOptions
    {
        public const string Authentication = nameof(Authentication);

        public GoogleOptions Google { get; set; }

        public FacebookOptions Facebook { get; set; }
    }
}
