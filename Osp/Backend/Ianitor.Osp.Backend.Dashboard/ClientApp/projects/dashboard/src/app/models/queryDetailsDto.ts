export class QueryDetailsDto {
  rtId: string;
  name: string;
  queryCkId: string | null;
  columns: string | null;
  sorting: string | null;
  fieldFilter: string | null;
  attributeSearchFilter: string | null;
  textSearchFilter: string | null;
}
