import {FailedDetails} from "./failedDetails";

export class ApiError {

  statusCode: number;
  statusDescription: string;
  message: string;
  details: Array<FailedDetails>;
}
