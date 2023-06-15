import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountComponent } from './account/account.component';
import { AccountRoutingModule } from './account-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { AccountService } from '../services/account.service';


@NgModule({
  declarations: [
    AccountComponent,
    
  ],
  imports: [
    CommonModule,
    AccountRoutingModule,
    ReactiveFormsModule
    
  ],
  providers: [
    AccountService
  ]
})
export class AccountModule { }
