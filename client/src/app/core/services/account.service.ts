import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { User } from '../../shared/models/user';
import { Address } from '../../shared/models/address';
import { LoginRequest } from '../../shared/models/loginRequest';
import { RegisterRequest } from '../../shared/models/registerRequest';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private baseURL = environment.apiURL;
  currentUser = signal<User | null>(null);
  login(values:LoginRequest){
    let params = new HttpParams();
    params = params.append('useCookies',true);
    return this.http.post<User>(this.baseURL + 'login',values,{params,withCredentials:true});
  }
  register(values:RegisterRequest){
    return this.http.post(this.baseURL + 'account/register',values)
  }
  getUserInfo(){
    return this.http.get<User>(this.baseURL + 'account/user-info',{withCredentials:true}).subscribe({
      next: response => this.currentUser.set(response)
    })
  }
  logout(){
    return this.http.post(this.baseURL + 'account/logout',{},{withCredentials:true});
  }
  updateAddress(address:Address){
    return this.http.post(this.baseURL + 'account/address',address);
  }
}
