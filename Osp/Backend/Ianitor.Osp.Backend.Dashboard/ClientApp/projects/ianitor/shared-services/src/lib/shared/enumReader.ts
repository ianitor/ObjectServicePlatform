export class EnumTuple {
  id: number;
  name: string;
}

export class EnumReader<TEnum> {

  constructor(private value: TEnum) {
  }

  public getNamesAndValues(): EnumTuple[] {
    return this.getNames().map((n) => {
      return <EnumTuple>{name: n, id: this.value[n] as number};
    });
  }

  public getNames() : string[] {
    return this.getObjValues().filter(v => typeof v === 'string') as string[];
  }

  public getValues<T extends number>() {
    return this.getObjValues().filter(v => typeof v === 'number') as T[];
  }

  private getObjValues(): (number | string)[] {
    return Object.keys(this.value).map(k => this.value[k]);
  }
}
