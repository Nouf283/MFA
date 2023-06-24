import { Injectable } from '@angular/core';
import { LoginDto, User, UserDto } from '../Models/user';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpErrorResponse} from '@angular/common/http';
import { Router } from '@angular/router';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
private userSubject: BehaviorSubject<User | null>;
private currentUserSource = new BehaviorSubject<User | null>(null);
 currentUser$ = this.currentUserSource.asObservable();
  public user: Observable<User | null>;
 
  endpoint: string = 'https://localhost:44305/api';
  headers = new HttpHeaders().set('Content-Type', 'application/json');
  currentUser = {};
  constructor(private http: HttpClient, public router: Router) {}
  // Sign-up
  signUp(user: User): Observable<any> {
    let api = `${this.endpoint}/register-user`;
    return this.http.post(api, user).pipe(catchError(this.handleError));
  }
  // Sign-in
//   signIn(user: User) {
//     return this.http
//       .post<any>(`${this.endpoint}/signin`, user)
//       .subscribe((res: any) => {
//         localStorage.setItem('access_token', res.token);
//         this.getUserProfile(res._id).subscribe((res) => {
//           this.currentUser = res;
//           this.router.navigate(['user-profile/' + res.msg._id]);
//         });
//       });
//   }
login(loginDto:LoginDto) {
    return this.http.post<any>(`${this.endpoint}/account/login`, loginDto)
        .pipe(map(user => {
            // store user details and jwt token in local storage to keep user logged in between page refreshes
            localStorage.setItem('user', JSON.stringify(user));
            this.currentUserSource.next(user);
            return user;
        }));
}

logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('user');
    this.userSubject.next(null);
    this.router.navigate(['/login']);
}
    
  getToken() {
    return localStorage.getItem('access_token');
  }
  get isLoggedIn(): boolean {
    let authToken = localStorage.getItem('access_token');
    return authToken !== null ? true : false;
  }
  doLogout() {
    let removeToken = localStorage.removeItem('access_token');
    if (removeToken == null) {
      this.router.navigate(['log-in']);
    }
  }
  // User profile
  getUserProfile(id: any): Observable<any> {
    let api = `${this.endpoint}/user-profile/${id}`;
    return this.http.get(api, { headers: this.headers }).pipe(
      map((res) => {
        return res || {};
      }),
      catchError(this.handleError)
    );
  }

  getQRScanImage(): Observable<any> {
   // let email = "cleinttest123@gmail.com";
   let email = "noufawal0311@gmail.com";
    let api = `${this.endpoint}/qrcodeAuthMFA/qrcode-setup/${email}`;
    return this.http.get(api, { headers: this.headers }).pipe(
      map((res) => {
        console.log(res);
        return res;
      }),
      catchError(this.handleError)
    );
  }


  // Error
  handleError(error: HttpErrorResponse) {
    let msg = '';
    if (error.error instanceof ErrorEvent) {
      // client-side error
      msg = error.error.message;
    } else {
      // server-side error
      msg = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(msg);
  }

  
}