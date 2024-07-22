namespace back_end_test.BaseModule;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public abstract class BaseModule
{
    public abstract void Setup(IServiceCollection services, IConfiguration? configuration);
}

public static class ModuleExt
{
    public static void InstallModule<TModule>(this IServiceCollection services, IConfiguration? configuration)
        where TModule : BaseModule, new()
    {
        var module = new TModule();
        module.Setup(services, configuration);
    }
}