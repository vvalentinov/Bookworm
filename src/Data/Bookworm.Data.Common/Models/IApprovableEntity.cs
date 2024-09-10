namespace Bookworm.Data.Common.Models
{
    public interface IApprovableEntity
    {
        bool IsApproved { get; set; }
    }
}
