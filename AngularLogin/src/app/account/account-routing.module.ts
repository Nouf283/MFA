import { RouterModule, Routes } from '@angular/router';
import { AccountComponent } from './account/account.component';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: '',
    component: AccountComponent
  }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
  })
  export class AccountRoutingModule { }