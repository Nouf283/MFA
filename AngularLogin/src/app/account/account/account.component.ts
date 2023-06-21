import { group } from '@angular/animations';
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { User } from 'src/app/Models/employee';
import { LoginDto } from 'src/app/Models/user';
import { AccountService } from 'src/app/services/account.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})

export class AccountComponent {
  loginDto: LoginDto;

  loginForm = new FormGroup({
    email: new FormControl('', Validators.required),
    password: new FormControl('',Validators.required)
  })

  constructor(public accountService:AccountService,public authService:AuthService) {
    this.loginDto = new LoginDto();
  }

  onSubmit() {
    // this.accountService.login(this.loginForm.value).subscribe({
    //   next: user => console.log(user)
    // })
    this.authService.login(this.loginDto).subscribe({
      next: user => console.log(user)
    })
 }

}
