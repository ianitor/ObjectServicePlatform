import {RoleDto} from "./roleDto";

export class UserDto {
  firstName: string;
  lastName: string;
  userId: string;
  email: string;
  name: string;
  roles: RoleDto[];
}
