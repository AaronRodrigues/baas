using System.Threading;
using Energy.EHLCommsLib.InterfacesHttp;
using Energy.EHLCommsLib.Models.Http;


namespace Energy.EHLCommsLibIntegrationTests.Services
{
    public sealed class AuthenticationTokenContext : IAuthenticationTokenContext
    {
        private readonly IAuthenticationTokenService _tokenService;
        private static Token _token;
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
        private readonly int _duration;

        public AuthenticationTokenContext(IAuthenticationTokenService tokenService)
        {
            _tokenService = tokenService;
            _duration = 60 + 30;
        }

        public Token CurrentToken
        {
            get
            {
                try
                {
                    _locker.EnterReadLock();
                    return _token;
                }
                finally
                {
                    _locker.ExitReadLock();
                }
            }
        }

        public void GetNewToken()
        {
            //Log.Info("Getting new Authentication Token from EHL");

            var tokenResonse = _tokenService.GenerateNew();
            UpdateToken(tokenResonse.Token);
        }

        public bool TokenExpired()
        {
            if (_token == null)
                return true;

            return (_token.ExpiryTime <= SystemTime.Now.AddSeconds(_duration));
        }

        private void UpdateToken(Token token)
        {
            try
            {
                _locker.EnterWriteLock();
                _token = token;
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }
    }
}