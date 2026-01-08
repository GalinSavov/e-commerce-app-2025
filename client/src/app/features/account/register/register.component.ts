import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { AccountService } from '../../../core/services/account.service';
import { Router } from '@angular/router';
import { RegisterRequest } from '../../../shared/models/registerRequest';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { TextInputComponent } from "../../../shared/components/text-input/text-input.component";

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, MatCard, MatButton, TextInputComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  protected formBuilder = inject(FormBuilder);
  protected accountService = inject(AccountService);
  protected router = inject(Router);
  protected snackbarService = inject(SnackbarService);
  validationErrors?:string[];

  registerForm = this.formBuilder.group({
    firstName:['',Validators.required],
    lastName:['',Validators.required],
    email:['',[Validators.required,Validators.email]],
    password:['',Validators.required]
  });
  onSubmit(){
    const registerRequest:RegisterRequest ={
      firstName: this.registerForm.value.firstName!,
      lastName:this.registerForm.value.lastName!,
      email:this.registerForm.value.email!,
      password:this.registerForm.value.password!
    }
    this.accountService.register(registerRequest).subscribe({
      next:() => {
        this.snackbarService.success("Registration successful!");
        this.router.navigateByUrl('/account/login');
      },
      error: errors => this.validationErrors = errors
    })
  }
}
