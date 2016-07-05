
namespace CAF.Infrastructure.Core.Data.Hooks
{
    public interface IHook
    {
        void HookObject(object entity, HookEntityMetadata metadata);
    }
}
