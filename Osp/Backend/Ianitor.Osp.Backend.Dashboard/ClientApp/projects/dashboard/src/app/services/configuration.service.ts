import { Injectable } from '@angular/core';
import {ConfigurationDto} from "../models/configurationDto";

const clientId = "osp-dashboard";

@Injectable()
export class ConfigurationService {

  private configuration: ConfigurationDto;

  constructor() { }

  public async loadConfig() {
    console.debug("loading config");

    const result = await fetch(`/_configuration/${clientId}`);
    if (!result.ok) {
      throw new Error(`Could not load settings for '${clientId}'`);
    }

    this.configuration = <ConfigurationDto> await result.json();

    console.debug("end loading config function");
  }

  public get config(): ConfigurationDto {
    return this.configuration;
  }
}
