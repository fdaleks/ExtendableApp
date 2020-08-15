namespace CommonSnappableTypes
{
    public interface IAppFunctionality
    {
        void DoIt();
    }

    public sealed class CompanyInfoAttribute : System.Attribute
    {
        public string CompanyName { get; set; }
        public string CompanyUrl { get; set; }
    }
}
