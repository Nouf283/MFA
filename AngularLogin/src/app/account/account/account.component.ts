import { group } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { User } from 'src/app/Models/employee';
import { LoginDto } from 'src/app/Models/user';
import { AccountService } from 'src/app/services/account.service';
import { AuthService } from 'src/app/services/auth.service';
import { DomSanitizer } from '@angular/platform-browser'

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})

export class AccountComponent implements OnInit{
  loginDto: LoginDto;
  public file: any;
  imageUrl: any;
  loginForm = new FormGroup({
    email: new FormControl('', Validators.required),
    password: new FormControl('',Validators.required)
  })
  sanitizer: any;

  constructor(public accountService:AccountService,public authService:AuthService,public _sanitizer: DomSanitizer) {
    this.loginDto = new LoginDto();
  }

  ngOnInit() {
    this.authService.getQRScanImage().subscribe({
      next: x => this.loadData(x)
    });
  }

  

  loadData(x: any) {
    this.file = x;
    let _pictype = ".png";
    if (this.file !== undefined && this.file !== null) {
      let _type = this.function(_pictype);
      const byteCharacters = window.atob(this.file);
      const byteNumbers = new Array(byteCharacters.length);
      for (let i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
      }
      const byteArray = new Uint8Array(byteNumbers);
      const blob = new Blob([byteArray], { type: _type });
      const url = URL.createObjectURL(blob);
       //this.imageUrl = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${blob}`);
       this.imageUrl = this._sanitizer.bypassSecurityTrustResourceUrl(url);
      // const objectUrl: string = URL.createObjectURL(blob);
      
       
    }
}

function(ext:any) {
    if (ext !== undefined) {
        return this.extToMimes(ext);
    }
    return undefined;
}

extToMimes(ext:any) {
    let type = undefined;
    switch (ext) {
        case 'jpg':
            type = 'image/jpeg';
            break;
        case 'png':
            type = 'image/jpeg';
            break;
        case 'jpeg':
            type = 'image/jpeg';
            break;
        case 'txt':
            type = 'text/plain';
            break;
        case 'xls':
            type = 'application/vnd.ms-excel';
            break;
        case 'doc':
            type = 'application/msword';
            break;
        case 'xlsx':
            type = 'application/vnd.ms-excel';
            break;
        case 'pdf':
            type = 'application/pdf';
            break;
        default:

    }
    return type;
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
