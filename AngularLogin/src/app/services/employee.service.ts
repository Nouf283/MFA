import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable} from "rxjs";
import {Employee} from "../Models/employee";

@Injectable()
export class EmployeeService {
  baseApiUrl:string= 'https://localhost:44344/';
  constructor(private http:HttpClient) {}

  getAllEmployees():Observable<Employee[]>{
    return  this.http.get<Employee[]>(this.baseApiUrl + 'api/employee/getEmployees');
  }
  getExternalLoginButton():Observable<any>{
    return  this.http.post(this.baseApiUrl + 'api/auth/OnPostLoginExternally',"Facebbok");
  }
}
