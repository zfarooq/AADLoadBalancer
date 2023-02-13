using System.Collections.Generic;
using System.Threading;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Jwt;

namespace SP.LoadBalancer.WebAPI
{
    public class OpenIdConnectSecurityKeyProvider : IIssuerSecurityKeyProvider
    {
        public ConfigurationManager<OpenIdConnectConfiguration> ConfigManager;
        private string _issuer;
        private ICollection<SecurityKey> _keys;
        private readonly string _metadataEndpoint;
        private readonly ReaderWriterLockSlim _synclock = new ReaderWriterLockSlim();
        public OpenIdConnectSecurityKeyProvider(string metadataEndpoint)
        {
            _metadataEndpoint = metadataEndpoint;
            ConfigManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataEndpoint, new OpenIdConnectConfigurationRetriever());

            RetrieveMetadata();
        }
        public string Issuer
        {
            get
            {
                RetrieveMetadata();
                _synclock.EnterReadLock();
                try
                {
                    return _issuer;
                }
                finally
                {
                    _synclock.ExitReadLock();
                }
            }
        }
        public IEnumerable<SecurityKey> SecurityKeys
        {
            get
            {
                RetrieveMetadata();
                _synclock.EnterReadLock();
                try
                {
                    return _keys;
                }
                finally
                {
                    _synclock.ExitReadLock();
                }
            }
        }
        private void RetrieveMetadata()
        {
            _synclock.EnterWriteLock();
            try
            {
                OpenIdConnectConfiguration config = ConfigManager.GetConfigurationAsync().Result;
                _issuer = config.Issuer;
                _keys = config.SigningKeys;
            }
            finally
            {
                _synclock.ExitWriteLock();
            }
        }
    }
}