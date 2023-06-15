import { group } from '@angular/animations';
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { User } from 'src/app/Models/employee';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})

export class AccountComponent {
 

  loginForm = new FormGroup({
    email: new FormControl('', Validators.required),
    password: new FormControl('',Validators.required)
  })

  constructor(public accountService:AccountService) {
    
  }

  onSubmit() {
    this.accountService.login(this.loginForm.value).subscribe({
      next: user => console.log(user)
    })
 }

}
