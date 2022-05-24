using Microsoft.Extensions.Configuration;

public class DelayedConfigurationSource : IConfigurationSource
{
    private IConfigurationProvider Provider { get; } = new DelayedConfigurationProvider();
    public IConfigurationProvider Build(IConfigurationBuilder builder) => Provider;
    public void Set(string key, string value) => Provider.Set(key, value);

    private class DelayedConfigurationProvider : ConfigurationProvider
    {
        public override void Set(string key, string value)
        {
            base.Set(key, value);
            OnReload();
        }
    }
}