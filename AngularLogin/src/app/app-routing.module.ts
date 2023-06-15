import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmployeeListComponent } from './Employee/employee-list/employee-list.component';

const routes: Routes = [
  // { path: '', redirectTo: 'employee', pathMatch: 'full' },
  // {
  //   path: 'employee',
  //   component: EmployeeListComponent,
  // },
  {
    path: 'account',
    loadChildren: () => import('./account/account.module').then(m => m.AccountModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
