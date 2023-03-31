using Syncfusion.Blazor;
namespace Task_management_system.Services;
public class SyncfusionLocalizer : ISyncfusionStringLocalizer
{
    public string GetText(string key)
    {
        return ResourceManager.GetString(key);
    }
    public System.Resources.ResourceManager ResourceManager
    {
        get
        {
            // Replace the ApplicationNamespace with your application name.
            return Task_management_system.Resources.SfResources.ResourceManager;
        }
    }
}