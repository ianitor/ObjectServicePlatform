import {PagedResultDto} from "@ianitor/shared-services";

export class PagedGraphResultDto<P, C> extends PagedResultDto<C>{

  document : P;
}
