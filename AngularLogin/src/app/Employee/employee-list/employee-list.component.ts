import { Component,OnInit } from '@angular/core';
import {Employee} from "../../Models/employee";
import {EmployeeService} from "../../services/employee.service";
import {markAsyncChunksNonInitial} from "@angular-devkit/build-angular/src/webpack/utils/async-chunks";

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css']
})
export class EmployeeListComponent implements OnInit{
 public employees:  Employee[]=[];
 constructor(private  employeeService: EmployeeService){

 }
 ngOnInit():void{
//    this.employeeService.getAllEmployees().subscribe((response) => {
//      //Code will execute when back-end will respond
//      console.log(response);
//      this.employees = response;
//    },
//      error => {
//        console.log(error);
//      });
//  }
   
this.employeeService.getExternalLoginButton().subscribe((response) => {
  //Code will execute when back-end will respond
  //console.log(response);
  //@ts-ignore
  this.document.location.href = response;
},
  error => {
    console.log(error);
  });
}

}
