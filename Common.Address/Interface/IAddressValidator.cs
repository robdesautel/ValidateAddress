using Common.Address.Enumerators;
using Common.Address.Model;
using Common.Address.Model.HerePlatform;

namespace Common.Address.Interface
{
    public interface IAddressValidator<T> where T : class
    {
        Task<T?> ValidateAsync(UserAddress input);
        Task<bool?> IsValidPostalCode(string postalCode);
        Task<bool?> IsValidLocation(List<Dictionary<SubqueryType, string>> subQueries);
        Task<bool?> IsValidLocation(SubqueryList subqueries);

    }
}
