export class ClientDto {
  isEnabled: boolean;
  clientId: string;
  clientName: string;
  clientUri: string;
  clientSecret: string;
  allowedGrantTypes: Array<string>;
  redirectUris: Array<string>;
  postLogoutRedirectUris: Array<string>;
  allowedCorsOrigins: Array<string>;
  allowedScopes: Array<string>;
  isOfflineAccessEnabled: boolean;
}
