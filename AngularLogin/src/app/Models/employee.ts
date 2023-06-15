export class Employee{
  constructor() {
    this.id = 0;
    this.name=undefined;
    this.email = undefined;
    this.department =undefined;
    this.phone = undefined;
    this.salary= 0;
  }
  id: number;
  name:undefined;
  email:undefined;
  phone:undefined;
  department:undefined;
  salary:number;

}

export class User{
  constructor() {
    this.id = 0;
 
    this.password ="";
  }
  id: number;
  email: string;
  password: string;
  token!: string;
}


