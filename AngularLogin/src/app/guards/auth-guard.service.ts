import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { User } from '../Models/user';
import { HttpClient } from '@angular/common/http';
//import { AuthService } from './../shared/auth.service';
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
endpoint: string = 'http://localhost:4000/api';
  constructor(
  //  public authService: AuthService,
    private http:HttpClient,
    public router: Router
  ) { }
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    // if (this.authService.isLoggedIn !== true) {
    //   window.alert("Access not allowed!");
    //   this.router.navigate(['log-in'])
    // }
    return true;
  }

  signIn(user: User) {
    return this.http
      .post<any>(`${this.endpoint}/signin`, user)
      .subscribe((res: any) => {
        localStorage.setItem('access_token', res.token);
        // this.getUserProfile(res._id).subscribe((res) => {
        //   this.currentUser = res;
        //   this.router.navigate(['user-profile/' + res.msg._id]);
        // });
      });
  }
  getToken() {
    return localStorage.getItem('access_token');
  }
  get isLoggedIn(): boolean {
    let authToken = localStorage.getItem('access_token');
    return authToken !== null ? true : false;
  }
}