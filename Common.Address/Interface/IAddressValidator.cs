using Common.Address.Model;

namespace Common.Address.Interface
{
    public interface IAddressValidator<T> where T : class
    {
        Task<T?> ValidateAsync(UserAddress input);
    }
}
