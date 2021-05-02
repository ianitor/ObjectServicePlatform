using System;

namespace Ianitor.Osp.Frontend.Client
{
    public interface IServiceClient
    {
        IServiceClientAccessToken AccessToken { get; }
        
        Uri ServiceUri { get; }

        void Initialize();
    }
}