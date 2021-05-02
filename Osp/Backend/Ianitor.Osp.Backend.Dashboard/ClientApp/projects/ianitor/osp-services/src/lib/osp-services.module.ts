import {ModuleWithProviders, NgModule} from '@angular/core';
import {OspServiceOptions} from "./options/osp-serviceOptions";



@NgModule({
  declarations: [],
  imports: [
  ],
  exports: []
})
export class OspServicesModule {
  static forRoot(ospServiceOptions: OspServiceOptions): ModuleWithProviders<OspServicesModule> {
    return {
      ngModule: OspServicesModule,
      providers: [
        {
          provide: OspServiceOptions,
          useValue: ospServiceOptions
        }
      ]
    }
  }
}
