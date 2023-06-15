import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {BehaviorSubject, Observable, map} from "rxjs";
import {Employee, User} from "../Models/employee";
import { Route, Router } from '@angular/router';

@Injectable()
export class AccountService {
private currentUserSource = new BehaviorSubject<User | null>(null);
 currentUser$ = this.currentUserSource.asObservable();
  baseApiUrl:string= 'https://localhost:44344/';
  constructor(private httpClient:HttpClient, private router:Router) {}

//   getAllUsers():Observable<User[]>{
//     return  this.http.get<User[]>(this.baseApiUrl + 'api/account/login');
//   }
  login(values: any){
      return this.httpClient.post<User>(this.baseApiUrl + 'api/account/login', values).pipe(
          map(user => {
              localStorage.setItem('token', user.token);
              this.currentUserSource.next(user);
        })
    )
    }
    
    register(values: any){
        return this.httpClient.post<User>(this.baseApiUrl + 'api/account/register', values).pipe(
            map(user => {
                localStorage.setItem('token', user.token);
                this.currentUserSource.next(user);
          })
      )
    }

    logout(values: any){
        localStorage.removeItem('token');
        this.currentUserSource.next(null);
        this.router.navigateByUrl('/');

    }
}