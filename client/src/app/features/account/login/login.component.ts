import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { AccountService } from '../../../core/services/account.service';
import { Router } from '@angular/router';
import { LoginRequest } from '../../../shared/models/loginRequest';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, MatCard,MatFormField,MatInput,MatLabel,MatButton],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  protected formBuilder = inject(FormBuilder)
  protected accountService = inject(AccountService)
  protected router = inject(Router);
  loginForm = this.formBuilder.group({
    email:[''],
    password:['']
  });

  onSubmit(){
    const loginRequest:LoginRequest = {
      email: this.loginForm.value.email!,
      password:this.loginForm.value.password!
    };
    this.accountService.login(loginRequest).subscribe({
      next: () => {
        this.accountService.getUserInfo().subscribe();
        this.router.navigateByUrl('/shop');
      }
    })
  }
}
