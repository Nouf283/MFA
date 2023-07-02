import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountComponent } from './account/account.component';
import { AccountRoutingModule } from './account-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { AuthService } from 'src/app/services/auth.service';
import { FormsModule } from '@angular/forms';
// import { LinkedInSdkModule } from 'angular-linkedin-sdk';


@NgModule({
  declarations: [
    AccountComponent
    
  ],
  imports: [
   // LinkedInSdkModule,
    CommonModule,
    AccountRoutingModule,
    ReactiveFormsModule,
    FormsModule
    
  ],
  providers: [
    AccountService,
    AuthService
  ]
})
export class AccountModule { }
