﻿using System.Threading.Tasks;

 namespace Ianitor.Osp.Backend.Infrastructure.Initialization
{
  public interface IAsyncInitializationService
  {
    Task InitializeAsync();
  }
}