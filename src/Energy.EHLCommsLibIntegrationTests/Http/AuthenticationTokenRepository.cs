using System.Collections.Generic;
using Energy.EHLCommsLibIntegrationTests.Model;


namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public class AuthenticationTokenRepository : IAuthenticationTokenRepository
    {
        //private readonly IDataProvider _dataProvider;

        public AuthenticationTokenRepository(/*IDataProvider dataProvider*/)
        {
            //_dataProvider = dataProvider;
        }

        public int UpdateAuthenticationTokenAudit(Token token)
        {
            /*
             var tokenParams = new List<SqlQueryParameter>
                                  {
                                      new SqlQueryParameter {Name = "Token", Value = token.AccessToken},
                                      new SqlQueryParameter {Name = "ExpiresIn", Value = token.ExpiresIn},
                                      new SqlQueryParameter {Name = "ExpiryTime", Value = token.ExpiryTime}
                                  };

            */
            return 0;//_dataProvider.SaveData("UpdateAuthenticationTokenAudit", tokenParams);
        }
    }
}