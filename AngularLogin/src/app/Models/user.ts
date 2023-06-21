export class User {
    id!: String;
    name!: String;
    email!: String;
    password!: String;
}
  
export class LoginDto {
    email: String;
    password: String;
    key: string;
}

export class UserDto {
    email : String;
    userName: String;
    token: string;
}

