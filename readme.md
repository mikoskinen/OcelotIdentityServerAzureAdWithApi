# Ocelot & IdentityServer & AzureAD

Demo for a scenario where the following tools are used:

* Ocelot Gateway
* Api behind the Ocelot Gateway
* Azure AD
* IdentityServer

Requires .NET Core SDK 3.1.100.

In this demo, routing to IdentityServer doesn't happen through Ocelot. Instead, the following model is used:

* User or API & Azure Ad & IdentityServer are "outside" the gateway's regime.
* Api-projects are "inside" the Ocelot Gateway. 

Similar to this:

![](https://lh6.googleusercontent.com/pdgGksm0pm0p64dk4JtYzdQmXAHTPVwI9-S_yOF925YH299PT6H18dsQ_2XH6BTdx1NFAYXsoZ2_e9LyO7mWpO32vAbMCVEcUJa_fws-qLPvVatfW656JKE_8UJMLJ_cQT49y8eb)

It *should be* possible to move IdentityServer "inside" the Ocelot's regime.

## Testing

Create a new Azure AD app registration:

![](azuread.png)

Copy the Azure Ad application settings to IdentityServer/Startup.cs.

Then run the applications in the following order:

1. Api (should open to https://localhost:44392/)
2. Gateway (https://localhost:44334/)
3. IdentityServer (http://localhost:5000/)
4. MvcTestSite (https://localhost:44322/)

To test, click "Call API with Client ID (not using authenticated user)" from the home page. Use should see something like the following: Hello from Weather API. Claims: nbf:1575980295, exp:1575983895, iss:http://localhost:5000, aud:api1, client_id:client, scope:api1

Then navigate to "Privacy"-page and authenticate with local user (bob/bob) or using AzureAd. Click "Call API with authenticated user". You should see the following: Hello from Weather API. Claims: nbf:1575980335, exp:1575983935, iss:http://localhost:5000, aud:api1, client_id:mvc, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier:88421113, auth_time:1575980335, http://schemas.microsoft.com/identity/claims/identityprovider:local, http://schemas.microsoft.com/ws/2008/06/identity/claims/role:Administrator, http://schemas.microsoft.com/ws/2008/06/identity/claims/role:User, name:bob, scope:openid, scope:profile, scope:api1, http://schemas.microsoft.com/claims/authnmethodsreferences:pwd